using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly ConcurrentQueue<System.Action> _actions = new ConcurrentQueue<System.Action>();

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Update()
    {
        while (_actions.TryDequeue(out System.Action action))
        {
            action?.Invoke();
        }
    }

    public void Enqueue(System.Action action)
    {
        _actions.Enqueue(action);
    }
}