using UnityEngine;

public class Goal : MonoBehaviour {
  [SerializeField] MeshRenderer MeshRenderer;
  [SerializeField] AudioSource AudioSource;
  [SerializeField] GameObject ContactVFX;
  [SerializeField] AudioClip ContactSFX;
  [SerializeField] float ContactCameraShakeIntensity = 10;

  void OnCollisionEnter(Collision c) {
    AudioSource.PlayOneShot(ContactSFX);
    Destroy(Instantiate(ContactVFX, c.contacts[0].point, transform.rotation), 3);
    CameraShaker.Instance.Shake(ContactCameraShakeIntensity);
    MeshRenderer.material.color = Color.green;
    GameManager.Instance.OnGoal.Fire();
  }
}