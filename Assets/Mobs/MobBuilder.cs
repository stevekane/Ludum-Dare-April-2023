using System.Collections.Generic;
using UnityEngine;

public class MobBuilder : MonoBehaviour {
  public static MobBuilder Instance;
  [SerializeField] Mob MobPrefab;
  [SerializeField] DeathBomb DeathBombPrefab;
  [SerializeField, ColorUsage(true, true)] Color RedColor;
  [SerializeField, ColorUsage(true, true)] Color GreenColor;
  [SerializeField, ColorUsage(true, true)] Color BlueColor;

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      DontDestroyOnLoad(gameObject);
      Instance = this;
    }
  }

  public Mob Build(Vector3 position, Quaternion rotation, HurtPair[] hurtSequence, float regenDuration = 1f, GameObject modifier = null) {
    var mob = Instantiate(MobPrefab, position, rotation);
    mob.HurtSequence = hurtSequence;
    if (modifier)
      modifier.transform.SetParent(mob.transform, false);
    return mob;
  }
  public Mob Build(Vector3 position, Quaternion rotation, string descr, float regenDuration = 1f) {
    (var hurtSequence, var modifier) = MobCodes(descr);
    return Build(position, rotation, hurtSequence, regenDuration, modifier);
  }

  // rRG,G,BB
  (HurtPair[], GameObject) MobCodes(string descr) {
    var go = DeathBombCode(descr[0]);
    if (go)
      descr = descr.Substring(1);
    return (HurtCodes(descr), go?.gameObject);
  }

  // RB,G,BB
  HurtPair[] HurtCodes(string descr) {
    static HurtType Type(char code) => code switch {
      'R' => HurtType.Red,
      'G' => HurtType.Green,
      'B' => HurtType.Blue,
      _ => HurtType.Red,
    };
    var seq = new List<HurtPair>();
    for (int i = 0; i < descr.Length; i++) {
      var p = new HurtPair();
      p.Left = p.Right = Type(descr[i++]);
      if (i < descr.Length && descr[i] != ',') {
        p.Right = Type(descr[i++]);
        p.Split = true;
      }
      seq.Add(p);
      Debug.Assert(i == descr.Length || descr[i] == ',');
    }
    return seq.ToArray();
  }

  DeathBomb DeathBombCode(char code) {
    var type = code switch {
      'r' => HurtType.Red,
      'g' => HurtType.Green,
      'b' => HurtType.Blue,
      _ => (HurtType)(-1),
    };
    if (type == (HurtType)(-1))
      return null;
    var bomb = Instantiate(DeathBombPrefab);
    bomb.Type = type;
    return bomb;
  }

  public Color ColorForType(HurtType type) => type switch {
    HurtType.Red => RedColor,
    HurtType.Green => GreenColor,
    HurtType.Blue => BlueColor,
    _ => Color.black
  };
}