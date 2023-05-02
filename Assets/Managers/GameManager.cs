using System;
using TMPro;
using UnityEngine;

public enum GameState {
  Menu,
  InGame
}

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  public TextMeshProUGUI ScoreText;
  public TextMeshProUGUI GameOverText;
  public EventSource OnGoal = new();

  bool GameOver = false;
  TaskScope Scope = new();
  public void OnGameOver() {
    GameOver = true;
    ScoreText.text = "";
    GameOverText.text = $"Victory!\nScore: {Player.Instance.Score}";
    Scope.Start(async s => {
      await s.Millis(5000);
      GameOverText.text += "\nPress 'r' to play again";
    });
    Scope.Start(async s => {
      var codes = new[] { "R", "G", "B", "R,G", "G,B", "R,B", "R,G,B" };
      for (int i = 0; i < 20; i++) {
        await s.Millis(UnityEngine.Random.Range(300, 1200));
        var code = codes[UnityEngine.Random.Range(0, codes.Length)];
        var pos = new Vector3(0, 20f, 40f) + UnityEngine.Random.insideUnitSphere * 20f;
        var mob = MobBuilder.Instance.Build(pos, Quaternion.identity, code);
        mob.Explode();
      }
    });
  }

  void OnReload() {
    Scope.Dispose();
    Scope = new();
    GameOver = false;
    GameOverText.text = "";
  }

  void FixedUpdate() {
    if (!GameOver)
      ScoreText.text = $"Score: {Player.Instance.Score}";
  }

  void Awake() {
    if (Instance) {
      Instance.OnReload();
      Destroy(gameObject);
    } else {
      DontDestroyOnLoad(gameObject);
      Instance = this;
    }
  }

  void OnDestroy() => Scope.Dispose();
}