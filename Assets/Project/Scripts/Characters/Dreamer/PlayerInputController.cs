using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  public class PlayerInputController : MonoBehaviour {
    private AbilityManager abilityManager;
    private HorizontalMovement horizontalMovement;
    private Jump jump;
    private Dash dash;

    private void Awake() {
      abilityManager = GetComponent<AbilityManager>();
      horizontalMovement = GetComponent<HorizontalMovement>();
      jump = GetComponent<Jump>();
      dash = GetComponent<Dash>();

      RegisterInputHandlers();
    }

    private void RegisterInputHandlers() {
      // Horizontal movement
      if (horizontalMovement != null) {
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputAxes.Horizontal, (moveAmt) => {
          horizontalMovement.Move(moveAmt);
        });
      }

      // Jumping
      if (jump != null) {
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Down, jump.OnJumpPress);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Hold, jump.OnJumpHold);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Up, jump.OnJumpRelease);
      }

      // Dashing
      if (dash != null) {
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Down, dash.StartDash);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Hold, dash.HoldDash);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Up, dash.OnDashRelease);
      }

      // Attacking
      if (abilityManager != null) {
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Down, abilityManager.OnAbilityDown);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Hold, abilityManager.OnAbilityHold);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Up, abilityManager.OnAbilityUp);
      }

      // Pausing
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Pause, InputButtonActions.Down, GameManager.Instance.Pause);
      InputManager.Instance.RegisterEvent(InputContexts.Paused, InputButtons.Pause, InputButtonActions.Down, GameManager.Instance.Unpause);
    }
  }
}