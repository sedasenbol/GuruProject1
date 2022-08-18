using UnityEngine;

namespace Pool
{
    public class CrossPool : Pool
    {
        private static CrossPool instance;
        public static CrossPool Instance
        {
            get
            {
                if (instance != null) return instance;
            
                instance = FindObjectOfType<CrossPool>();

                if (instance != null) return instance;
            
                GameObject newGo = new GameObject();
                instance = newGo.AddComponent<CrossPool>();
                return instance;
            }
        }

        protected void Awake()
        {
            instance = this as CrossPool;
        }
    }
}