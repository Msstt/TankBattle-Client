using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour {
  private readonly float speed = 30f;
  public BaseTank tank;

  public void Init(string prefabsPath) {
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

    Instantiate(ResourceManager.Load("Explosion"), transform.position, transform.rotation);
    Destroy(gameObject);

    if (otherTank == null) {
      return;
    }
    if (tank.id == GameMain.ID) {
      NetManager.Send(new MsgHit() {
        ID = otherTank.id
      });
    }
  }
}
