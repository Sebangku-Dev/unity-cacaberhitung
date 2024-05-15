using UnityEngine;

/// <summary>
/// Singleton -> Single-only static object but not persistent
/// SingletonPersistent -> Single-only, static, and persistent accross scenes
/// </summary>

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        Instance = this as T;
    }

    protected void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
        
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
