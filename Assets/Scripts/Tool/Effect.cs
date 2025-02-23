using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {
  private new ParticleSystem particleSystem;
  public Action onFinish;

  void Start() {
    particleSystem = GetComponent<ParticleSystem>();
    if (particleSystem != null) {
      particleSystem.Play();
    }
  }

  // Update is called once per frame
  void Update() {
    if (!particleSystem.isPlaying) {
      onFinish?.Invoke();
      Destroy(gameObject);
    }
  }
}
