using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
  static void SetEnabled(InputAction action, bool enabled) {
    if (enabled)
      action.Enable();
    else
      action.Disable();
  }

  [Header("Animation")]
  [SerializeField] Animator Animator;
  [SerializeField] string IdleName = "Idle";
  [SerializeField] string HighKickName = "High Kick";
  [SerializeField] string MediumKickName = "Front Kick";
  [SerializeField] string LowKickName = "Low Kick";
  [SerializeField] string TossName = "Toss";
  [SerializeField] string TossWindupName = "Toss Windup";
  [SerializeField] string TossChargeName = "Toss Charge Loop";
  [SerializeField] string TossReleaseName = "Toss Release";
  [SerializeField] string[] CheerNames = new string[3] { "Cheering", "Rally", "Clapping" };
  [Header("Transforms")]
  [SerializeField] Transform AimTransform;
  [SerializeField] Transform LaunchTransform;
  [Header("Controls")]
  [SerializeField] float TurnSpeed;
  [Header("Properties")]
  [SerializeField] AnimationCurve ThrowHeight;
  [SerializeField] float SwingForce;
  [SerializeField] float LaunchHeight;
  [SerializeField] float ContactRadius = 1;
  [SerializeField] float ContactCameraShakeIntensity = 20;
  [SerializeField] Timeval ContactHitStopDuration = Timeval.FromSeconds(.5f);
  [Header("Prefabs")]
  [SerializeField] LayerMask BallLayerMask;
  [SerializeField] Ball BallPrefab;
  [SerializeField] GameObject HitVFXPrefab;
  [Header("Audio")]
  [SerializeField] AudioClip HitSFX;
  [Header("Components")]
  [SerializeField] ProjectileArcRenderer ProjectileArcRenderer;
  [SerializeField] LocalTimeScale LocalTimeScale;
  [SerializeField] AudioSource AudioSource;

  int HitStopFrames;
  bool Serving;
  bool Swinging;
  PlayerActions Actions;
  TaskScope Scope;
  Ball Ball;
  EventSource ServeReleasedSource = new();
  EventSource SwingReleasedSource = new();
  EventSource LaunchBallSource = new();
  EventSource EndServeSource = new();
  ChargeTimer TossCharge = new();

  bool CanAim => true;
  bool CanServe => !Swinging;
  bool CanSwing => !Serving && !Swinging;

  void Start() {
    Animator.enabled = false;
    Scope = new();
    Actions = new();
    Actions.InGame.Serve.performed += ctx => Scope.Run(Serve);
    Actions.InGame.Serve.canceled += ctx => ReleaseServe();
    Actions.InGame.Serve.canceled += ctx => Debug.Log("Released");
    Actions.InGame.Swing.performed += ctx => Scope.Run(Swing);
    Actions.InGame.Swing.canceled += ctx => SwingReleasedSource.Fire();
  }

  void OnDestroy() {
    Actions.Dispose();
  }

  void FixedUpdate() {
    SetEnabled(Actions.InGame.Aim, CanAim);
    SetEnabled(Actions.InGame.Serve, CanServe);
    SetEnabled(Actions.InGame.Swing, CanSwing);
    Aim();
    ProjectileArcRenderer.Render(LaunchTransform.position, SwingForce * LaunchTransform.forward);
    LocalTimeScale.Value = HitStopFrames > 0 ? 0 : 1;
    Animator.Update(Time.fixedDeltaTime * LocalTimeScale.Value);
    HitStopFrames = Mathf.Max(0, HitStopFrames-1);
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

  int ServeStart;
  int ServeEnd;
  bool ServeCharging;
  public void ReleaseServe() => ServeEnd = Timeval.TickCount;
  public void LaunchBall() => LaunchBallSource.Fire();
  public void EndServe() => EndServeSource.Fire();
  async Task Serve(TaskScope scope) {
    ServeStart = Timeval.TickCount;
    ServeEnd = Timeval.TickCount;
    Serving = true;
    Animator.CrossFadeInFixedTime(TossName, .25f, 0);
    await scope.ListenFor(LaunchBallSource);
    if (ServeEnd == ServeStart)
      ServeEnd = Timeval.TickCount;
    var totalFrames = Timeval.TickCount - ServeStart;
    var fraction = (float)(ServeEnd-ServeStart)/totalFrames;
    var chargeFactor = ThrowHeight.Evaluate(fraction);
    var velocity = Vector3.up * Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * LaunchHeight * chargeFactor);
    Ball = Instantiate(BallPrefab, LaunchTransform.position, LaunchTransform.rotation);
    Ball.GetComponent<Rigidbody>().velocity = velocity;
    Destroy(Ball.gameObject, 10);
    await scope.ListenFor(EndServeSource);
    Animator.CrossFadeInFixedTime(IdleName, .25f, 0);
    Serving = false;
  }

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
      }
    }
    Ball = null;
  }

  async Task Swing(TaskScope scope) {
    try {
      Swinging = true;
      Animator.CrossFadeInFixedTime(HighKickName, .1f, 0);
      await scope.Any(
        Waiter.Ticks(30),
        Waiter.ListenFor(ServeReleasedSource));
      Animator.CrossFadeInFixedTime(IdleName, .25f, 0);
    } finally {
      Swinging = false;
    }
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(LaunchTransform.position, ContactRadius);
  }
}