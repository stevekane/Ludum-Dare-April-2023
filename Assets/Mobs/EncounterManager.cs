using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Parser {
  public string Text;
  int Pos = 0;
  int Line = 0;

  public Parser(string text) => Text = text;

  public string Location => $"Line {Line}, position {Pos}";
  public bool EOF => Pos == Text.Length;
  public char ReadChar() {
    if (Text[Pos++] is var c && c == '\n')
      Line++;
    return c;
  }
  public int ReadInt() {
    return int.Parse(ReadLine());
  }
  static char[] Newline = new char[] { '\n', '\r' };
  public string ReadLine() {
    var start = Pos;
    while (!EOF && ReadChar() is var c && c != '\n')
      ;
    return Text.Substring(start, Mathf.Max(0, Pos - start)).Trim(Newline);
  }
}

class Spawn {
  public string Code;
  public Vector3 Position;
}

class Wave {
  int Delay;
  List<Spawn> Spawns = new();

  public static Wave Parse(Parser parser, Transform center) {
    if (parser.EOF)
      return null;
    var wave = new Wave();
    try {
      wave.Delay = parser.ReadInt();
      for (int i = 0; i < 4; i++) {
        var line = parser.ReadLine();
        for (int j = 0; line.Length > 0; j++) {
          var code = ParseCode(ref line);
          if (code != " ") {
            wave.Spawns.Add(new Spawn { Code = code, Position = GetPosition(center, i, j) });
          }
        }
      }
    } catch (Exception e) {
      Debug.LogError($"Parse error at {parser.Location}: {e}");
    }
    return wave;
  }

  static char[] Separators = new char[] { '/', ' ' };
  static string ParseCode(ref string line) {
    var i = line.IndexOfAny(Separators);
    if (i == 0) {
      line = line.Substring(1);
      return " ";
    }

    if (i == -1) i = line.Length;
    var result = line.Substring(0, i);
    if (i < line.Length && line[i] == '/')
      i++;
    line = line.Substring(i);
    return result;
  }

  static Vector3 GetPosition(Transform center, int y, int x) {
    return center.position + new Vector3(x*5 - 7.5f, 20f - y*5, 0f);
  }

  public async Task Run(TaskScope scope) {
    try {
      await scope.Seconds(Delay);
      var rotation = Quaternion.LookRotation(Vector3.back);
      for (int i = 0; i < Spawns.Count; i++) {
        var mob = MobBuilder.Instance.Build(Spawns[i].Position, rotation, Spawns[i].Code);
        mob.RegenDuration = Timeval.FromAnimFrames(Mob.MaxTotalTicks / mob.HurtSequence.Length, 60);
        Debug.Log($"{Mob.MaxTotalTicks} / {mob.HurtSequence.Length} = {mob.RegenDuration.Ticks}");
      }
    } finally {

    }
  }
}

public class Encounter {
  List<Wave> Waves = new();

  public static Encounter Parse(Parser parser, Transform center) {
    var encounter = new Encounter();
    while (Wave.Parse(parser, center) is var wave && wave != null) {
      encounter.Waves.Add(wave);
    }
    return encounter;
  }

  public async Task Run(TaskScope scope) {
    for (int i = 0; i < Waves.Count; i++)
      await Waves[i].Run(scope);
  }
}

public class EncounterManager : MonoBehaviour {
  [Serializable]
  class ObjectMap {
    public char Code;
    public GameObject Prefab;
  }

  [SerializeField] TextAsset File;
  Encounter Encounter = new();
  TaskScope Scope = new();

  public Encounter Parse(string descr, Transform center) {
    var parser = new Parser(descr);
    return Encounter.Parse(parser, center);
  }

  void Start() {
    Encounter = Parse(File.text, transform);
    Scope.Run(Encounter.Run);
  }
  void OnDestroy() => Scope.Dispose();
}