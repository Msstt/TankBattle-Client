using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
  public static GameObject Load(string path) {
    GameObject res = Resources.Load<GameObject>(path);
    if (res == null) {
      Debug.Log("Prefabs " + path + " is missing!");
    }
    return res;
  }
}
