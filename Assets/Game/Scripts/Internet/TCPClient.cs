using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    [Header("TCP设置")]
    public string serverIP = "127.0.0.1";
    public int port = 2222;

    [Header("重连设置")]
    public bool autoReconnect = true;
    public float reconnectDelay = 5f;

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiveThread;
    private bool _isConnected;
    private bool _shouldStop;
    private CancellationTokenSource _cancellationTokenSource;
    private UnityMainThreadDispatcher _mainThreadDispatcher;

    int _peakQueueSize = 0; // 每帧最多处理的消息数量
    int maxFrame = 15; // 每帧最多处理的消息数量
    bool isProcessiong = false;
    object _queueLock = new object();
    private Queue<string> _messageQueue = new Queue<string>();

    // 消息缓冲区
    private StringBuilder _messageBuffer = new StringBuilder();
    private object _bufferLock = new object(); // 缓冲区锁

    // 用于触发缓冲区处理
    private bool _hasNewData = false;
    private Coroutine _bufferProcessingCoroutine;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        // 在主线程中预先获取UnityMainThreadDispatcher实例
        _mainThreadDispatcher = UnityMainThreadDispatcher.Instance;

        if (_mainThreadDispatcher == null)
        {
            Debug.LogError("无法获取UnityMainThreadDispatcher实例！");
            return;
        }

        ConnectToServer();

        // 启动缓冲区处理协程
        _bufferProcessingCoroutine = StartCoroutine(ProcessBufferCoroutine());
    }

    private void ConnectToServer()
    {
        try
        {
            _shouldStop = false;
            _cancellationTokenSource = new CancellationTokenSource();

            _client = new TcpClient();
            _client.Connect(serverIP, port);
            _stream = _client.GetStream();
            _isConnected = true;

            _receiveThread = new Thread(ReceiveData);
            _receiveThread.IsBackground = true;
            _receiveThread.Start();
            Debug.Log($"连接到服务器 {serverIP}:{port}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"连接失败: {e.Message}");
            _isConnected = false;

            if (autoReconnect)
            {
                StartCoroutine(TryReconnect());
            }
        }
    }

    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        while (_isConnected && !_shouldStop)
        {
            try
            {
                if (_stream == null || !_stream.CanRead)
                    break;

                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // 连接已关闭

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // 直接将数据添加到缓冲区，不进行复杂处理
                lock (_bufferLock)
                {
                    _messageBuffer.Append(data);
                    _hasNewData = true;
                }
            }
            catch (System.Exception e)
            {
                if (!_shouldStop) // 只在非主动关闭时记录错误
                {
                    Debug.LogError($"接收出错: {e.Message}");
                    _mainThreadDispatcher?.Enqueue(() => OnConnectionLost());
                }
                break;
            }
        }
    }

    // 新增：协程处理缓冲区
    private IEnumerator ProcessBufferCoroutine()
    {
        while (!_shouldStop)
        {
            yield return null; // 每帧检查一次

            if (!_hasNewData) continue;

            lock (_bufferLock)
            {
                _hasNewData = false;
                string bufferContent = _messageBuffer.ToString();

                if (string.IsNullOrEmpty(bufferContent)) continue;

                // 使用JSON对象边界检测
                int startIndex = 0;
                int braceCount = 0;
                bool inString = false;
                char prevChar = '\0';
                bool processedAny = false;

                for (int i = 0; i < bufferContent.Length; i++)
                {
                    char c = bufferContent[i];

                    // 处理字符串内的转义字符
                    if (inString && c == '\\' && prevChar != '\\')
                    {
                        prevChar = c;
                        continue;
                    }

                    // 处理字符串开始/结束
                    if (c == '"' && prevChar != '\\')
                    {
                        inString = !inString;
                    }

                    // 只在非字符串区域内计数大括号
                    if (!inString)
                    {
                        if (c == '{')
                        {
                            if (braceCount == 0) startIndex = i; // 记录JSON对象开始位置
                            braceCount++;
                        }
                        else if (c == '}')
                        {
                            braceCount--;

                            // 找到一个完整的JSON对象
                            if (braceCount == 0)
                            {
                                int length = i - startIndex + 1;
                                string jsonMessage = bufferContent.Substring(startIndex, length);

                                // 将完整消息加入处理队列
                                EnqueueMessage(jsonMessage);

                                // 标记已处理消息
                                processedAny = true;
                            }
                        }
                    }

                    prevChar = c;
                }

                // 如果处理了任何消息，更新缓冲区
                if (processedAny)
                {
                    // 找到最后一个处理的消息的结束位置
                    int lastProcessedIndex = bufferContent.LastIndexOf('}') + 1;
                    if (lastProcessedIndex > 0 && lastProcessedIndex <= bufferContent.Length)
                    {
                        _messageBuffer.Remove(0, lastProcessedIndex);
                    }
                }
            }
        }
    }

    // 将消息加入队列
    private void EnqueueMessage(string message)
    {
        lock (_queueLock)
        {
            _messageQueue.Enqueue(message);
            // 更新峰值队列大小
            if (_messageQueue.Count > _peakQueueSize)
            {
                _peakQueueSize = _messageQueue.Count;
            }
        }

        // 如果还没有开始处理队列，启动处理
        if (!isProcessiong)
        {
            StartCoroutine(ProcessQueueCoroutine());
        }
    }

    private IEnumerator ProcessQueueCoroutine()
    {
        isProcessiong = true;

        while (true)
        {
            int frame = 0;

            lock (_queueLock)
            {
                // 处理当前帧的消息
                while (_messageQueue.Count > 0 && frame < maxFrame)
                {
                    string data = _messageQueue.Dequeue();
                    ProcessSingleMessage(data);
                    frame++;
                }

                // 如果队列为空，跳出循环
                if (_messageQueue.Count == 0)
                {
                    break;
                }
            }

            yield return null; // 等待下一帧继续处理
        }

        isProcessiong = false;
    }

    private void ProcessSingleMessage(string data)
    {
        try
        {
            DataInfo danmu = JsonUtility.FromJson<DataInfo>(data);
            NetManager.Instance.OnDispseMsg(danmu);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"处理消息失败: {e.Message}\n原始数据: {data}");
        }
    }

    public void Send(string message)
    {
        if (!_isConnected || _stream == null) return;

        try
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"发送失败: {e.Message}");
            OnConnectionLost();
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        _shouldStop = true;
        _isConnected = false;

        _cancellationTokenSource?.Cancel();

        if (_receiveThread != null && _receiveThread.IsAlive)
        {
            _receiveThread.Join(1000); // 等待线程结束，最多1秒
        }

        // 停止协程
        if (_bufferProcessingCoroutine != null)
        {
            StopCoroutine(_bufferProcessingCoroutine);
        }

        _stream?.Close();
        _client?.Close();

        _cancellationTokenSource?.Dispose();
    }

    private void OnConnectionLost()
    {
        _isConnected = false;

        if (autoReconnect)
        {
            StartCoroutine(TryReconnect());
        }
    }

    private IEnumerator TryReconnect()
    {
        Debug.Log($"等待 {reconnectDelay} 秒后尝试重连...");
        yield return new WaitForSeconds(reconnectDelay);

        if (!_isConnected && !_shouldStop)
        {
            Debug.Log("尝试重新连接...");
            ConnectToServer();

            // 重新启动缓冲区处理协程
            if (_bufferProcessingCoroutine != null)
            {
                StopCoroutine(_bufferProcessingCoroutine);
            }
            _bufferProcessingCoroutine = StartCoroutine(ProcessBufferCoroutine());
        }
    }
}