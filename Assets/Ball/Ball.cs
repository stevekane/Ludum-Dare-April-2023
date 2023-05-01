using UnityEngine;

public class Ball : MonoBehaviour {
  public TrailRenderer TrailRenderer;
  public Rigidbody Rigidbody;
  public int HitStopFrames;
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
    if (collision.gameObject.tag == "Ground")
      Destroy(gameObject, .01f);
  }
}