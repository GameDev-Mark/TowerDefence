using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance = null;
    private static readonly object padLock = new object();

    public static T Instance
    {
        get
        {
            lock (padLock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject _newObj = new GameObject();
                        _newObj.name = typeof(T).Name;
                        instance = _newObj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}