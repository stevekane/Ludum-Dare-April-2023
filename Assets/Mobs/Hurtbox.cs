using UnityEngine;

public enum HurtType { Red, Green, Blue }

public class Hurtbox : MonoBehaviour {
  public Mob Owner { get; set; }

  void OnCollisionEnter(Collision collision) {
    Debug.Log($"you hit me {gameObject.tag} vs {collision.gameObject.tag}");
    Owner.OnHurt(GetHurtType(collision.gameObject.tag));
  }

  static HurtType GetHurtType(string tag) {
    if (tag == "Red") return HurtType.Red;
    if (tag == "Green") return HurtType.Green;
    if (tag == "Blue") return HurtType.Blue;
    return HurtType.Red;
  }
}