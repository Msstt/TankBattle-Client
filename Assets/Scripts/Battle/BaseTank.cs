using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BaseTank : MonoBehaviour {
  public string id = "";

  protected float steer = 30;
  protected float speed = 3;
  protected float turretSpeed = 10;

  protected Transform turret;
  protected Transform firePoint;

  public float Health { get; private set; } = 100;

  public int camp = 0;

  public virtual void Init() {
    GameObject prefabs;
    if (camp == 0) {
      prefabs = ResourceManager.Load("BlueTank");
    } else {
      prefabs = ResourceManager.Load("GreenTank");
    }
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

  protected GameObject Fire() {
    GameObject bulletObject = new();
    bulletObject.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
    Bullet bullet = bulletObject.AddComponent<Bullet>();
    bullet.Init("Bullet");
    bullet.tank = this;
    return bulletObject;
  }

  public void Attack(float damage) {
    if (IsDie()) {
      return;
    }
    Health -= damage;
    if (IsDie()) {
      GameObject effect = Instantiate(ResourceManager.Load("Destroy"), transform.position, transform.rotation);
      BattleManager.effects.Add(effect);
    }
  }

  protected bool IsDie() {
    return Health <= 0;
  }
}
