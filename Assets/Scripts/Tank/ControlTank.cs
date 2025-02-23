using System.Collections;
using System.Collections.Generic;
using Codice.CM.Client.Differences;
using UnityEngine;

public class ControlTank : BaseTank {
  private float fireCD = 0.5f;
  private float lastFireTime = 0;

  new void Update() {
    base.Update();
    MoveUpdate();
    TurretUpdate();
    FireUpdate();
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
    Fire();
    lastFireTime = Time.time;
  }
}
