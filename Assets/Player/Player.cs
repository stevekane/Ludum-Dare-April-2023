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
  [SerializeField] Timeval TossChargeMinDuration = Timeval.FromAnimFrames(30, 60);
  [SerializeField] Timeval TossChargeMaxDuration = Timeval.FromAnimFrames(90, 60);
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
  EventSource ServeReleased = new();
  EventSource SwingReleased = new();
  ChargeTimer TossCharge = new();

  bool CanAim => true;
  bool CanServe => !Serving && !Swinging;
  bool CanSwing => (Ball != null && Ball.transform.position.y > 0) && !Serving && !Swinging;

  void Start() {
    Animator.enabled = false;
    Scope = new();
    Actions = new();
    Actions.InGame.Serve.started += ctx => Scope.Run(Serve);
    Actions.InGame.Serve.canceled += ctx => ServeReleased.Fire();
    Actions.InGame.Swing.performed += ctx => Scope.Run(Swing);
    Actions.InGame.Swing.canceled += ctx => SwingReleased.Fire();
  }

  void OnDestroy() {
    Actions.Dispose();
  }

  void FixedUpdate() {
    SetEnabled(Actions.InGame.Aim, CanAim);
    SetEnabled(Actions.InGame.Serve, CanServe);
    SetEnabled(Actions.InGame.Swing, CanSwing);
    Aim();
    if (Ball) {
      var v0 = SwingForce * AimTransform.forward;
      ProjectileArcRenderer.Render(Ball.transform.position, v0);
    } else {
      ProjectileArcRenderer.Hide();
    }
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

  async Task Serve(TaskScope scope) {
    Animator.CrossFadeInFixedTime(TossWindupName, .25f, 0);
    var released = Scope.ListenFor(ServeReleased);
    await Scope.Delay(TossChargeMinDuration);
    Animator.CrossFade(TossChargeName, .0f, 0);
    await Scope.Any(
      s => TossCharge.Ticks(s, TossChargeMaxDuration.Ticks),
      s => released);
    Serving = true;
    Animator.CrossFade(TossReleaseName, .0f, 0);
  }

  public void LaunchBall() {
    var chargeFactor = 1f + 2f*TossCharge.ElapsedPercent;  // range: 1-3
    var velocity = Vector3.up * Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * LaunchHeight * chargeFactor);
    Ball = Instantiate(BallPrefab, LaunchTransform.position, LaunchTransform.rotation);
    Ball.GetComponent<Rigidbody>().velocity = velocity;
    Destroy(Ball, 10);
  }

  public void EndServe() {
    Animator.CrossFade(IdleName, .25f, 0);
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
        ball.StoredVelocity = SwingForce * AimTransform.forward;
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
        Waiter.ListenFor(ServeReleased));
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