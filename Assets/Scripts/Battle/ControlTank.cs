using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTank : BaseTank {
  private static readonly float fireCD = 0.5f;
  private float lastFireTime = 0;

  private float lastSyncTime = 0;
  private static readonly float syncInterval = 0.03f;

  public override void Init() {
    base.Init();
    turret.gameObject.AddComponent<CameraControl>();
    GameObject crosshair = ResourceManager.Load("Crosshair");
    Instantiate(crosshair, turret.transform);
  }

  new void Update() {
    base.Update();
    SyncUpdate();
    if (IsDie()) {
      return;
    }
    MoveUpdate();
    TurretUpdate();
    FireUpdate();
  }

  private void SyncUpdate() {
    if (Time.time - lastSyncTime < syncInterval) {
      return;
    }
    lastSyncTime = Time.time;
    NetManager.Send(new MsgSyncTank() {
      Time = Time.time,
      X = transform.position.x,
      Y = transform.position.y,
      Z = transform.position.z,
      EX = transform.eulerAngles.x,
      EY = transform.eulerAngles.y,
      EZ = transform.eulerAngles.z,
      TurretY = turret.localEulerAngles.y
    });
  }

  private void MoveUpdate() {
    transform.Rotate(0, Input.GetAxis("Horizontal") * steer * Time.deltaTime, 0);
    transform.position += Input.GetAxis("Vertical") * speed * Time.deltaTime * transform.forward;
  }

  private void TurretUpdate() {
    float axis = 0f;
    if (Input.GetKey(KeyCode.Q)) {
      axis += -1;
    }
    if (Input.GetKey(KeyCode.E)) {
      axis += 1;
    }
    Vector3 angles = turret.transform.localEulerAngles;
    angles.y += axis * Time.deltaTime * turretSpeed;
    turret.transform.localEulerAngles = angles;
  }

  private void FireUpdate() {
    if (!Input.GetKey(KeyCode.Space)) {
      return;
    }
    if (Time.time - lastFireTime < fireCD) {
      return;
    }
    lastFireTime = Time.time;
    GameObject bullet = Fire();
    NetManager.Send(new MsgFire() {
      X = bullet.transform.position.x,
      Y = bullet.transform.position.y,
      Z = bullet.transform.position.z,
      EX = bullet.transform.eulerAngles.x,
      EY = bullet.transform.eulerAngles.y,
      EZ = bullet.transform.eulerAngles.z
    });
  }
}
