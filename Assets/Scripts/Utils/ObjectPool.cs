using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 单例对象池，需确保预制体名称全局唯一
public class ObjectPool : UnitySingleton<ObjectPool> {
  private Dictionary<string, List<GameObject>> pool = new();

  private void CheckPrefabs(GameObject prefabs) {
    if (!pool.ContainsKey(prefabs.name)) {
      pool.Add(prefabs.name, new());
    }
  }

  private GameObject CreatePrefabs(GameObject prefabs) {
    GameObject gameObject = Instantiate(prefabs);
    gameObject.SetActive(false);
    pool[prefabs.name].Add(gameObject);
    return gameObject;
  }

  public void Register(GameObject prefabs, int number) {
    CheckPrefabs(prefabs);
    for (int i = pool[prefabs.name].Count; i < number; i++) {
      CreatePrefabs(prefabs);
    }
  }

  public GameObject Create(GameObject prefabs, Vector3 position, Quaternion rotation) {
    CheckPrefabs(prefabs);
    GameObject gameObject = pool[prefabs.name].Find(gameObject => !gameObject.activeInHierarchy);
    if (gameObject == null) {
      gameObject = CreatePrefabs(prefabs);
    }
    gameObject.transform.SetPositionAndRotation(position, rotation);
    gameObject.SetActive(true);
    return gameObject;
  }

  public void Destroy(GameObject gameObject) {
    gameObject.SetActive(false);
  }

  public void Destroy(GameObject gameObject, float time) {
    StartCoroutine(DestroyCoroutine(gameObject, time));
  }

  IEnumerator DestroyCoroutine(GameObject gameObject, float time) {
    yield return new WaitForSeconds(time);
    Destroy(gameObject);
  }

  private void Clear(string prefabsName) {
    for (int i = 0; i < pool[prefabsName].Count; i++) {
      Destroy(pool[prefabsName][i]);
    }
    pool.Remove(prefabsName);
  }

  public void Clear(GameObject prefabs) {
    if (!pool.ContainsKey(prefabs.name)) {
      return;
    }
    Clear(prefabs.name);
  }

  public void ClearAll() {
    foreach (string prefabsName in pool.Keys) {
      Clear(prefabsName);
    }
  }
}
