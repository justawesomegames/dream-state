using System;
using UnityEngine;

namespace DreamState {
  [Serializable]
  public class KillVelocity : PlatformerPhysics2DModifier {
    public override Vector2 ModifyVelocity(Vector2 v) {
      return Vector2.zero;
    }

    public override bool IsUniqueModifier() {
      return true;
    }
  }
}
