using UnityEngine;

public class Vibrator : MonoBehaviour {
  [SerializeField] Transform Target;

  Vector3 Axis;
  Vector3 LocalPosition;
  float Amplitude;
  int FramesRemaining;
  int Sign = 1;

  public void Vibrate(Vector3 axis, int frames, float amplitude) {
    Axis = axis;
    Amplitude = amplitude;
    FramesRemaining = Mathf.Max(FramesRemaining,frames);
  }

  void Start() {
    LocalPosition = Target.transform.localPosition;
  }

  void FixedUpdate() {
    if (FramesRemaining > 0) {
      Sign *= -1;
      Target.transform.localPosition = LocalPosition+Sign*Amplitude*Axis;
      FramesRemaining--;
    } else {
      Target.transform.localPosition = LocalPosition;
    }
  }
}