using System;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    [DisallowMultipleComponent]
    public class ChargeDash : Modifier {
      public bool IsDashing { get { return isDashing; } }

      [SerializeField] private Vector2 dashVelocity = new Vector2(20, 0);
      [SerializeField] private float dashTime = 1.0f;

      private bool isDashing;
      private bool facingRight;
      private float curDashTime;

      public override Vector2 ModifyVelocity(Vector2 v) {
        if (!isDashing) {
          return v;
        }

        curDashTime += Time.deltaTime;
        if (curDashTime > dashTime) {
          isDashing = false;
        }

        v.x = facingRight ? dashVelocity.x : -dashVelocity.x;
        v.y = physics.Grounded() ? v.y : dashVelocity.y;
        return v;
      }

      public void SetDashing(bool dashing, bool right = true) {
        // Immediately kill dashing
        if (!dashing) {
          isDashing = false;
          return;
        }

        // Start dashing
        facingRight = right;
        curDashTime = 0.0f;
        isDashing = true;
      }
    }
  }
}