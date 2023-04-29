using UnityEngine;

public class Goal : MonoBehaviour {
  [SerializeField] MeshRenderer MeshRenderer;

  void OnCollisionEnter(Collision c) {
    Debug.Log("Collision");
    MeshRenderer.material.color = Color.green;
  }
}