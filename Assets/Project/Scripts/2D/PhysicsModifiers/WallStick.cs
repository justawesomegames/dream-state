using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class WallStick : PhysicsObject2DModifier {
    public bool StickingToWall { get { return stickingToWall; } }

    [SerializeField] private float wallSlideSpeed = 7f;
    [SerializeField] private float wallStickTimeBeforeSlide = 0.3f;
    [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
    [SerializeField] private Vector2 dashWallJump = new Vector2(20f, 20f);
    
    private bool stickingToWall = false;
    private float wallStickTime = 0.0f;
    
    public override Vector2 ModifyVelocity(Vector2 v) {
      if (v.x == 0.0f) {
        stickingToWall = false;
        return v;
      }

      var gDir = target.GravityDirection();
      stickingToWall = (target.LastTargetVelocity.x < 0.0f && target.Collisions.Left.IsColliding() ||
                        target.LastTargetVelocity.x > 0.0f && target.Collisions.Right.IsColliding()) &&
                        !target.Grounded() &&
                        ((gDir == -1 && v.y <= 0.0f) || (gDir == 1 && v.y >= 0.0f));

      if (!stickingToWall) {
        wallStickTime = 0.0f;
        return v;
      }

      wallStickTime += Time.deltaTime;
      if (wallStickTime < wallStickTimeBeforeSlide) {
        v.y = 0;
      } else {
        v.y = wallSlideSpeed * gDir;
      }

      return v;
    }

    public override bool IsUniqueModifier() {
      return true;
    }

    public void JumpOffWall(bool dashing) {
      stickingToWall = false;
      var v = dashing ? dashWallJump : wallJump;
      target.SetVelocity(-new Vector2(v.x * Mathf.Sign(target.LastTargetVelocity.x), v.y * target.GravityDirection()));
    }
  }
}
