using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour {
  private float speed = 30f;
  public BaseTank tank;

  public void init(string prefabsPath) {
    GameObject prefabs = ResourceManager.Load(prefabsPath);
    GameObject skin = Instantiate(prefabs, transform);
    skin.transform.localPosition = Vector3.zero;
    skin.transform.localEulerAngles = Vector3.zero;

    Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
    rigidbody.useGravity = false;
    BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
    boxCollider.center = new Vector3(0, 0, 0.2f);
    boxCollider.size = new Vector3(0.2f, 0.2f, 0.5f);
    boxCollider.isTrigger = true;
  }

  void Update() {
    transform.position += speed * Time.deltaTime * transform.forward;
  }

  void OnTriggerEnter(Collider other) {
    BaseTank otherTank = other.gameObject.GetComponent<BaseTank>();
    if (otherTank == tank) {
      return;
    }
    if (otherTank != null) {
      otherTank.Attack(35);
    }
    Instantiate(ResourceManager.Load("Explosion"), transform.position, transform.rotation);
    Destroy(gameObject);
  }
}
