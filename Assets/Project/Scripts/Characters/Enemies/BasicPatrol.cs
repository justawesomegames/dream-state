using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  [RequireComponent(typeof(HorizontalMovement))]
  public class BasicPatrol : Character {
    [SerializeField] private bool changeDirOnHorizontalCollision = true;

    private HorizontalMovement horizontalMovement;

    public void Reverse() {
      SetFacingDir(curFacingDir == FacingDir.Right ? FacingDir.Left : FacingDir.Right);
    }

    private void Start() {
      horizontalMovement = GetComponent<HorizontalMovement>();
      if (changeDirOnHorizontalCollision) {
        physics.Collisions.Left.RegisterCallback(OnHorizontalCollision);
        physics.Collisions.Right.RegisterCallback(OnHorizontalCollision);
      }
    }

    private void Update() {
      horizontalMovement.Move(curFacingDir == FacingDir.Right ? 1 : -1);
    }

    private void OnHorizontalCollision(bool colliding) {
      if (!colliding) return;
      Reverse();
    }
  }
}