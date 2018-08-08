using UnityEngine;

namespace DreamState {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(PlatformerPhysics2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  public abstract class MovableCharacter : MonoBehaviour {
    public WallStick WallStick { get { return wallStick; } }
    public bool Dashing { get { return dash.IsDashing || chargeDash.IsDashing; } }

    [Header("Movement")]
    [SerializeField] protected float runSpeed = 10f;
    [SerializeField] protected Dash dash;
    [SerializeField] protected ChargeDash chargeDash;
    [SerializeField] protected float chargeDashTime = 2.0f;

    [Header("Jumping")]
    [SerializeField] protected float jumpForce = 20f;
    [SerializeField] protected float doubleJumpForce = 10f;
    [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
    [SerializeField] protected bool canDoubleJump = false;
    [SerializeField] protected float jumpTolerance = 0.2f;

    [Tooltip("Amount of time to apply acceleration after wall jump before re-enabling instant movement")]
    [SerializeField]
    protected float wallJumpFloatTime = 0.2f;

    [Header("Wall Stick")]
    [SerializeField] protected WallStick wallStick;

    protected PlatformerPhysics2D physics;
    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    private bool didDoubleJump;
    private float curJumpToleranceTime;
    private float curWallJumpFloatTime;
    private bool didDashInAir = false;
    private float curChargeDashTime = 0.0f;

    public virtual void HorizontalMove(float moveScalar) {
      physics.Move(Vector2.right * runSpeed * moveScalar, curWallJumpFloatTime >= wallJumpFloatTime);
    }

    public virtual void OnJumpPress() {
      // Handle wall jumping
      if (wallStick.StickingToWall) {
        JumpOffWall();
        return;
      }

      var grounded = physics.Grounded();

      // Handle jumping from ground
      if (grounded || curJumpToleranceTime < jumpTolerance) {
        physics.SetVelocityY(jumpForce);
        return;
      }

      // Handle double jumping
      if (canDoubleJump && !grounded && !didDoubleJump) {
        didDoubleJump = true;
        physics.SetVelocityY(doubleJumpForce);
      }

      if (chargeDash.IsDashing) {
        chargeDash.SetDashing(false);
      }
    }

    public virtual void OnJumpHold() { }

    public virtual void OnJumpRelease() {
      var gDir = physics.GravityDirection();
      if ((gDir == -1 && physics.CurrentVelocity.y > 0.0f || gDir == 1 && physics.CurrentVelocity.y < 0.0f)) {
        physics.SetVelocityY(0);  
      }

      // Prevent tolerant jump if already jumped
      curJumpToleranceTime = jumpTolerance;
    }

    public virtual void OnDashPress() {
      if (didDashInAir || wallStick.StickingToWall) {
        return;
      }
      dash.SetDashing(true, FacingRight());
      if (!physics.Grounded()) {
        didDashInAir = true;
      }
    }

    public virtual void OnDashHold() {
      curChargeDashTime += Time.deltaTime;
    }

    public virtual void OnDashRelease() {
      if (curChargeDashTime > chargeDashTime) {
        chargeDash.SetDashing(true, FacingRight());

        // Prevent double jump out of a charge dash
        didDoubleJump = true;
      }
      curChargeDashTime = 0.0f;
    }

    private void OnGroundedChange(bool grounded) {
      if (grounded) {
        didDashInAir = false;
        didDoubleJump = false;
      } else {
        curJumpToleranceTime = 0.0f;
      }
    }

    private void OnWallStickChange(bool stickingToWall) {
      if (stickingToWall) {
        dash.SetDashing(false);
        chargeDash.SetDashing(false);
        didDashInAir = false;
        didDoubleJump = false;
      } else {
        curJumpToleranceTime = 0.0f;
      }
    }

    private void JumpOffWall() {
      // Let physics calculate acceleration for a time
      curWallJumpFloatTime = 0.0f;

      physics.SetVelocity(-new Vector2(wallJump.x * Mathf.Sign(physics.TargetVelocity.x), wallJump.y * physics.GravityDirection()));
    }

    private void Awake() {
      rigidBody = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();

      physics = GetComponent<PlatformerPhysics2D>();
      physics.RegisterModifier(wallStick);
      physics.RegisterModifier(dash);
      physics.RegisterModifier(chargeDash);
      physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);

      wallStick.OnWallStickChange(OnWallStickChange);

      curWallJumpFloatTime = wallJumpFloatTime;
    }

    private void Update() {
      if (curJumpToleranceTime < jumpTolerance) {
        curJumpToleranceTime += Time.deltaTime;
      }
      if (curWallJumpFloatTime < wallJumpFloatTime) {
        curWallJumpFloatTime += Time.deltaTime;
      }
    }

    private bool FacingRight() {
      return !spriteRenderer.flipX;
    }
  }
}
