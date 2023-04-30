using System;
using UnityEngine;

public class Mob : MonoBehaviour {
  [SerializeField] HurtType[] HurtSequence;

  Hurtbox Hurtbox;
  int SequenceIdx = 0;

  void Start() {
    Hurtbox = GetComponentInChildren<Hurtbox>();
    Hurtbox.Owner = this;
  }

  public void OnHurt(HurtType type) {
    if (type != HurtSequence[SequenceIdx])
      return;
    if (++SequenceIdx == HurtSequence.Length)
      Die();
  }

  void Die() {
    Destroy(gameObject, .01f);
  }
}