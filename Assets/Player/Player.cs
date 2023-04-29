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
  // TODO: Multiple balls
  bool CanServe => (Ball == null || Ball.transform.position.y < 0) && !Serving && !Swinging;
  bool CanSwing => (Ball != null && Ball.transform.position.y > 0) && !Serving && !Swinging;

  void Start() {
    Animator.enabled = false;
    Scope = new();
    Actions = new();
    Actions.InGame.Serve.started += Serve;
    Actions.InGame.Serve.canceled += ctx => ServeReleased.Fire();
    Actions.InGame.Swing.performed += Swing;
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

  public void Aim() {
    var axis = Actions.InGame.Aim.ReadValue<Vector2>();
    var currentAim = AimTransform.rotation.eulerAngles;
    var currentPitch = currentAim.x;
    currentPitch = (currentPitch + 180f) % 360f - 180f;
    currentAim.x = Mathf.Clamp(currentPitch + axis.y * Time.fixedDeltaTime * TurnSpeed, -75, 75);
    AimTransform.rotation = Quaternion.Euler(currentAim);
    transform.rotation *= Quaternion.Euler(0, axis.x * Time.fixedDeltaTime * TurnSpeed, 0);
  }

  public async void Serve(InputAction.CallbackContext ctx) {
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
    var chargeFactor = 1f + TossCharge.ElapsedPercent;
    var velocity = Vector3.up * Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * LaunchHeight * chargeFactor);
    Ball = Instantiate(BallPrefab, LaunchTransform.position, LaunchTransform.rotation);
    Ball.GetComponent<Rigidbody>().velocity = velocity;
    Destroy(Ball, 10);
  }

  public void EndServe() {
    Animator.CrossFade(IdleName, .25f, 0);
    Serving = false;
  }

  public void Swing(InputAction.CallbackContext ctx) {
    Scope.Run(SwingTask);
  }

  public void Contact() {
    Swinging = false;
    if (Ball && Vector3.Distance(Ball.transform.position, LaunchTransform.position) < ContactRadius) {
      HitStopFrames = ContactHitStopDuration.Ticks;
      CameraShaker.Instance.Shake(ContactCameraShakeIntensity);
      AudioSource.PlayOneShot(HitSFX);
      Destroy(Instantiate(HitVFXPrefab, Ball.transform.position, transform.rotation), 3);
      Ball.HitStopFrames = ContactHitStopDuration.Ticks;
      Ball.StoredVelocity = SwingForce * AimTransform.forward;
      Ball.TrailRenderer.enabled = true;
    }
    Ball = null;
  }

  async Task SwingTask(TaskScope scope) {
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