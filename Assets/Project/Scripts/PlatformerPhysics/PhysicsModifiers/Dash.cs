using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class Dash : PlatformerPhysics2DModifier {
    public bool IsDashing { get { return isDashing; } }

    [SerializeField] protected Vector2 dashVelocity = new Vector2(20, 0);
    [SerializeField] protected float dashTime = 0.2f;
    [SerializeField] protected float dashTimeout = 0.2f;

    private bool isDashing;
    private bool facingRight;
    private float curDashTime;
    private float curDashTimeout;
    
    public override Vector2 ModifyVelocity(Vector2 v) {
      if (curDashTimeout < dashTimeout + dashTime) {
        curDashTimeout += Time.deltaTime;
      }

      if (!isDashing) {
        return v;
      }

      curDashTime += Time.deltaTime;
      if (curDashTime > dashTime) {
        isDashing = false;
      }

      return new Vector2(facingRight ? dashVelocity.x : -dashVelocity.x, dashVelocity.y * -target.GravityDirection());;
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

      // Prevent dashing before timeout is up
      if (curDashTimeout < dashTimeout + dashTime) {
        return;
      }

      // Start dashing
      facingRight = right;
      curDashTime = 0.0f;
      curDashTimeout = 0.0f;
      isDashing = true;
    }
  }
}
