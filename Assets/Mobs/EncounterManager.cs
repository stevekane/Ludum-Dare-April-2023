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
  public string ReadLine() {
    var start = Pos;
    while (!EOF && ReadChar() is var c && c != '\n')
      ;
    return Text.Substring(start, Mathf.Max(0, Pos - start - 1));
  }
}

class Spawn {
  public GameObject Prefab;
  public Vector3 Position;
}

class Wave {
  int Delay;
  List<Spawn> Spawns = new();

  public static Wave Parse(Parser parser, Transform center, Dictionary<char, GameObject> objectMap) {
    if (parser.EOF)
      return null;
    var wave = new Wave();
    try {
      wave.Delay = parser.ReadInt();
      for (int i = 0; i < 4; i++) {
        var line = parser.ReadLine();
        for (int j = 0; j < line.Length; j++) {
          if (objectMap.TryGetValue(line[j], out var obj)) {
            wave.Spawns.Add(new Spawn { Prefab = obj, Position = GetPosition(center, i, j) });
          } else if (!char.IsWhiteSpace(line[j])) {
            Debug.LogWarning($"Unknown object code in encounter: {line[j]}");
          }
        }
      }
    } catch (Exception e) {
      Debug.LogError($"Parse error at {parser.Location}: {e}");
    }
    return wave;
  }

  static Vector3 GetPosition(Transform center, int y, int x) {
s    return center.position + new Vector3(x*5 - 7.5f, 20f - y*5, 0f);
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

  public static Encounter Parse(Parser parser, Transform center, Dictionary<char, GameObject> objectMap) {
    var encounter = new Encounter();
    while (Wave.Parse(parser, center, objectMap) is var wave && wave != null) {
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
  [SerializeField] ObjectMap[] ObjectMappings;
  Dictionary<char, GameObject> ObjectMappingsDict = new();
  Encounter Encounter = new();
  TaskScope Scope = new();

  public Encounter Parse(string descr, Transform center) {
    var parser = new Parser(descr);
    return Encounter.Parse(parser, center, ObjectMappingsDict);
  }

  void Start() {
    ObjectMappingsDict = new();
    foreach (var m in ObjectMappings)
      ObjectMappingsDict[m.Code] = m.Prefab;
    Encounter = Parse(File.text, transform);
    Scope.Run(Encounter.Run);
  }

  void OnDestroy() => Scope.Dispose();
}