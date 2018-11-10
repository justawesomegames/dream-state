using System;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Modifier encapsulates any discrete changes to velocity outside
    /// of the standard PlatformerPhysics handling.
    /// </summary>
    public abstract class Modifier : MonoBehaviour {
      protected PlatformerPhysics physics;

      /// <summary>
      /// Called once per physics update.
      /// Modify velocity after gravity and acceleration have been applied.
      /// </summary>
      /// <param name="v">Velocity after gravity and acceleration have been applied</param>
      /// <returns>New velocity</returns>
      public abstract Vector2 ModifyVelocity(Vector2 v);

      private void OnEnable() {
        physics = GetComponent<PlatformerPhysics>();
        physics.RegisterModifier(this);
      }

      private void OnDestroy() {
        physics.RemoveModifier(this);
      }

      private void OnDisable() {
        physics.RemoveModifier(this);
      }
    }
  }
}
