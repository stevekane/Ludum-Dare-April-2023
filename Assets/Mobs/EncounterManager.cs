using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Parser {
  public string Text;
  int Pos = 0;

  public Parser(string text) => Text = text;

  public bool EOF => Pos == Text.Length;
  public char ReadChar() {
    return Text[Pos++];
  }
  public int ReadInt() {
    return int.Parse(ReadLine());
  }
  public string ReadLine() {
    var start = Pos;
    while (ReadChar() is var c && c != '\n')
      ;
    return Text.Substring(start, Pos - start - 1);
  }
  public void SkipSpace() {
    while (!EOF && char.IsWhiteSpace(Text[Pos]))
      Pos++;
  }
}

class Spawn {
  public GameObject Prefab;
  public Vector3 Position;
}

class Wave {
  int Delay;
  List<Spawn> Spawns = new();

  public static Wave Parse(Parser parser, Dictionary<char, GameObject> objectMap) {
    if (parser.EOF)
      return null;
    var wave = new Wave();
    wave.Delay = parser.ReadInt();
    for (int i = 0; i < 4; i++) {
      var line = parser.ReadLine();
      for (int j = 0; j < line.Length; j++) {
        if (objectMap.TryGetValue(line[j], out var obj)) {
          wave.Spawns.Add(new Spawn { Prefab = obj, Position = GetPosition(i, j) });
        } else if (!char.IsWhiteSpace(line[j])) {
          Debug.LogWarning($"Unknown object code in encounter: {line[j]}");
        }
      }
    }
    return wave;
  }

  static Vector3 GetPosition(int y, int x) {
    return new(x*5 - 7.5f, 20f - y*5, 120f);
  }

  public async Task Run(TaskScope scope) {
    await scope.Seconds(Delay);
    var rotation = Quaternion.LookRotation(Vector3.back);
    for (int i = 0; i < Spawns.Count; i++)
      GameObject.Instantiate(Spawns[i].Prefab, Spawns[i].Position, rotation);
  }
}

public class Encounter {
  List<Wave> Waves = new();

  public static Encounter Parse(Parser parser, Dictionary<char, GameObject> objectMap) {
    var encounter = new Encounter();
    while (Wave.Parse(parser, objectMap) is var wave && wave != null) {
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

  [SerializeField] ObjectMap[] ObjectMappings;
  Dictionary<char, GameObject> ObjectMappingsDict = new();
  Encounter Encounter = new();
  TaskScope Scope = new();

  [SerializeField, TextArea(10, 50)] string Test = @"5
R  G
 B

R  G
10
 G
 G

BBBB
";

  public Encounter Parse(string descr) {
    var parser = new Parser(descr);
    return Encounter.Parse(parser, ObjectMappingsDict);
  }

  void Start() {
    ObjectMappingsDict = new();
    foreach (var m in ObjectMappings)
      ObjectMappingsDict[m.Code] = m.Prefab;
    Encounter = Parse(Test);
    Scope.Run(Encounter.Run);
  }

  void OnDestroy() => Scope.Dispose();
}