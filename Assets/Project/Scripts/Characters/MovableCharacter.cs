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
    [SerializeField] protected bool canDoubleJump = false;
    [SerializeField] protected float doubleJumpForce = 10f;
    [SerializeField] protected float jumpTolerance = 0.05f;

    [Header("Wall Stick/Jumping")]
    [SerializeField] protected WallStick wallStick;

    protected PlatformerPhysics2D physics;
    protected Rigidbody2D rigidBody;

    private float curDashTime;
    private bool holdingDash;
    private float curMoveSpeed;
    private bool initDashFacingDir;
    private bool didDoubleJump;
    private bool canStillJump;
    private float curJumpToleranceTime;

    public void HorizontalMove(float moveScalar) {
      physics.Move(Vector2.right * curMoveSpeed * moveScalar);
    }

    public virtual void OnJumpPress() {
      var grounded = physics.Grounded();

      // Handle wall jumping
      if (wallStick.StickingToWall) {
        curMoveSpeed = holdingDash ? dashSpeed : runSpeed;
        wallStick.JumpOffWall(holdingDash);
        return;
      }
      
      // Handle jumping from ground
      if (grounded || canStillJump) {
        curMoveSpeed = (holdingDash && curDashTime < maxDashTime) ? dashSpeed : runSpeed;
        physics.SetVelocity(Vector2.up * jumpForce);
      }

      // Handle double jumping
      if (canDoubleJump && !grounded && !didDoubleJump) {
        didDoubleJump = true;
        physics.SetVelocity(Vector2.up * doubleJumpForce);
      }
    }

    public virtual void OnJumpHold() { }

    public virtual void OnJumpRelease() {
      var gDir = physics.GravityDirection();
      if ((gDir == -1 && physics.CurrentVelocity.y > 0.0f || gDir == 1 && physics.CurrentVelocity.y < 0.0f)) {
        physics.SetVelocity(Vector2.zero);  
      }
    }

    public virtual void OnDashPress() {
      initDashFacingDir = FacingRight();
    }

    public virtual void OnDashHold() {
      holdingDash = true;
      curDashTime += Time.deltaTime;
      var facingRight = FacingRight();
      if (initDashFacingDir == facingRight && curDashTime < maxDashTime && physics.Grounded()) {
        physics.Move((facingRight ? Vector2.right : Vector2.left) * dashSpeed);
      } else {
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
      } else {
        canStillJump = true;
        curJumpToleranceTime = 0.0f;
      }
    }

    private void OnWallStickChange(bool stickingToWall) {
      if (stickingToWall) {
        didDoubleJump = false;
        curMoveSpeed = runSpeed;
      } else {
        canStillJump = true;
        curJumpToleranceTime = 0.0f;
      }
    }

    private void Awake() {
      physics = GetComponent<PlatformerPhysics2D>();
      rigidBody = GetComponent<Rigidbody2D>();
      physics.RegisterModifier(wallStick);
      physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);
      wallStick.OnWallStickChange(OnWallStickChange);
      curMoveSpeed = runSpeed;
    }

    private void Update() {
      // Handle jump tolerance
      if (canStillJump) {
        curJumpToleranceTime += Time.deltaTime;
        if (curJumpToleranceTime > jumpTolerance) {
          canStillJump = false;
        }
      }
    }

    private bool FacingRight() {
      return transform.localScale.x == 1;
    }
  }
}
