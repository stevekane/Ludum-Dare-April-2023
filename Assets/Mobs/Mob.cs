using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HurtPair {
  public HurtType Left;
  public HurtType Right;
  public bool Split;
}

public class Mob : MonoBehaviour {
  public static int MaxTotalTicks = Timeval.FromSeconds(3f).Ticks;

  [SerializeField] AudioClip[] GoodImpactSounds;
  [SerializeField] AudioClip[] BadImpactSounds;
  [SerializeField] AudioClip[] DeathSounds;
  [SerializeField] AudioSource AudioSource;
  [SerializeField] float MinExplosiveForce = 40;
  [SerializeField] float MaxExplosiveForce = 80;
  [SerializeField] GameObject MainModel;
  [SerializeField] Rigidbody[] PartBodies;
  [SerializeField] public HurtPair[] HurtSequence;
  [SerializeField] MeshRenderer TargetMeshRenderer;
  [SerializeField] Color RegenColor;
  [SerializeField] float MinZ = 0f;

  public EventSource OnDeath { get; private set; } = new();

  int HurtBufferTicks = Timeval.FromMillis(200).Ticks;
  List<(HurtType, int)> HurtBuffer = new();

  public Timeval RegenDuration = Timeval.FromSeconds(1f);
  public int SequenceIdx = 0;
  public int RegenTicks = 0;
  public int RegeneratingRing => Mathf.Max(SequenceIdx-1, 0);

  int Score => HurtSequence.Sum(p => p.Split ? 400 : 100) * HurtSequence.Length;

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

  void OnHitKill() {
  }
  void OnHitGood() {
    AudioSource.PlayOneShot(GoodImpactSounds[UnityEngine.Random.Range(0, GoodImpactSounds.Length)]);
  }
  void OnHitBad() {
    AudioSource.PlayOneShot(BadImpactSounds[UnityEngine.Random.Range(0, BadImpactSounds.Length)]);
  }

  public void Explode() {
    var hurtTypes = new List<HurtType>();
    foreach(var pair in HurtSequence) {
      if (!hurtTypes.Contains(pair.Left))
        hurtTypes.Add(pair.Left);
      if (!hurtTypes.Contains(pair.Right))
        hurtTypes.Add(pair.Right);
    }
    foreach (var part in PartBodies) {
      var type = hurtTypes[UnityEngine.Random.Range(0, hurtTypes.Count)];
      var color = MobBuilder.Instance.ColorForType(type);
      part.GetComponent<MeshRenderer>().material.color = color;
      part.GetComponent<TrailRenderer>().enabled = true;
      part.GetComponent<TrailRenderer>().material.color = color;
    }
    CameraShaker.Instance.Shake(10);
    GameManager.Instance.OnGoal.Fire();
    GetComponent<MoveForward>().enabled = false;
    AudioSource.PlayOneShot(DeathSounds[UnityEngine.Random.Range(0, DeathSounds.Length)]);
    MainModel.SetActive(false);
    foreach (var part in PartBodies) {
      part.transform.localScale *= UnityEngine.Random.Range(.5f, 1f);
      part.GetComponent<TrailRenderer>().time = 1;
      part.isKinematic = false;
      part.AddForce(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(MinExplosiveForce, MaxExplosiveForce), ForceMode.Impulse);
    }
    OnDeath.Fire();
    Destroy(gameObject, 5);
  }

  public bool Dead = false;
  void Die() {
    if (Dead) return;
    Dead = true;
    var msg = WorldSpaceMessageManager.Instance.SpawnMessage($"{Score}", transform.position + new Vector3(0, 0, -5f));
    msg.LocalVelocity = (transform.position - Player.Instance.transform.position).normalized * 5f;
    Player.Instance.Score += Score;
    Explode();
  }

  void AttackPlayer() {
    if (Dead) return;
    Dead = true;
    Player.Instance.Score -= 100;
    Destroy(gameObject);
  }

  void FixedUpdate() {
    // Remove old hurtbuffer entries.
    var didRemove = HurtBuffer.RemoveAll(e => e.Item2 + HurtBufferTicks < Timeval.TickCount) > 0;
    var didHit = false;
    var didKill = false;
    while (ProcessHurt()) {
      didHit = true;
      RegenTicks = RegenDuration.Ticks;
      if (++SequenceIdx == HurtSequence.Length) {
        didKill = true;
        Die();
      }
    }
    if (didKill) {
      OnHitKill();
    } else if (didHit) {
      OnHitGood();
    } else if (didRemove) {
      OnHitBad();
    }

    if (transform.position.z <= MinZ)
      AttackPlayer();

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
      return (fraction * MobBuilder.Instance.ColorForType(HurtSequence[index].Left), fraction * MobBuilder.Instance.ColorForType(HurtSequence[index].Right), HurtSequence[index].Split);
    } else {
      return (Color.black, Color.black, false);
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