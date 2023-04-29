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
    Vector3 hitFrom = default;
    Vector3 hitTo = default;
    for (var i = 0; i < positions.Length; i++) {
      positions[i] = position;
      velocity += Time.fixedDeltaTime * Physics.gravity;
      position += Time.fixedDeltaTime * velocity;
      var delta = position - positions[i];
      var hit = Physics.Raycast(positions[i], delta.normalized, out var rayHit, delta.magnitude, LayerMask, QueryTriggerInteraction.Collide);
      if (!didHit && hit) {
        didHit = true;
        hitFrom = positions[i];
        hitTo = position;
      }
    }
    if (didHit) {
      ContactIndicator.SetActive(true);
      ContactIndicator.transform.position = hitFrom;
      ContactIndicator.transform.LookAt(hitTo);
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