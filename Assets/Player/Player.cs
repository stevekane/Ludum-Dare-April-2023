using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
  static void SetEnabled(InputAction action, bool enabled) {
    if (enabled)
      action.Enable();
    else
      action.Disable();
  }
  public static Player Instance;

  [Header("Animation")]
  [SerializeField] Animator Animator;
  [SerializeField] string IdleName = "Idle";
  [SerializeField] string TossName = "Toss";
  [SerializeField] string[] KickNames = new string[2] { "High Kick", "High Kick 2" };
  [SerializeField] string[] CheerNames = new string[3] { "Cheering", "Rally", "Clapping" };
  [Header("Transforms")]
  [SerializeField] Transform AimTransform;
  [SerializeField] Transform LaunchTransform;
  [Header("Controls")]
  [SerializeField] float TurnSpeed;
  [Header("Properties")]
  [SerializeField] AnimationCurve ThrowHeight;
  [SerializeField] float SwingForce;
  [SerializeField] float ContactRadius = 1;
  [SerializeField] float ContactCameraShakeIntensity = 20;
  [SerializeField] Timeval ContactHitStopDuration = Timeval.FromSeconds(.5f);
  [Header("Prefabs")]
  [SerializeField] LayerMask BallLayerMask;
  [SerializeField] Ball RedBallPrefab;
  [SerializeField] Ball GreenBallPrefab;
  [SerializeField] Ball BlueBallPrefab;
  [SerializeField] GameObject HitVFXPrefab;
  [Header("Audio")]
  [SerializeField] AudioClip HitSFX;
  [SerializeField] AudioClip SwingSFX;
  [Header("Components")]
  [SerializeField] ProjectileArcRenderer ProjectileArcRenderer;
  [SerializeField] LocalTimeScale LocalTimeScale;
  [SerializeField] AudioSource AudioSource;
  [SerializeField] Vibrator Vibrator;

  public int Score = 0;
  [SerializeField] TextMeshProUGUI ScoreText;

  int HitStopFrames;
  bool Serving;
  bool Swinging;
  bool Cheering;
  PlayerActions Actions;
  TaskScope Scope;
  EventSource ServeReleasedSource = new();
  EventSource SwingReleasedSource = new();
  EventSource LaunchBallSource = new();
  EventSource EndServeSource = new();

  bool CanAim => true;
  bool CanServe => !Swinging;
  bool CanSwing => !Serving && !Swinging;

  Dictionary<InputAction, (Action, Action, Func<bool>)> Buttons = new();

  [SerializeField] Timeval BufferDuration = Timeval.FromAnimFrames(6,60);
  Dictionary<InputAction, int> BufferPress = new();
  Dictionary<InputAction, int> BufferRelease = new();

  void Start() {
    Instance = this;
    Animator.enabled = false;
    Scope = new();
    Actions = new();
    Actions.InGame.Restart.Enable();
    Actions.InGame.Restart.performed += ctx => Reload();
    Buttons[Actions.InGame.Red] = (() => Run(Serve(RedBallPrefab)), ReleaseServe, () => CanServe);
    Buttons[Actions.InGame.Green] = (() => Run(Serve(GreenBallPrefab)), ReleaseServe, () => CanServe);
    Buttons[Actions.InGame.Blue] = (() => Run(Serve(BlueBallPrefab)), ReleaseServe, () => CanServe);
    Buttons[Actions.InGame.Swing] = (() => Run(Swing), SwingReleasedSource.Fire, () => CanSwing);
    GameManager.Instance.OnGoal.Listen(OnCheer);
  }

  void OnDestroy() {
    Actions.Dispose();
  }

  void Reload() {
    Debug.Log("Reload");
    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
  }

  Task Run(TaskFunc f) {
    Scope?.Dispose();
    Scope = new();
    return Scope.Run(f);
  }

  void OnCheer() {
    if (!Cheering && !Swinging && !Serving) {
      Run(Cheer);
    }
  }

  async Task Cheer(TaskScope scope) {
    try {
      var cheerName = CheerNames[Mathf.RoundToInt(UnityEngine.Random.Range(0,CheerNames.Length))];
      Cheering = true;
      Animator.CrossFade(cheerName, .25f, 0);
      await scope.Ticks(Timeval.FromSeconds(1).Ticks);
    } finally {
      Cheering = false;
      Animator.CrossFade(IdleName, .25f, 0);
    }
  }

  void FixedUpdate() {
    SetEnabled(Actions.InGame.Aim, CanAim);
    SetEnabled(Actions.InGame.Red, true);
    SetEnabled(Actions.InGame.Green, true);
    SetEnabled(Actions.InGame.Blue, true);
    SetEnabled(Actions.InGame.Swing, true);
    Aim();
    HandleButtonBuffer();
    ProjectileArcRenderer.Render(LaunchTransform.position, SwingForce * LaunchTransform.forward);
    LocalTimeScale.Value = HitStopFrames > 0 ? 0 : 1;
    Animator.Update(Time.fixedDeltaTime * LocalTimeScale.Value);
    HitStopFrames = Mathf.Max(0, HitStopFrames-1);
    ScoreText.text = $"Score: {Score}";
  }

  void Aim() {
    var axis = Actions.InGame.Aim.ReadValue<Vector2>();
    var currentAim = AimTransform.rotation.eulerAngles;
    var currentPitch = currentAim.x;
    currentPitch = (currentPitch + 180f) % 360f - 180f;
    currentAim.x = Mathf.Clamp(currentPitch + axis.y * Time.fixedDeltaTime * TurnSpeed, -75, 75);
    AimTransform.rotation = Quaternion.Euler(currentAim);
    transform.rotation *= Quaternion.Euler(0, axis.x * Time.fixedDeltaTime * TurnSpeed, 0);
  }

  void HandleButtonBuffer() {
    foreach (var b in Buttons) {
      if (b.Key.WasPressedThisFrame())
        BufferPress[b.Key] = Timeval.TickCount;
      if (b.Key.WasReleasedThisFrame())
        BufferRelease[b.Key] = Timeval.TickCount;

      {
        if (BufferPress.TryGetValue(b.Key, out int tickCount) && b.Value.Item3() && Timeval.TickCount - tickCount <= BufferDuration.Ticks) {
          b.Value.Item1();
          BufferPress.Remove(b.Key);
        }
      }
      {
        if (BufferRelease.TryGetValue(b.Key, out int tickCount) && /* b.Value.Item3() &&*/ Timeval.TickCount - tickCount <= BufferDuration.Ticks) {
          b.Value.Item2();
          BufferRelease.Remove(b.Key);
        }
      }
    }
  }

  int ServeStart;
  int ServeEnd;
  bool ServeCharging;
  public void ReleaseServe() => ServeEnd = Timeval.TickCount;
  public void LaunchBall() => LaunchBallSource.Fire();
  public void EndServe() => EndServeSource.Fire();
  TaskFunc Serve(Ball ballPrefab) => async scope => {
    try {
      ServeStart = Timeval.TickCount;
      ServeEnd = Timeval.TickCount;
      Serving = true;
      Animator.CrossFadeInFixedTime(TossName, .25f, 0);
      await scope.ListenFor(LaunchBallSource);
      if (ServeEnd == ServeStart)
        ServeEnd = Timeval.TickCount;
      var totalFrames = Timeval.TickCount - ServeStart;
      var fraction = (float)(ServeEnd-ServeStart)/totalFrames;
      fraction = fraction < .8 ? 0 : 1;
      var launchHeight = ThrowHeight.Evaluate(fraction);
      var velocity = Vector3.up * Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * launchHeight);
      var ball = Instantiate(ballPrefab, LaunchTransform.position, LaunchTransform.rotation);
      ball.GetComponent<Rigidbody>().velocity = velocity;
      Destroy(ball.gameObject, 10);
      await scope.ListenFor(EndServeSource);
    } finally {
      Animator.CrossFadeInFixedTime(IdleName, .25f, 0);
      Serving = false;
    }
  };

  public void Contact() {
    Swinging = false;
    var hits = Physics.OverlapSphere(LaunchTransform.position, ContactRadius, BallLayerMask);
    if (hits.Length > 0) {
      HitStopFrames = ContactHitStopDuration.Ticks;
      CameraShaker.Instance.Shake(ContactCameraShakeIntensity);
      AudioSource.PlayOneShot(HitSFX);
      foreach (var hit in hits) {
        Destroy(Instantiate(HitVFXPrefab, hit.transform.position, transform.rotation), 3);
        var ball = hit.GetComponent<Ball>();
        ball.HitStopFrames = ContactHitStopDuration.Ticks;
        ball.StoredVelocity = SwingForce * LaunchTransform.forward;
        ball.TrailRenderer.enabled = true;
        Vibrator.Vibrate(transform.forward, ContactHitStopDuration.Ticks, .5f);
      }
    }
  }

  async Task Swing(TaskScope scope) {
    try {
      Swinging = true;
      AudioSource.PlayOneShot(SwingSFX);
      Animator.CrossFadeInFixedTime(KickNames[Mathf.RoundToInt(UnityEngine.Random.Range(0,KickNames.Length))], .1f, 0);
      await scope.Any(
        Waiter.Ticks(30),
        Waiter.ListenFor(ServeReleasedSource));
    } finally {
      Animator.CrossFadeInFixedTime(IdleName, .25f, 0);
      Swinging = false;
    }
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(LaunchTransform.position, ContactRadius);
  }
}