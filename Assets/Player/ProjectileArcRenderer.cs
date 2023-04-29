using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileArcRenderer : MonoBehaviour {
  [SerializeField] GameObject ContactPrefab;
  [SerializeField] LineRenderer LineRenderer;
  [SerializeField] LayerMask LayerMask;

  Vector3[] positions;
  GameObject ContactIndicator;

  void Start() {
    positions = new Vector3[LineRenderer.positionCount];
    ContactIndicator = Instantiate(ContactPrefab);
    ContactIndicator.SetActive(false);
  }

  void OnDestroy() {
    Destroy(ContactIndicator);
  }

  public void Render(Vector3 position, Vector3 velocity) {
    bool didHit = false;
    Vector3 hitStart = default;
    Vector3 hitPoint = default;
    for (var i = 0; i < positions.Length; i++) {
      positions[i] = position;
      velocity += Time.fixedDeltaTime * Physics.gravity;
      position += Time.fixedDeltaTime * velocity;
      var delta = position - positions[i];
      var hit = Physics.Raycast(positions[i], delta.normalized, out var rayHit, delta.magnitude, LayerMask);
      if (!didHit && hit) {
        var toHitPoint = rayHit.point - positions[i];
        didHit = true;
        hitStart = rayHit.point-toHitPoint.normalized;
        hitPoint = rayHit.point;
      }
    }
    if (didHit) {
      ContactIndicator.SetActive(true);
      ContactIndicator.transform.position = hitStart;
      ContactIndicator.transform.LookAt(hitPoint);
    } else {
      ContactIndicator.SetActive(false);
    }
    LineRenderer.enabled = true;
    LineRenderer.SetPositions(positions);
  }

  public void Hide() {
    LineRenderer.enabled = false;
    ContactIndicator.SetActive(false);
  }
}