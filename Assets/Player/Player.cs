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

  [SerializeField] Transform AimTransform;
  [SerializeField] Transform LaunchTransform;
  [SerializeField] float TurnSpeed;
  [SerializeField] float SwingForce;
  [SerializeField] float LaunchHeight;
  [SerializeField] Timeval SwingDuration;
  [SerializeField] GameObject BallPrefab;
  [SerializeField] Animator Animator;
  [SerializeField] ProjectileArcRenderer ProjectileArcRenderer;

  bool Serving;
  bool Swinging;
  PlayerActions Actions;
  TaskScope Scope;
  GameObject Ball;

  bool CanAim => true;
  bool CanServe => (Ball == null || Ball.transform.position.y < 0) && !Serving && !Swinging;
  bool CanSwing => (Ball != null && Ball.transform.position.y > 0) && !Serving && !Swinging;

  void Start() {
    Scope = new();
    Actions = new();
    Actions.InGame.Serve.performed += Serve;
    Actions.InGame.Swing.performed += Swing;
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

  public void Serve(InputAction.CallbackContext ctx) {
    Animator.CrossFadeInFixedTime("Serve", .25f, 0);
    Serving = true;
  }

  public void LaunchBall() {
    var velocity = Vector3.up * Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * LaunchHeight);
    Ball = Instantiate(BallPrefab, LaunchTransform.position, LaunchTransform.rotation);
    Ball.GetComponent<Rigidbody>().velocity = velocity;
    Serving = false;
    Animator.CrossFade("Idle", .25f, 0);
  }

  public void Swing(InputAction.CallbackContext ctx) {
    if (!Swinging)
      Scope.Run(SwingTask);
  }

  async Task SwingTask(TaskScope scope) {
    Swinging = true;
    Animator.Play("Swing", 0);
    if (Ball && Vector3.Distance(Ball.transform.position, LaunchTransform.position) < 2) {
      Ball.GetComponent<Rigidbody>().velocity = SwingForce * AimTransform.forward;
      Ball = null;
    }
    await scope.Ticks(SwingDuration.Ticks);
    Animator.CrossFadeInFixedTime("Idle", .25f, 0);
    Swinging = false;
  }
}