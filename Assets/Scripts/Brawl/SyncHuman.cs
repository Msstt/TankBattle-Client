using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncHuman : BaseHuman {
  new void Start() {
    base.Start();
  }

  new void Update() {
    base.Update();
  }

  public void Attack(float direction) {
    transform.eulerAngles = new Vector3(0, direction, 0);
    Attack();
  }
}
