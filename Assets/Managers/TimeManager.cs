using UnityEngine;

public class TimeManager : MonoBehaviour {
  public static TimeManager Instance;

  void Start() {
    if (Instance) {
      Destroy(gameObject);
    } else {
      Time.fixedDeltaTime = 1f / Timeval.FixedUpdatePerSecond;
      DontDestroyOnLoad(gameObject);
      Instance = this;
    }
  }

  void FixedUpdate() {
    if (Timeval.TickEvent != null) {
      Timeval.TickEvent.Fire();
    }
    Timeval.TickCount++;
  }
}