using System.Collections;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle dashing from a grounded state or in the air
    /// </summary>
    [DisallowMultipleComponent]
    public class Dash : Ability {
      public FacingDir DashDir { get { return dashDir; } }
      public bool HoldingDash { get { return holdingDash; } }
      public float DashSpeed { get { return dashSpeed; } }

      [SerializeField] private float dashSpeed = 20f;
      [SerializeField] private float dashTime = 0.3f;
      [SerializeField] private float dashCooldown = 0.3f;

      private HorizontalMovement horizontalMovement;
      private WallStick wallStick;
      private IFaceable faceable;
      private bool inDashCooldown;
      private float curDashTime;
      private FacingDir dashDir = FacingDir.Right;
      private bool holdingDash;
      private bool didAirDash;

      public override void ProcessAbility() {
        if (!Doing) {
          return;
        }

        physics.SetVelocityX(dashSpeed * (dashDir == FacingDir.Right ? 1 : -1));
        if (!physics.Grounded) {
          didAirDash = true;
          physics.SetVelocityY(0);
        }

        curDashTime += Time.deltaTime;
        if (curDashTime > dashTime) {
          StopDash();
        }
      }

      public void StartDash() {
        if (faceable == null) {
          Debug.LogWarning(string.Format("{0} does not have a facing direction, use DashInDirection", gameObject.name));
          return;
        }
        DoDash(faceable.CurFacingDir());
      }

      public void DashInDirection(FacingDir dir) {
        DoDash(dir);
      }

      public void HoldDash() {
        holdingDash = true;
      }

      public void OnDashRelease() {
        holdingDash = false;
        StopDash();
      }

      public void StopDash() {
        ChangeState(AbilityStates.Stopped);
      }

      protected override void Initialize() {
        horizontalMovement = GetComponent<HorizontalMovement>();
        wallStick = GetComponent<WallStick>();
        faceable = GetComponent<IFaceable>();
        physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);
        if (wallStick != null) {
          wallStick.OnStart(OnWallStickStart);
          wallStick.OnStop(OnWallStickStop);
        }
      }

      private void DoDash(FacingDir dir) {
        if (inDashCooldown) {
          return;
        }

        if (wallStick != null && wallStick.Doing) {
          return;
        }

        if (!physics.Grounded && didAirDash) {
          return;
        }

        StartCoroutine(DashCooldown());
        ChangeState(AbilityStates.Doing);
        curDashTime = 0.0f;
        dashDir = dir;
      }

      private void OnGroundedChange(bool grounded) {
        if (grounded) {
          didAirDash = false;
        } else if (!grounded) {
          if (horizontalMovement != null && Doing) {
            horizontalMovement.SetSpeed(dashSpeed);
            didAirDash = true;
          }
        }

        StopDash();
      }

      private void OnWallStickStart() {
        StopDash();
        didAirDash = false;
      }

      private void OnWallStickStop() {
        if (holdingDash) {
          didAirDash = true;
        }
      }

      private IEnumerator DashCooldown() {
        inDashCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        inDashCooldown = false;
      }
    }
  }
}