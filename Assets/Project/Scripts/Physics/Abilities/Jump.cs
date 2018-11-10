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
      [SerializeField] private float jumpTolerance = 0.2f;
      [SerializeField] private bool canDoubleJump = false;

      [Header("Wall Jump")]
      [SerializeField] private float doubleJumpForce = 15f;
      [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
      [Tooltip("Horizontal acceleration for <wallJumpFloatTime> after wall jump")]
      [SerializeField]
      private float wallJumpAirAcceleration = 2f;
      [Tooltip("Amount of time to apply acceleration after wall jump before re-enabling previous horizontal acceleration")]
      [SerializeField]
      protected float wallJumpFloatTime = 0.2f;

      private StickToWall wallStick;
      private bool didDoubleJump;
      private bool canCoyoteJump;

      public override void Initialize() {
        physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);
        wallStick = GetComponent<StickToWall>();
        if (wallStick != null) {
          wallStick.OnWallStickChange(OnWallStickChange);
        }
      }

      public override void Do() { }

      public virtual void OnJumpPress() {
        if (wallStick != null && wallStick.StickingToWall) {
          StartCoroutine(HandleWallJumpAcceleration());
          physics.SetVelocity(new Vector2(-(wallJump.x * Mathf.Sign(physics.TargetVelocity.x)), wallJump.y));
          return;
        }

        var grounded = physics.Grounded();

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

      private void OnGroundedChange(bool grounded) {
        if (grounded) {
          didDoubleJump = false;
        } else {
          StartCoroutine(CoyoteJump());
        }
      }

      private void OnWallStickChange(bool stickingToWall) {
        if (stickingToWall) {
          didDoubleJump = false;
        } else {
          StartCoroutine(CoyoteJump());
        }
      }

      private IEnumerator CoyoteJump() {
        canCoyoteJump = true;
        yield return new WaitForSeconds(jumpTolerance);
        canCoyoteJump = false;
      }

      private IEnumerator HandleWallJumpAcceleration() {
        var initialAcceleration = physics.HorizontalAcceleration;
        physics.HorizontalAcceleration = wallJumpAirAcceleration;

        yield return new WaitForSeconds(wallJumpFloatTime);

        physics.HorizontalAcceleration = initialAcceleration;
      }
    }
  }
}