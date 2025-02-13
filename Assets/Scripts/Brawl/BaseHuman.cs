using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BaseHuman : MonoBehaviour {
  private Vector3 targetPosition;
  protected bool isMoving = false;
  protected bool isAttacking = false;
  private float attackTime;
  private Animator animator;

  public float speed = 5f;
  public string desc;

  protected void Start() {
    animator = GetComponent<Animator>();
  }

  protected void Update() {
    MoveUpdate();
    AttackUpdate();
  }

  private void MoveUpdate() {
    if (!isMoving) {
      return;
    }
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    transform.LookAt(targetPosition);
    if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
      isMoving = false;
      animator.SetBool("isMoving", isMoving);
    }
  }

  public void MoveTo(Vector3 position) {
    targetPosition = position;
    isMoving = true;
    animator.SetBool("isMoving", isMoving);
  }

  public void Attack() {
    isAttacking = false;
    attackTime = Time.time;
    animator.SetTrigger("attack");
  }

  private void AttackUpdate() {
    if (!isAttacking) {
      return;
    }
    if (Time.time - attackTime > 1.2f) {
      return;
    }
    isAttacking = false;
  }
}
