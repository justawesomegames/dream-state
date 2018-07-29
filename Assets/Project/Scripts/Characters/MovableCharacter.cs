using DreamState.Global;
using System;
using UnityEngine;

namespace DreamState {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(PhysicsObject2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(SpriteRenderer))]
  public abstract class MovableCharacter : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] protected float runSpeed = 10f;
    [SerializeField] protected float dashSpeed = 15f;
    [SerializeField] protected float maxDashTime = 0.4f;
    [SerializeField] protected float jumpForce = 20f;
    [SerializeField] protected WallStick wallStick;

    protected PhysicsObject2D physics;
    protected Rigidbody2D rigidBody;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    private float curDashTime;
    private bool holdingDash;
    private float curMoveSpeed;

    public void HorizontalMove(float moveScalar) {
      var newMove = Vector2.right * curMoveSpeed * moveScalar;
      if (newMove.x != 0.0f) {
        physics.Move(newMove);
        spriteRenderer.flipX = newMove.x < 0.0f;
      }
    }

    public virtual void OnJumpPress() {
      if (physics.Grounded()) {
        curMoveSpeed = Dashing() ? dashSpeed : runSpeed;
        physics.SetVelocity(Vector2.up * jumpForce);
      } else if (wallStick.StickingToWall) {
        curMoveSpeed = holdingDash ? dashSpeed : runSpeed;
        wallStick.JumpOffWall(holdingDash);
      }
    }

    public virtual void OnJumpHold() { }

    public virtual void OnJumpRelease() {
      var gDir = physics.GravityDirection();
      if ((gDir == -1 && physics.CurrentVelocity.y > 0.0f || gDir == 1 && physics.CurrentVelocity.y < 0.0f)) {
        physics.SetVelocity(Vector2.zero);  
      }
    }

    public virtual void OnDashPress() { }

    public virtual void OnDashHold() {
      holdingDash = true;
      curDashTime += Time.deltaTime;
      if (Dashing() && physics.Grounded()) {
        animator.SetBool("Dashing", true);
        physics.Move((spriteRenderer.flipX ? Vector2.left : Vector2.right) * dashSpeed);
      } else {
        animator.SetBool("Dashing", false);
      }
    }

    public virtual void OnDashRelease() {
      holdingDash = false;
      curDashTime = 0.0f;
      animator.SetBool("Dashing", false);
    }

    protected bool Dashing() {
      return holdingDash && curDashTime < maxDashTime;
    }

    private void OnGroundedChange(bool newGrounded) {
      if (newGrounded) {
        curMoveSpeed = runSpeed;
      }
    }

    private void Awake() {
      physics = GetComponent<PhysicsObject2D>();
      rigidBody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();

      physics.RegisterModifier(wallStick);
      physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);

      curMoveSpeed = runSpeed;
    }

    private void Update() {
      animator.SetBool("Grounded", physics.Grounded());
      animator.SetFloat("xSpeed", Mathf.Abs(physics.CurrentVelocity.x));
      animator.SetFloat("ySpeed", physics.CurrentVelocity.y);
      animator.SetBool("StickingToWall", wallStick.StickingToWall);

      spriteRenderer.flipY = physics.GravityDirection() == 1;

      if (wallStick.StickingToWall) {
        curMoveSpeed = runSpeed;
      }
    }
  }
}
