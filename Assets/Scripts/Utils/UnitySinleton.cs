using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 懒汉模式单例，无法在编译期确保唯一
public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour {
  private static T instance;
  private static object mutex = new object();

  public static T Instance {
    get {
      lock (mutex) {
        if (instance == null) {
          if (FindObjectsOfType(typeof(T)).Length != 0) {
            Debug.LogError("UnitySingleton should not create by hand!");
          }
          GameObject singleton = new();
          instance = singleton.AddComponent<T>();
          instance.name = "(singleton)" + typeof(T).ToString();
          DontDestroyOnLoad(singleton);
          Debug.Log("Singleton of " + typeof(T).ToString() + " created.");
        }
      }
      return instance;
    }
  }
}
