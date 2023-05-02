using UnityEngine;

public class DeathBomb : MonoBehaviour {
  [SerializeField] public HurtType Type = HurtType.Red;
  [SerializeField] GameObject ExplosionVFX;
  [SerializeField] float Radius = 10f;

  Mob Owner;
  void Start() {
    Owner = GetComponentInParent<Mob>();
    Owner.OnDeath.Listen(OnDeath);
    var renderer = GetComponentInChildren<MeshRenderer>();
    renderer.material.color = MobBuilder.Instance.ColorForType(Type);
  }

  void OnDeath() {
    var obj = Instantiate(ExplosionVFX, transform.position, Quaternion.identity);
    obj.transform.localScale = Vector3.one*Radius;
    Destroy(obj, .25f);
    var hits = Physics.OverlapSphere(transform.position, Radius, 1 << gameObject.layer);
    foreach (var hit in hits) {
      if (hit.gameObject.TryGetComponent(out Hurtbox hb) && hb.Owner != Owner)
        hb.Owner.OnHurt(Type);
    }
  }
}