using System.Collections;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle jumping from a grounded state, double jumping,
    /// and jumping while stuck to a wall
    /// </summary>
    [DisallowMultipleComponent]
    public class Jump : Ability {
      [SerializeField] private float jumpForce = 20f;
      [SerializeField] private float doubleJumpForce = 15f;
      [SerializeField] private float jumpTolerance = 0.2f;
      [SerializeField] private bool canDoubleJump = false;

      [Header("Wall Jump")]
      [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
      [SerializeField] private Vector2 dashWallJump = new Vector2(20f, 20f);
      [Tooltip("Horizontal acceleration for <wallJumpFloatTime> after wall jump")]
      [SerializeField] private float wallJumpAirAcceleration = 2f;
      [SerializeField] private float dashWallJumpAirAcceleration = 4f;
      [Tooltip("Amount of time to apply acceleration after wall jump before re-enabling previous horizontal acceleration")]
      [SerializeField] protected float wallJumpFloatTime = 0.2f;

      private WallStick wallStick;
      private Dash dash;
      private HorizontalMovement horizontalMovement;
      private IFaceable faceable;
      private bool didDoubleJump;
      private bool canCoyoteJump;
      private bool wasWallStickingLeft, wasWallStickingRight;

      public virtual void OnJumpPress() {
        if (wasWallStickingLeft || wasWallStickingRight) {
          StartCoroutine(HandleWallJumpAcceleration());
          Vector2 vWallJump = wallJump;
          if (dash != null && dash.HoldingDash) {
            vWallJump = dashWallJump;
            if (horizontalMovement != null) {
              horizontalMovement.SetSpeed(dash.DashSpeed);
            }
          }
          var dir = Mathf.Sign(physics.TargetVelocity.x);
          if (faceable != null) {
            dir = faceable.CurFacingDir() == FacingDir.Right ? 1 : -1;
          }
          physics.SetVelocity(new Vector2(-(vWallJump.x * dir), vWallJump.y));
          wasWallStickingLeft = wasWallStickingRight = false;
          return;
        }

        var grounded = physics.Grounded;

        // Handle jumping from ground
        if (grounded || canCoyoteJump) {
          physics.SetVelocityY(jumpForce);
          return;
        }

        // Handle double jumping
        if (canDoubleJump && !grounded && !didDoubleJump) {
          didDoubleJump = true;
          physics.SetVelocityY(doubleJumpForce);
        }
      }

      public void OnJumpHold() { }

      public void OnJumpRelease() {
        var gDir = physics.GravityDirection();
        if ((gDir == -1 && physics.CurrentVelocity.y > 0.0f || gDir == 1 && physics.CurrentVelocity.y < 0.0f)) {
          physics.SetVelocityY(0);
        }

        // Prevent coyote jump if already jumped
        canCoyoteJump = false;
      }

      protected override void ProcessAbility() {
        if (wasWallStickingLeft && physics.TargetVelocity.x > 0.0f) {
          wasWallStickingLeft = false;
        }
        if (wasWallStickingRight && physics.TargetVelocity.x < 0.0f) {
          wasWallStickingRight = false;
        }
      }

      protected override void Initialize() {
        physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);
        wallStick = GetComponent<WallStick>();
        dash = GetComponent<Dash>();
        horizontalMovement = GetComponent<HorizontalMovement>();
        faceable = GetComponent<IFaceable>();
        if (wallStick != null) {
          wallStick.OnStart(OnWallStickStart);
        }
      }

      private void OnGroundedChange(bool grounded) {
        if (grounded) {
          didDoubleJump = false;
          wasWallStickingLeft = wasWallStickingRight = false;
        } else {
          StartCoroutine(CoyoteJump());
        }
      }

      private void OnWallStickStart() {
        didDoubleJump = false;
        if (physics.Collisions.Left.IsColliding()) {
          wasWallStickingLeft = true;
        } else {
          wasWallStickingRight = true;
        }
      }

      private IEnumerator CoyoteJump() {
        canCoyoteJump = true;
        yield return new WaitForSeconds(jumpTolerance);
        canCoyoteJump = false;
      }

      private IEnumerator HandleWallJumpAcceleration() {
        var initialAcceleration = physics.HorizontalAcceleration;
        physics.HorizontalAcceleration = (dash != null && dash.HoldingDash) ? dashWallJumpAirAcceleration : wallJumpAirAcceleration;

        yield return new WaitForSeconds(wallJumpFloatTime);

        physics.HorizontalAcceleration = initialAcceleration;
      }
    }
  }
}