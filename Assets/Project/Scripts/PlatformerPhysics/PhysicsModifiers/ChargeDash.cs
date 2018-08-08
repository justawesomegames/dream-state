using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class ChargeDash : PlatformerPhysics2DModifier {
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

      return new Vector2(facingRight ? dashVelocity.x : -dashVelocity.x, dashVelocity.y * -target.GravityDirection());
    }

    public override bool IsUniqueModifier() {
      return true;
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