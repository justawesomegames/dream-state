using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle sticking to a wall
    /// </summary>
    [DisallowMultipleComponent]
    public class WallStick : Ability {
      [SerializeField] private float wallSlideSpeed = -7f;
      [SerializeField] private float wallStickTimeBeforeSlide = 0.3f;

      private float wallStickTime = 0.0f;

      public override void ProcessAbility() {
        var newWallStick = (physics.TargetVelocity.x < 0.0f && physics.Collisions.Left.IsColliding() ||
                            physics.TargetVelocity.x > 0.0f && physics.Collisions.Right.IsColliding()) &&
                            !physics.Grounded && physics.CurrentVelocity.y <= 0.0f;

        if (!newWallStick) {
          ChangeState(AbilityStates.Stopped);
          wallStickTime = 0.0f;
          return;
        }

        ChangeState(AbilityStates.Doing);
        wallStickTime += Time.deltaTime;
        physics.SetVelocityY(wallStickTime < wallStickTimeBeforeSlide ? 0 : wallSlideSpeed);
      }
    }
  }
}