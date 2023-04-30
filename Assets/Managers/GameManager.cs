using UnityEngine;

public enum GameState {
  Menu,
  InGame
}

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  public EventSource OnGoal = new();

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      DontDestroyOnLoad(gameObject);
      Instance = this;
    }
  }
}