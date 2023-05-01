using UnityEngine;

public class Mob : MonoBehaviour {
  [SerializeField] HurtType[] HurtSequence;
  [SerializeField] Timeval RegenDuration = Timeval.FromSeconds(1f);
  [SerializeField] MeshRenderer RingPrefab;
  [SerializeField] Transform RingContainer;

  public EventSource OnDeath { get; private set; } = new();

  MeshRenderer[] RingRenderers;
  int SequenceIdx = 0;
  int RegenTicks = 0;

  static int MaxTotalTicks = Timeval.FromSeconds(3f).Ticks;

  public void OnHurt(HurtType type) {
    if (type != HurtSequence[SequenceIdx])
      return;
    RegenTicks = RegenDuration.Ticks;
    if (++SequenceIdx == HurtSequence.Length)
      Die();
  }

  void Die() {
    OnDeath.Fire();
    Destroy(gameObject, .01f);
  }

  void FixedUpdate() {
    if (SequenceIdx > 0) {
      if (--RegenTicks <= 0) {
        SequenceIdx--;
        RegenTicks = RegenDuration.Ticks;
      }
    }
  }

  void Awake() {
    RingRenderers = new MeshRenderer[HurtSequence.Length];
    for (var i = 0; i < HurtSequence.Length; i++)
      RingRenderers[i] = Instantiate(RingPrefab, RingContainer);
  }

  void LateUpdate() {
    var outerRadius = 1f;
    for (var i = 0; i < HurtSequence.Length; i++) {
      var ring = HurtSequence[i];
      var ringRenderer = RingRenderers[i];
      var innerRadius = outerRadius-(float)RegenDuration.Ticks/MaxTotalTicks;
      // duration / total determines the range
      ringRenderer.material.SetFloat("_Opacity", SequenceIdx > i ? 0f : 1f);
      ringRenderer.material.SetFloat("_OuterRadius", outerRadius);
      ringRenderer.material.SetFloat("_InnerRadius", innerRadius);
      ringRenderer.material.SetColor("_Color", ring switch {
        HurtType.Red => Color.red,
        HurtType.Green => Color.green,
        HurtType.Blue => Color.blue,
        _ => Color.black
      });
      outerRadius = innerRadius;
    }
  }
}