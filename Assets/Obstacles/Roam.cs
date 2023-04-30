using System;
using System.Linq;
using UnityEngine;

public class Roam : MonoBehaviour {
  public enum ModeType {
    // 0 to N-1 forward, N to 2N-2 backwards, back to 0.
    BackAndForth,
    // 0 to N-1 forward, back to 0.
    Loop,
  }

  [SerializeField] Transform[] Waypoints;
  [SerializeField] float Speed;
  [SerializeField] ModeType Mode;
  int SegmentIndex;

  void Start() {
    var closest = Waypoints.Aggregate((bestT, t) => Vector3.Distance(t.position, transform.position) < Vector3.Distance(bestT.position, transform.position) ? t : bestT);
    SegmentIndex = Array.IndexOf(Waypoints, closest);
  }

  int NumSegments => Mode switch {
    ModeType.Loop => Waypoints.Length,
    ModeType.BackAndForth => 2*Waypoints.Length - 2,
    _ => 1
  };
  int WaypointIdx(int seg) => Mode switch {
    ModeType.Loop => seg % Waypoints.Length,
    ModeType.BackAndForth => seg >= Waypoints.Length ? NumSegments - seg : seg,
    _ => seg
  };
  Transform At(int idx) => Waypoints[WaypointIdx(idx)];
  void FixedUpdate() {
    //var from = At(SegmentIndex);
    var to = At(SegmentIndex + 1);
    var dir = (to.position - transform.position).normalized;
    var dx = Time.fixedDeltaTime * Speed;
    transform.position += dx * dir;
    if (Vector3.Distance(transform.position, to.position) < dx)
      SegmentIndex = (SegmentIndex+1) % NumSegments;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    foreach (var t in Waypoints) {
      Gizmos.DrawWireSphere(t.position, .15f);
    }
  }
}