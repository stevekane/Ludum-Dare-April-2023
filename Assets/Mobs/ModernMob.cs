using System;
using UnityEngine;

[Serializable]
public struct HurtRing {
  public HurtType Type;
  public Timeval Duration;
}

public class ModernMob : MonoBehaviour {
  [SerializeField] MeshRenderer RingPrefab;
  [SerializeField] HurtRing[] HurtRings;
  [SerializeField] Transform RingContainer;

  MeshRenderer[] RingRenderers;

  void Awake() {
    RingRenderers = new MeshRenderer[HurtRings.Length];
    for (var i = 0; i < HurtRings.Length; i++)
      RingRenderers[i] = Instantiate(RingPrefab, RingContainer);
  }

  void LateUpdate() {
    var totalTicks = 0f;
    foreach (var ring in HurtRings)
      totalTicks += ring.Duration.Ticks;
    var outerRadius = 1f;
    for (var i = 0; i < HurtRings.Length; i++) {
      var ring = HurtRings[i];
      var ringRenderer = RingRenderers[i] ;
      var innerRadius = outerRadius-ring.Duration.Ticks/totalTicks;
      // duration / total determines the range
      // ringRenderer.material.SetFloat("_Opacity", 1);
      ringRenderer.material.SetFloat("_OuterRadius", outerRadius);
      ringRenderer.material.SetFloat("_InnerRadius", innerRadius);
      ringRenderer.material.SetColor("_Color", ring.Type switch {
        HurtType.Red => Color.red,
        HurtType.Green => Color.green,
        HurtType.Blue => Color.blue,
        _ => Color.black
      });
      outerRadius = innerRadius;
    }
  }
}
