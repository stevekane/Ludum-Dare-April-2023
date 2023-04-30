using UnityEngine;

public class MoveForward : MonoBehaviour {
  [SerializeField] float MoveSpeed;

  void FixedUpdate() {
    transform.position += MoveSpeed * Time.fixedDeltaTime * transform.forward;
  }
}