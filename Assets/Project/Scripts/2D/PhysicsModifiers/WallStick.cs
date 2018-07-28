using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class WallStick : PhysicsObject2DModifier {
    #region Inspector
    [SerializeField] private float wallSlideSpeed = 5;
    [SerializeField] private float wallStickTimeBeforeSlide = 0.5f;
    [SerializeField] private Vector2 wallJump;
    #endregion

    #region Internal
    private bool stickingToWall = false;
    private float wallStickTime = 0.0f;
    #endregion

    public override Vector2 ModifyVelocity(Vector2 v) {
      if (v.x == 0.0f) return v;

      stickingToWall = (v.x < 0.0f && target.Collisions.Left.IsColliding() ||
                        v.x > 0.0f && target.Collisions.Right.IsColliding()) &&
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

    public override bool IsUnique() {
      return true;
    }
  }
}
