using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTank : BaseTank {
  public class Status {
    public Vector3 position;
    public Vector3 rotation;
    public float time;
  }
  private readonly Status[] status = new Status[2];

  public override void Init() {
    base.Init();
    if (TryGetComponent<Rigidbody>(out var rigidbody)) {
      rigidbody.useGravity = false;
      rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
    status[0] = status[1] = new Status() {
      position = transform.position,
      rotation = transform.eulerAngles,
      time = Time.time,
    };
  }

  new void Update() {
    base.Update();
    MoveUpdate();
  }

  private void MoveUpdate() {
    float t = (Time.time - status[0].time) / status[1].time;
    t = Mathf.Clamp(t, 0, 2);
    transform.position = Lerp(status[0].position, status[1].position, t);
    transform.eulerAngles = Lerp(status[0].rotation, status[1].rotation, t);
  }

  private Vector3 Lerp(Vector3 a, Vector3 b, float t) {
    return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
  }

  public void SyncPosition(MsgSyncTank msg) {
    status[0] = status[1];
    status[1] = new Status() {
      position = new Vector3(msg.X, msg.Y, msg.Z),
      rotation = new Vector3(msg.EX, msg.EY, msg.EZ),
      time = msg.Time
    };

    Vector3 angle = turret.localEulerAngles;
    angle.y = msg.TurretY;
    turret.localEulerAngles = angle;
  }

  public void SyncFire(MsgFire msg) {
    GameObject bullet = Fire();
    bullet.transform.position = new Vector3(msg.X, msg.Y, msg.Z);
    bullet.transform.eulerAngles = new Vector3(msg.EX, msg.EY, msg.EZ);
  }
}
