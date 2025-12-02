using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameLogManager : MonoBehaviour
{
    [Header("日志系统设置")]
    public bool enableLogSystem = true;
    public int maxLogFiles = 5; // 最大日志文件数量
    public int maxFileSizeKB = 1024; // 单个文件最大大小(KB)
    public string serverURL = "https://yourserver.com/api/logs"; // 日志服务器地址

    private string logDirectory;
    private string currentLogFile;
    private bool hasErrorOccurred = false;
    private Queue<string> pendingLogs = new Queue<string>(); // 待发送的日志队列

    public static GameLogManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLogSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeLogSystem()
    {
        if (!enableLogSystem) return;

        // 创建日志目录
        logDirectory = Path.Combine(Application.persistentDataPath, "GameLogs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // 设置当前日志文件
        currentLogFile = Path.Combine(logDirectory, $"game_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        // 注册日志回调
        Application.logMessageReceived += HandleLog;

        // 启动时清理旧日志
        CleanupOldLogs();

        // 记录系统启动信息
        LogSystemInfo();
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string formattedLog = $"[{timestamp}] [{type}] {logString}";

        if (!string.IsNullOrEmpty(stackTrace) && type == LogType.Exception)
        {
            formattedLog += $"\nStack Trace:\n{stackTrace}";
        }

        // 写入文件
        WriteLogToFile(formattedLog);

        // 如果是错误或异常，添加到待发送队列并尝试发送
        if (type == LogType.Error)
        {
            hasErrorOccurred = true;
            pendingLogs.Enqueue(formattedLog);

            // 尝试发送错误日志
            StartCoroutine(TrySendErrorLogs());
        }

        // 检查文件大小
        CheckFileSize();
    }

    void WriteLogToFile(string logContent)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(currentLogFile, true))
            {
                writer.WriteLine(logContent);
                writer.WriteLine("---"); // 分隔符
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"写入日志文件失败: {e.Message}");
        }
    }

    void CheckFileSize()
    {
        try
        {
            FileInfo fileInfo = new FileInfo(currentLogFile);
            if (fileInfo.Exists && fileInfo.Length > maxFileSizeKB * 1024)
            {
                // 文件过大，创建新文件
                currentLogFile = Path.Combine(logDirectory, $"game_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                WriteLogToFile($"--- 创建新的日志文件(原文件过大) ---");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"检查日志文件大小失败: {e.Message}");
        }
    }

    void CleanupOldLogs()
    {
        try
        {
            DirectoryInfo dirInfo = new DirectoryInfo(logDirectory);
            FileInfo[] logFiles = dirInfo.GetFiles("game_log_*.txt");

            if (logFiles.Length > maxLogFiles)
            {
                // 按创建时间排序，删除最旧的文件
                Array.Sort(logFiles, (x, y) => x.CreationTime.CompareTo(y.CreationTime));

                for (int i = 0; i < logFiles.Length - maxLogFiles; i++)
                {
                    File.Delete(logFiles[i].FullName);
                    Debug.Log($"删除旧日志文件: {logFiles[i].Name}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"清理旧日志失败: {e.Message}");
        }
    }

    void LogSystemInfo()
    {
        string systemInfo = $@"
=== 系统信息 ===
游戏版本: {Application.version}
系统: {SystemInfo.operatingSystem}
设备: {SystemInfo.deviceName}
处理器: {SystemInfo.processorType}
内存: {SystemInfo.systemMemorySize}MB
显卡: {SystemInfo.graphicsDeviceName}
Unity版本: {Application.unityVersion}
启动时间: {DateTime.Now}
================
";
        WriteLogToFile(systemInfo);
    }

    private IEnumerator TrySendErrorLogs()
    {
        if (pendingLogs.Count == 0) yield break;

        // 等待一帧，确保所有错误日志都被捕获
        yield return new WaitForEndOfFrame();

        List<string> logsToSend = new List<string>();
        while (pendingLogs.Count > 0)
        {
            logsToSend.Add(pendingLogs.Dequeue());
        }

        // 添加当前日志文件内容
        string recentLogs = GetRecentLogs();
        if (!string.IsNullOrEmpty(recentLogs))
        {
            logsToSend.Add("=== 最近日志 ===");
            logsToSend.Add(recentLogs);
        }

        // 发送到服务器
        LogSender sender = gameObject.AddComponent<LogSender>();
        sender.SendLogsToServer(logsToSend);
    }

    private string GetRecentLogs(int maxLines = 100)
    {
        try
        {
            if (!File.Exists(currentLogFile)) return string.Empty;

            string[] allLines = File.ReadAllLines(currentLogFile);
            int startIndex = Mathf.Max(0, allLines.Length - maxLines);
            int length = Mathf.Min(maxLines, allLines.Length - startIndex);

            string[] recentLines = new string[length];
            Array.Copy(allLines, startIndex, recentLines, 0, length);

            return string.Join("\n", recentLines);
        }
        catch
        {
            return "无法读取日志文件";
        }
    }

    public void ClearSentLogs()
    {
        pendingLogs.Clear();
    }

    // 手动触发日志发送（可用于测试或特定情况）
    public void ManuallySendLogs()
    {
        StartCoroutine(TrySendErrorLogs());
    }

    // 获取所有日志文件列表
    public List<string> GetLogFiles()
    {
        List<string> logFiles = new List<string>();

        try
        {
            DirectoryInfo dirInfo = new DirectoryInfo(logDirectory);
            FileInfo[] files = dirInfo.GetFiles("game_log_*.txt");

            foreach (FileInfo file in files)
            {
                logFiles.Add(file.FullName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"获取日志文件列表失败: {e.Message}");
        }

        return logFiles;
    }

    void OnApplicationQuit()
    {
        // 游戏退出时尝试发送未发送的日志
        if (hasErrorOccurred && pendingLogs.Count > 0)
        {
            StartCoroutine(TrySendErrorLogs());
        }

        // 取消注册回调
        if (enableLogSystem)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}

public class LogSender : MonoBehaviour
{
    private class LogData
    {
        public string deviceId;
        public string gameVersion;
        public List<string> logs = new List<string>();
        public string systemInfo;
    }

    public void SendLogsToServer(List<string> logContents)
    {
        StartCoroutine(SendLogsCoroutine(logContents));
    }

    private IEnumerator SendLogsCoroutine(List<string> logContents)
    {
        LogData logData = new LogData
        {
            deviceId = SystemInfo.deviceUniqueIdentifier,
            gameVersion = Application.version,
            logs = logContents,
            systemInfo = GetSystemInfo()
        };

        string jsonData = JsonUtility.ToJson(logData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(GameLogManager.Instance.serverURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 设置超时时间
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("日志发送成功");
                // 清空已发送的日志
                GameLogManager.Instance.ClearSentLogs();
            }
            else
            {
                Debug.LogWarning($"日志发送失败: {request.error}");
            }
        }
    }

    private string GetSystemInfo()
    {
        return $"OS: {SystemInfo.operatingSystem}, Device: {SystemInfo.deviceModel}, " +
               $"Memory: {SystemInfo.systemMemorySize}MB, GPU: {SystemInfo.graphicsDeviceName}";
    }
}
