using UnityEngine;

namespace Pool
{
    public class SquarePool : Pool
    {
        private static SquarePool instance;
        public static SquarePool Instance
        {
            get
            {
                if (instance != null) return instance;
            
                instance = FindObjectOfType<SquarePool>();

                if (instance != null) return instance;
            
                GameObject newGo = new GameObject();
                instance = newGo.AddComponent<SquarePool>();
                return instance;
            }
        }

        protected void Awake()
        {
            instance = this as SquarePool;
        }
    }
}