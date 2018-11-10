using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle sticking to a wall
    /// </summary>
    [DisallowMultipleComponent]
    public class StickToWall : Ability {
      public bool StickingToWall { get { return stickingToWall; } }

      [SerializeField] private float wallSlideSpeed = -7f;
      [SerializeField] private float wallStickTimeBeforeSlide = 0.3f;

      private bool stickingToWall = false;
      private float wallStickTime = 0.0f;
      private List<Action<bool>> callbacks = new List<Action<bool>>();

      public override void Do() {
        var newWallStick = (physics.TargetVelocity.x < 0.0f && physics.Collisions.Left.IsColliding() ||
                            physics.TargetVelocity.x > 0.0f && physics.Collisions.Right.IsColliding()) &&
                            !physics.Grounded() && physics.CurrentVelocity.y <= 0.0f;

        SetWallstick(newWallStick);
        if (!newWallStick) {
          return;
        }

        wallStickTime += Time.deltaTime;
        physics.SetVelocityY(wallStickTime < wallStickTimeBeforeSlide ? 0 : wallSlideSpeed);
      }

      public void OnWallStickChange(Action<bool> callback) {
        callbacks.Add(callback);
      }

      private void SetWallstick(bool newWallStick) {
        if (stickingToWall != newWallStick) {
          callbacks.ForEach(c => c(newWallStick));
        }
        stickingToWall = newWallStick;
        if (!stickingToWall) {
          wallStickTime = 0.0f;
        }
      }
    }
  }
}