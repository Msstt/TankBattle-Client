using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
  private Camera camera;
  private Vector3 initDistance = new(0, 6, -12);
  public Vector3 distance = new(0, 5, -10);
  private Vector3 offset = new(1, 0, 0);
  private float speed = 3f;

  void Start() {
    camera = Camera.main;
    camera.transform.position = transform.position
      + initDistance.z * transform.forward + initDistance.y * Vector3.up;
  }

  void LateUpdate() {
    Vector3 target = transform.position
      + distance.z * transform.forward + distance.y * Vector3.up;
    camera.transform.position = Vector3.MoveTowards(camera.transform.position, target, speed * Time.deltaTime);
    camera.transform.LookAt(transform.position + offset);
  }
}
