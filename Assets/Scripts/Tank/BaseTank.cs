using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public class BaseTank : MonoBehaviour {
  protected float steer = 30;
  protected float speed = 3;
  protected float turretSpeed = 10;

  protected Transform turret;
  protected Transform firePoint;

  private float health = 100;

  public void init(string prefabsPath) {
    GameObject prefabs = ResourceManager.Load(prefabsPath);
    GameObject skin = Instantiate(prefabs, transform);
    skin.transform.localPosition = Vector3.zero;
    skin.transform.localEulerAngles = Vector3.zero;

    gameObject.AddComponent<Rigidbody>();
    BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
    boxCollider.center = new Vector3(0, 1, 0);
    boxCollider.size = new Vector3(2, 2, 2);

    turret = skin.transform.Find("Turret");
    firePoint = turret.Find("FirePoint");
  }

  protected void Update() {
  }

  protected void Fire() {
    GameObject bulletObject = new();
    bulletObject.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
    Bullet bullet = bulletObject.AddComponent<Bullet>();
    bullet.init("Bullet");
    bullet.tank = this;
  }

  public void Attack(float damage) {
    if (IsDie()) {
      return;
    }
    health -= damage;
    if (IsDie()) {
      GameObject destroy = Instantiate(ResourceManager.Load("Destroy"), transform.position, transform.rotation);
      destroy.GetComponent<Effect>().onFinish += () => Destroy(gameObject);
    }
  }

  protected bool IsDie() {
    return health <= 0;
  }
}
