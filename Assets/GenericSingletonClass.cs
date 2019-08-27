using UnityEngine;


    public class GenericSingletonClass<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<T> ();
                    if (instance == null) {
                        GameObject obj = new GameObject {name = typeof(T).Name};
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
 
        public void Awake ()
        {
            if (instance == null) {
                instance = this as T;
                //DontDestroyOnLoad (this.gameObject);
            } else {
                Destroy (gameObject);
            }
            SingletonAwake();
        }

        protected virtual void SingletonAwake()
        {
        
        }
    }
