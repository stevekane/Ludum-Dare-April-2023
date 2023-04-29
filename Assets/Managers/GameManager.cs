using UnityEngine;

public enum GameState {
  Menu,
  InGame
}

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  void Start() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      DontDestroyOnLoad(gameObject);
      Instance = this;
    }
  }
}