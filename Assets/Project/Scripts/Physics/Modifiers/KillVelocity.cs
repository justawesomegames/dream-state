using System;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    [DisallowMultipleComponent]
    public class KillVelocity : Modifier {
      public override Vector2 ModifyVelocity(Vector2 v) {
        return Vector2.zero;
      }
    }
  }
}