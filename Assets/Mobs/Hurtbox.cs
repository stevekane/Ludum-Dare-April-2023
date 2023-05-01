using UnityEngine;

public enum HurtType { Red, Green, Blue }

public class Hurtbox : MonoBehaviour {
  public Mob Owner { get; set; }

  void OnCollisionEnter(Collision collision) {
    var type = collision.gameObject.tag switch {
      "Red" => HurtType.Red,
      "Green" => HurtType.Green,
      "Blue" => HurtType.Blue,
      _ => (HurtType)(-1)
    };
    if (type == (HurtType)(-1))
      return;
    Owner.OnHurt(type);
  }

  void Start() {
    Owner = GetComponentInParent<Mob>();
  }
}