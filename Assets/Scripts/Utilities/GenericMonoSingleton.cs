using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMonoSingleton<T> : MonoBehaviour where T : GenericMonoSingleton<T>
{
    // Singleton
    public static T Instance {get{ return instance;}}
    private static T instance;

     private void Awake() 
        {
            if (instance == null)
            {
                instance = (T)this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogError("Singleton of" + (T)this +  " called 2nd Service and denied");
            }
        }
}
