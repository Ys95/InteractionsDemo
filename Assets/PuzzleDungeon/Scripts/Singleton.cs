using System;
using UnityEngine;

namespace Tide
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError($"{typeof(T)} is not on scene!");
                        return null;
                    }
                }
                return _instance;
            }
        }
        
        private static T _instance;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"Singleton {typeof(T)} has multiple instances!");
                Destroy(_instance);
            }
            
            _instance = this as T;
        }
        
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}