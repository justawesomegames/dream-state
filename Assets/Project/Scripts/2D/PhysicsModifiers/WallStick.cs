using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class WallStick : PhysicsObject2DModifier {
    public bool StickingToWall { get { return stickingToWall; } }

    [SerializeField] private float wallSlideSpeed = 7f;
    [SerializeField] private float wallStickTimeBeforeSlide = 0.3f;
    [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
    
    private bool stickingToWall = false;
    private float wallStickTime = 0.0f;
    
    public override Vector2 ModifyVelocity(Vector2 v) {
      if (v.x == 0.0f) {
        stickingToWall = false;
        return v; 
      }

      stickingToWall = (target.LastTargetVelocity.x < 0.0f && target.Collisions.Left.IsColliding() ||
                        target.LastTargetVelocity.x > 0.0f && target.Collisions.Right.IsColliding()) &&
                        !target.Grounded() &&
                        v.y <= 0.0f;

      if (!stickingToWall) {
        wallStickTime = 0.0f;
        return v;
      }

      wallStickTime += Time.deltaTime;
      if (wallStickTime < wallStickTimeBeforeSlide) {
        v.y = 0;
      } else {
        v.y = wallSlideSpeed * target.GravityDirection();
      }

      return v;
    }

    public override bool IsUniqueModifier() {
      return true;
    }

    public void JumpOffWall() {
      target.SetVelocity(-new Vector2(wallJump.x * Mathf.Sign(target.LastTargetVelocity.x), wallJump.y * target.GravityDirection()));
    }
  }
}
