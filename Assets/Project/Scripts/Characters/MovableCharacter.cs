using DreamState.Global;
using System;
using UnityEngine;

namespace DreamState {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(PlatformerPhysics2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  public abstract class MovableCharacter : MonoBehaviour {
    public WallStick WallStick { get { return wallStick; } }
    public bool Dashing { get { return holdingDash && curDashTime < maxDashTime; } }

    [Header("Movement")]
    [SerializeField] protected float runSpeed = 10f;
    [SerializeField] protected float dashSpeed = 15f;
    [SerializeField] protected float maxDashTime = 0.4f;

    [Header("Jumping")]
    [SerializeField] protected float jumpForce = 20f;
    [SerializeField] protected float doubleJumpForce = 10f;
    [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
    [SerializeField] private Vector2 dashWallJump = new Vector2(20f, 20f);
    [SerializeField] protected bool canDoubleJump = false;
    [SerializeField] protected float jumpTolerance = 0.2f;
    [SerializeField] protected float dashWallJumpTolerance = 0.2f;

    [Tooltip("Amount of time to apply acceleration after wall jump before re-enabling instant movement")]
    [SerializeField]
    protected float wallJumpFloatTime = 0.2f;

    [Header("Wall Stick")]
    [SerializeField] protected WallStick wallStick;

    protected PlatformerPhysics2D physics;
    protected Rigidbody2D rigidBody;

    private float curDashTime;
    private bool holdingDash;
    private float curMoveSpeed;
    private bool initDashFacingDir;
    private bool didDoubleJump;
    private float curJumpToleranceTime;
    private float curDashWallJumpTime;
    private float curWallJumpFloatTime;

    public virtual void HorizontalMove(float moveScalar) {
      physics.Move(Vector2.right * curMoveSpeed * moveScalar);
    }

    public virtual void OnJumpPress() {
      var grounded = physics.Grounded();

      // Handle wall jumping
      if (wallStick.StickingToWall) {
        JumpOffWall();
        return;
      }
      
      // Handle jumping from ground
      if (grounded || curJumpToleranceTime < jumpTolerance) {
        curMoveSpeed = (holdingDash && curDashTime < maxDashTime) ? dashSpeed : runSpeed;
        physics.SetVelocityY(jumpForce);
        return;
      }

      // Handle double jumping
      if (canDoubleJump && !grounded && !didDoubleJump) {
        didDoubleJump = true;
        physics.SetVelocityY(doubleJumpForce);
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
      initDashFacingDir = FacingRight();
      curDashWallJumpTime = 0.0f;
    }

    public virtual void OnDashHold() {
      holdingDash = true;
      curDashTime += Time.deltaTime;
      var facingRight = FacingRight();
      if (initDashFacingDir == facingRight && curDashTime < maxDashTime && physics.Grounded()) {
        physics.Move((facingRight ? Vector2.right : Vector2.left) * dashSpeed);
      }
    }

    public virtual void OnDashRelease() {
      holdingDash = false;
      curDashTime = 0.0f;
    }

    private void OnGroundedChange(bool grounded) {
      if (grounded) {
        didDoubleJump = false;
        curMoveSpeed = runSpeed;

        // Prevent dashing when just landing
        if (holdingDash) {
          curDashTime = maxDashTime;
        }
      } else {
        curJumpToleranceTime = 0.0f;
      }
    }

    private void OnWallStickChange(bool stickingToWall) {
      if (stickingToWall) {
        didDoubleJump = false;
        curMoveSpeed = runSpeed;
      } else {
        curJumpToleranceTime = 0.0f;
      }
    }

    private void JumpOffWall() {
      // Let physics calculate acceleration for a time
      curWallJumpFloatTime = 0.0f;
      physics.SetInstantVelocity(false);

      // Set horizontal speed
      var useDashSpeed = curDashWallJumpTime < dashWallJumpTolerance;
      curMoveSpeed = useDashSpeed ? dashSpeed : runSpeed;

      // Determine and set wall jump velocity
      var v = useDashSpeed ? dashWallJump : wallJump;
      physics.SetVelocity(-new Vector2(v.x * Mathf.Sign(physics.TargetVelocity.x), v.y * physics.GravityDirection()));
    }

    private void Awake() {
      physics = GetComponent<PlatformerPhysics2D>();
      rigidBody = GetComponent<Rigidbody2D>();
      physics.RegisterModifier(wallStick);
      physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);
      wallStick.OnWallStickChange(OnWallStickChange);
      curMoveSpeed = runSpeed;
      physics.SetInstantVelocity(true);
    }

    private void Update() {
      if (curJumpToleranceTime < jumpTolerance) {
        curJumpToleranceTime += Time.deltaTime;
      }
      if (curDashWallJumpTime < dashWallJumpTolerance) {
        curDashWallJumpTime += Time.deltaTime;
      }
      if (curWallJumpFloatTime < wallJumpFloatTime) {
        curWallJumpFloatTime += Time.deltaTime;
        if (curWallJumpFloatTime >= wallJumpFloatTime) {
          physics.SetInstantVelocity(true);
        }
      }
    }

    private bool FacingRight() {
      return transform.localScale.x == 1;
    }
  }
}
