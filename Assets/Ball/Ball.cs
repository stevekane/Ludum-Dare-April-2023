using UnityEngine;

public class Ball : MonoBehaviour {
  public TrailRenderer TrailRenderer;
  public Rigidbody Rigidbody;
  public int HitStopFrames;
  public GameObject VFX;
  public Vector3 StoredVelocity;

  void FixedUpdate() {
    if (HitStopFrames == 0) {
      Rigidbody.isKinematic = false;
    } else if (HitStopFrames == 1) {
      Rigidbody.isKinematic = false;
      Rigidbody.velocity = StoredVelocity;
    } else {
      Rigidbody.isKinematic = true;
    }
    HitStopFrames = Mathf.Max(0, HitStopFrames-1);
  }

  void OnCollisionEnter(Collision collision) {
    Destroy(Instantiate(VFX, transform.position, transform.rotation), 3);
    Destroy(gameObject);
  }
}