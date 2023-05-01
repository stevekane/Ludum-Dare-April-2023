using UnityEngine;

public class Mob : MonoBehaviour {
  [SerializeField] HurtType[] HurtSequence;
  [SerializeField] Timeval RegenDuration = Timeval.FromSeconds(1f);
  [SerializeField] MeshRenderer TargetMeshRenderer;
  [SerializeField, ColorUsage(true, true)] Color RedColor;
  [SerializeField, ColorUsage(true, true)] Color GreenColor;
  [SerializeField, ColorUsage(true, true)] Color BlueColor;
  [SerializeField] Color RegenColor;

  public EventSource OnDeath { get; private set; } = new();

  public int SequenceIdx = 0;
  public int RegenTicks = 0;
  public int RegeneratingRing => Mathf.Max(SequenceIdx-1, 0);

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
    if (--RegenTicks <= 0) {
      if (RegeneratingRing > 0) {
        SequenceIdx--;
        RegenTicks = RegenDuration.Ticks;
      } else {
        SequenceIdx = 0;
        RegenTicks = 0;
      }
    }
  }

  void LateUpdate() {
    var outerRadius = 1f;
    for (var i = 0; i < 3; i++) {
      var innerRadius = outerRadius-(float)RegenDuration.Ticks/(float)MaxTotalTicks;
      var outerFillRadius = OuterRadiusForRing(i, outerRadius, innerRadius);
      var color = ColorForRing(i);
      TargetMeshRenderer.material.SetFloat($"_OuterRadius{i}", outerFillRadius);
      TargetMeshRenderer.material.SetFloat($"_InnerRadius{i}", innerRadius);
      TargetMeshRenderer.material.SetColor($"_Color{i}0", color);
      TargetMeshRenderer.material.SetColor($"_Color{i}1", color);
      outerRadius = innerRadius;
    }
  }

  Color ColorForRing(int index) {
    if (index == RegeneratingRing && RegenTicks > 0) {
      return RegenColor;
    } else if (index < HurtSequence.Length && index >= SequenceIdx) {
      var fraction = (float)(RegenDuration.Ticks - RegenTicks) / RegenDuration.Ticks;
      return fraction * HurtSequence[index] switch {
        HurtType.Red => RedColor,
        HurtType.Green => GreenColor,
        HurtType.Blue => BlueColor,
        _ => Color.black
      };
    } else {
      return Color.black;
    }
  }

  float OuterRadiusForRing(int index, float outerRadius, float innerRadius) {
    if (index == RegeneratingRing) {
      var thickness = outerRadius - innerRadius;
      var fraction = (float)(RegenDuration.Ticks - RegenTicks) / RegenDuration.Ticks;
      return innerRadius + thickness * fraction;
    } else {
      return outerRadius;
    }
  }
}