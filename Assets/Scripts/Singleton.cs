using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            
            instance = FindObjectOfType<T>();

            if (instance != null) return instance;
            
            GameObject newGo = new GameObject();
            instance = newGo.AddComponent<T>();
            return instance;
        }
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }
}