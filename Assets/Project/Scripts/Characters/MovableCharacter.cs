using DreamState.Global;
using System;
using UnityEngine;

namespace DreamState {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(PlatformerPhysics2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(SpriteRenderer))]
  public abstract class MovableCharacter : MonoBehaviour {
    [SerializeField] protected float runSpeed = 10f;
    [SerializeField] protected float dashSpeed = 15f;
    [SerializeField] protected float maxDashTime = 0.4f;
    [SerializeField] protected float jumpForce = 20f;
    [SerializeField] protected float doubleJumpForce = 10f;
    [SerializeField] protected WallStick wallStick;

    protected PlatformerPhysics2D physics;
    protected Rigidbody2D rigidBody;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    private bool facingRight;
    private float curDashTime;
    private bool holdingDash;
    private float curMoveSpeed;
    private bool initDashFacingDir;
    private bool didDoubleJump;

    public void HorizontalMove(float moveScalar) {
      var newMove = Vector2.right * curMoveSpeed * moveScalar;
      physics.Move(newMove);
      if (newMove.x != 0.0f) {
        Flip(newMove.x > 0.0);
      }
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
      if (grounded) {
        curMoveSpeed = (holdingDash && curDashTime < maxDashTime) ? dashSpeed : runSpeed;
        physics.SetVelocity(Vector2.up * jumpForce);
      } else if (!didDoubleJump) {
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
      initDashFacingDir = facingRight;
    }

    public virtual void OnDashHold() {
      holdingDash = true;
      curDashTime += Time.deltaTime;
      if (initDashFacingDir == facingRight && curDashTime < maxDashTime && physics.Grounded()) {
        animator.SetBool("Dashing", true);
        physics.Move((facingRight ? Vector2.right : Vector2.left) * dashSpeed);
      } else {
        animator.SetBool("Dashing", false);
      }
    }

    public virtual void OnDashRelease() {
      holdingDash = false;
      curDashTime = 0.0f;
      animator.SetBool("Dashing", false);
    }

    private void OnGroundedChange(bool grounded) {
      if (grounded) {
        didDoubleJump = false;
        curMoveSpeed = runSpeed;
      }
    }

    private void OnWallStickChange(bool stickingToWall) {
      if (stickingToWall) {
        curMoveSpeed = runSpeed;
        didDoubleJump = false;
      }
    }

    private void Awake() {
      physics = GetComponent<PlatformerPhysics2D>();
      rigidBody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();

      physics.RegisterModifier(wallStick);
      physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);

      wallStick.OnWallStickChange(OnWallStickChange);

      curMoveSpeed = runSpeed;
      facingRight = true;
    }

    private void Update() {
      animator.SetBool("Grounded", physics.Grounded());
      animator.SetFloat("xSpeed", Mathf.Abs(physics.TargetVelocity.x));
      animator.SetFloat("ySpeed", physics.CurrentVelocity.y);
      animator.SetBool("StickingToWall", wallStick.StickingToWall);

      spriteRenderer.flipY = physics.GravityDirection() == 1;
    }

    private void Flip(bool right) {
      if (right != facingRight) {
        var newScale = transform.localScale;
        newScale.x = right ? 1 : -1;
        transform.localScale = newScale;
      }
      facingRight = right;
    }
  }
}
