using System;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    [DisallowMultipleComponent]
    public class WallStick : Modifier {
      public bool StickingToWall { get { return stickingToWall; } }

      [SerializeField] private float wallSlideSpeed = 7f;
      [SerializeField] private float wallStickTimeBeforeSlide = 0.3f;

      private bool stickingToWall = false;
      private float wallStickTime = 0.0f;
      private Action<bool> onWallStickChange;

      public override Vector2 ModifyVelocity(Vector2 v) {
        if (v.x == 0.0f) {
          SetWallstick(false);
          return v;
        }

        var gDir = physics.GravityDirection();
        var newWallStick = (physics.TargetVelocity.x < 0.0f && physics.Collisions.Left.IsColliding() ||
                            physics.TargetVelocity.x > 0.0f && physics.Collisions.Right.IsColliding()) &&
                            !physics.Grounded() &&
                            ((gDir == -1 && v.y <= 0.0f) || (gDir == 1 && v.y >= 0.0f));

        SetWallstick(newWallStick);
        if (!newWallStick) {
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

      public void OnWallStickChange(Action<bool> a) {
        onWallStickChange = a;
      }

      private void SetWallstick(bool newWallStick) {
        if (stickingToWall != newWallStick && onWallStickChange != null) {
          onWallStickChange(newWallStick);
        }
        stickingToWall = newWallStick;
      }
    }
  }
}
