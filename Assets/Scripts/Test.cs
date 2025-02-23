using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
  void Start() {
    GameObject tank = new GameObject("tank");
    tank.AddComponent<ControlTank>().init("Tank");
    tank.AddComponent<CameraControl>();

    GameObject tank2 = new GameObject("tank");
    tank2.transform.position = new Vector3(5, 0, 0);
    tank2.AddComponent<BaseTank>().init("Tank");
  }
}
