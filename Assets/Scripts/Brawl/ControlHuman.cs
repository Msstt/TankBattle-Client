using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ControlHuman : BaseHuman {
  new void Start() {
    base.Start();
  }

  new void Update() {
    base.Update();

    MoveUpdate();
    AttackUpdate();
  }

  private void MoveUpdate() {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
        if (hitInfo.collider.CompareTag("Terrain")) {
          MoveTo(hitInfo.point);
          BrawlNetManager.Send("Move", new string[] {
            BrawlNetManager.GetDesc(),
            hitInfo.point.x.ToString(),
            hitInfo.point.y.ToString(),
            hitInfo.point.z.ToString()
          });
        }
      }
    }
  }

  private void AttackUpdate() {
    if (Input.GetMouseButtonDown(1) && !isMoving && !isAttacking) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (!Physics.Raycast(ray, out RaycastHit hitInfo)) {
        return;
      }
      if (!hitInfo.collider.CompareTag("Terrain")) {
        return;
      }
      transform.LookAt(hitInfo.point);
      Attack();
      BrawlNetManager.Send("Attack", new string[] {
        BrawlNetManager.GetDesc(),
        transform.eulerAngles.y.ToString()
      });

      StartCoroutine(AttackCheck());
    }
  }

  private IEnumerator AttackCheck() {
    yield return new WaitForSeconds(0.1f);

    Vector3 end = transform.position + 0.5f * Vector3.up;
    Vector3 start = end + 20 * transform.forward;
    Debug.DrawLine(start, end, Color.red, 5f);
    if (!Physics.Linecast(start, end, out var hitInfo)) {
      yield break;
    }
    if (hitInfo.collider.gameObject == gameObject) {
      yield break;
    }
    if (!hitInfo.collider.gameObject.TryGetComponent<SyncHuman>(out var syncHuman)) {
      yield break;
    }
    BrawlNetManager.Send("Hit", new string[] {
      BrawlNetManager.GetDesc(),
      syncHuman.desc
    });
  }
}
