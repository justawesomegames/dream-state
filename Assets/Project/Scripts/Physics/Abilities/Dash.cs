using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle dashing while on the ground
    /// </summary>
    [DisallowMultipleComponent]
    public class GroundDash : Ability {
      [SerializeField] private float dashSpeed = 20f;
      [SerializeField] private float dashTime = 0.2f;
      [SerializeField] private float dashTimeout = 0.2f;

      public override void Do() {
        // TODO
      }

      // public void OnDashPress() {
      //   if (didDashInAir || (wallStick != null && wallStick.StickingToWall)) {
      //     return;
      //   }
      //   dash.SetDashing(true, FacingRight());
      //   if (!physics.Grounded()) {
      //     didDashInAir = true;
      //   }
      // }

      // public void OnDashHold() {
      //   curChargeDashTime += Time.deltaTime;
      // }

      // public void OnDashRelease() {
      //   if (chargeDash != null && curChargeDashTime > chargeDashTime) {
      //     chargeDash.SetDashing(true, FacingRight());

      //     // Prevent double jump out of a charge dash
      //     didDoubleJump = true;
      //   }
      //   curChargeDashTime = 0.0f;
      // }
    }
  }
}