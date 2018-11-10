using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle horizontal movement
    /// </summary>
    [DisallowMultipleComponent]
    public class HorizontalMovement : Ability {
      [SerializeField] private float runSpeed = 10f;

      private float curMoveScalar;

      public override void Do() {
        physics.Move(Vector2.right * runSpeed * curMoveScalar);
      }

      public void Move(float moveScalar) {
        curMoveScalar = moveScalar;
      }
    }
  }
}