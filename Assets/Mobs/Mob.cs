using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HurtPair {
  public HurtType Left;
  public HurtType Right;
  public bool Split;
}

public class Mob : MonoBehaviour {
  [SerializeField] HurtPair[] HurtSequence;
  [SerializeField] Timeval RegenDuration = Timeval.FromSeconds(1f);
  [SerializeField] MeshRenderer TargetMeshRenderer;
  [SerializeField, ColorUsage(true, true)] Color RedColor;
  [SerializeField, ColorUsage(true, true)] Color GreenColor;
  [SerializeField, ColorUsage(true, true)] Color BlueColor;
  [SerializeField] Color RegenColor;

  public EventSource OnDeath { get; private set; } = new();

  int HurtBufferTicks = Timeval.FromMillis(200).Ticks;
  List<(HurtType, int)> HurtBuffer = new();

  public int SequenceIdx = 0;
  public int RegenTicks = 0;
  public int RegeneratingRing => Mathf.Max(SequenceIdx-1, 0);

  static int MaxTotalTicks = Timeval.FromSeconds(3f).Ticks;

  public void OnHurt(HurtType type) {
    HurtBuffer.Add((type, Timeval.TickCount));
  }

  bool ProcessHurt() {
    if (SequenceIdx >= HurtSequence.Length) return false;
    var left = HurtBuffer.FindIndex(e => e.Item1 == HurtSequence[SequenceIdx].Left);
    var right = HurtBuffer.FindLastIndex(e => e.Item1 == HurtSequence[SequenceIdx].Right);
    if (left < 0 || right < 0) return false;
    if (HurtSequence[SequenceIdx].Split && left == right) return false;

    // Remove the higher index first to avoid changing order.
    HurtBuffer.RemoveAt(Mathf.Max(left, right));
    if (left != right) // Will be the same for a same-sequence.
      HurtBuffer.RemoveAt(Mathf.Min(left, right));
    return true;
  }

  void Die() {
    OnDeath.Fire();
    Destroy(gameObject, .01f);
  }

  void FixedUpdate() {
    // Remove old hurtbuffer entries.
    HurtBuffer.RemoveAll(e => e.Item2 + HurtBufferTicks < Timeval.TickCount);
    while (ProcessHurt()) {
      RegenTicks = RegenDuration.Ticks;
      if (++SequenceIdx == HurtSequence.Length)
        Die();
    }

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
    TargetMeshRenderer.material.SetFloat($"_BoundThickness", .05f);
    for (var i = 0; i < 3; i++) {
      var innerRadius = outerRadius-(float)RegenDuration.Ticks/(float)MaxTotalTicks;
      var outerFillRadius = OuterRadiusForRing(i, outerRadius, innerRadius);
      var colors = ColorsForRing(i);
      TargetMeshRenderer.material.SetFloat($"_OuterRadius{i}", outerFillRadius);
      TargetMeshRenderer.material.SetFloat($"_InnerRadius{i}", innerRadius);
      TargetMeshRenderer.material.SetColor($"_Color{i}0", colors.Item1);
      TargetMeshRenderer.material.SetColor($"_Color{i}1", colors.Item2);
      TargetMeshRenderer.material.SetFloat($"_SplitThickness{i}", colors.Item3 ? .05f : 0f);
      outerRadius = innerRadius;
    }
  }

  (Color, Color, bool) ColorsForRing(int index) {
    if (index == RegeneratingRing && RegenTicks > 0) {
      return (RegenColor, RegenColor, false);
    } else if (index < HurtSequence.Length && index >= SequenceIdx) {
      var fraction = (float)(RegenDuration.Ticks - RegenTicks) / RegenDuration.Ticks;
      return (fraction * ColorForType(HurtSequence[index].Left), fraction * ColorForType(HurtSequence[index].Right), HurtSequence[index].Split);
    } else {
      return (Color.black, Color.black, false);
    }
  }

  Color ColorForType(HurtType type) => type switch {
    HurtType.Red => RedColor,
    HurtType.Green => GreenColor,
    HurtType.Blue => BlueColor,
    _ => Color.black
  };

  float OuterRadiusForRing(int index, float outerRadius, float innerRadius) {
    if (index == RegeneratingRing) {
      var thickness = outerRadius - innerRadius;
      var fraction = (float)(RegenDuration.Ticks - RegenTicks) / RegenDuration.Ticks;
      return innerRadius + thickness * fraction;
    } else {
      return outerRadius;
    }
  }

  TaskScope Scope = new();
  [ContextMenu("RB")]
  async void Test() {
    OnHurt(HurtType.Red);
    await Scope.Ticks(5);
    OnHurt(HurtType.Blue);
  }

  [ContextMenu("RB, G")]
  async void Test2() {
    OnHurt(HurtType.Red);
    await Scope.Ticks(5);
    OnHurt(HurtType.Blue);
    await Scope.Ticks(30);
    OnHurt(HurtType.Green);
  }
}