using System;
using UnityEngine;

public class Mob : MonoBehaviour {
  [SerializeField] HurtType[] HurtSequence;
  [SerializeField] Timeval RegenDuration = Timeval.FromSeconds(1f);

  int SequenceIdx = 0;
  int RegenTicks = 0;

  public void OnHurt(HurtType type) {
    if (type != HurtSequence[SequenceIdx])
      return;
    RegenTicks = RegenDuration.Ticks;
    if (++SequenceIdx == HurtSequence.Length)
      Die();
  }

  void Die() {
    Destroy(gameObject, .01f);
  }

  void FixedUpdate() {
    if (SequenceIdx > 0) {
      if (--RegenTicks == 0)
        SequenceIdx--;
    }
  }
}