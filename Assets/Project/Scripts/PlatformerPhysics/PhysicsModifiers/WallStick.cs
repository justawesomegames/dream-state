using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class WallStick : PlatformerPhysics2DModifier {
    public bool StickingToWall { get { return stickingToWall; } }

    [SerializeField] private float wallSlideSpeed = 7f;
    [SerializeField] private float wallStickTimeBeforeSlide = 0.3f;
    [SerializeField] private Vector2 wallJump = new Vector2(15f, 15f);
    [SerializeField] private Vector2 dashWallJump = new Vector2(20f, 20f);
    
    private bool stickingToWall = false;
    private float wallStickTime = 0.0f;
    private Action<bool> onWallStickChange;
    
    public override Vector2 ModifyVelocity(Vector2 v) {
      if (v.x == 0.0f) {
        SetWallstick(false);
        return v;
      }

      var gDir = target.GravityDirection();
      var newWallStick = (target.TargetVelocity.x < 0.0f && target.Collisions.Left.IsColliding() ||
                          target.TargetVelocity.x > 0.0f && target.Collisions.Right.IsColliding()) &&
                          !target.Grounded() &&
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

    public override bool IsUniqueModifier() {
      return true;
    }

    public void JumpOffWall(bool dashing) {
      var v = dashing ? dashWallJump : wallJump;
      target.SetVelocity(-new Vector2(v.x * Mathf.Sign(target.TargetVelocity.x), v.y * target.GravityDirection()));
      SetWallstick(false);
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
