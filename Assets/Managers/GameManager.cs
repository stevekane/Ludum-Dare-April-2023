using UnityEngine;

public enum GameState {
  Menu,
  InGame
}

public class GameManager : MonoBehaviour {
  public static GameManager Instance;

  public Material[] ColorMaterials;
  public EventSource OnGoal = new();

  public Material MaterialForHurtType(HurtType type) => ColorMaterials[(int)type];

  void Awake() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      DontDestroyOnLoad(gameObject);
      Instance = this;
    }
  }
}