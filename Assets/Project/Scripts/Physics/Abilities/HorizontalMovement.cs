using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// Handle horizontal movement
    /// </summary>
    [DisallowMultipleComponent]
    public class HorizontalMovement : Ability {
      [SerializeField] private float moveSpeed = 10f;

      private float curMoveScalar;
      private float curMoveSpeed;
      private Dash dash;
      private WallStick wallStick;

      public override void ProcessAbility() {
        if (dash != null && dash.Doing) {
          if (curMoveScalar == 0.0f ||
              dash.DashDir == FacingDir.Right && curMoveScalar > 0.0f ||
              dash.DashDir == FacingDir.Left && curMoveScalar < 0.0f) {
            return;
          }
          dash.StopDash();
        }

        physics.Move(Vector2.right * curMoveSpeed * curMoveScalar);
      }

      public void Move(float moveScalar) {
        if (moveScalar != 0.0f && curMoveScalar == 0.0f) {
          ChangeState(AbilityStates.Doing);
        } else if (moveScalar == 0.0f && curMoveScalar != 0.0f) {
          ChangeState(AbilityStates.Stopped);
        }
        curMoveScalar = moveScalar;
      }

      public void SetSpeed(float moveSpeed) {
        curMoveSpeed = moveSpeed;
      }

      protected override void Initialize() {
        curMoveSpeed = moveSpeed;
        dash = GetComponent<Dash>();
        wallStick = GetComponent<WallStick>();
        physics.Collisions.Bottom.RegisterCallback(OnGroundedChange);
        if (wallStick != null) {
          wallStick.OnStart(OnWallStickStart);
        }
      }

      private void OnGroundedChange(bool grounded) {
        if (grounded) {
          curMoveSpeed = moveSpeed;
        }
      }

      private void OnWallStickStart() {
        curMoveSpeed = moveSpeed;
      }
    }
  }
}