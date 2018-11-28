using UnityEngine;
using DreamState.Physics;
using DreamState.Abilities;

namespace DreamState {
  public class PlayerInputController : MonoBehaviour {
    private AbilityManager abilityManager;
    private HorizontalMovement horizontalMovement;
    private Jump jump;
    private Dash dash;

    public void EnablePlayerInput() {
      // Horizontal movement
      if (horizontalMovement != null) {
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputAxes.Horizontal);
      }

      // Jumping
      if (jump != null) {
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Down);
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Hold);
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Up);
      }

      // Dashing
      if (dash != null) {
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Down);
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Hold);
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Up);
      }

      // Attacking
      if (abilityManager != null) {
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Down);
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Hold);
        InputManager.Instance.EnableEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Up);
      }
    }

    public void DisablePlayerInput() {
      // Horizontal movement
      if (horizontalMovement != null) {
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputAxes.Horizontal);
      }

      // Jumping
      if (jump != null) {
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Down);
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Hold);
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Up);
      }

      // Dashing
      if (dash != null) {
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Down);
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Hold);
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Up);
      }

      // Attacking
      if (abilityManager != null) {
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Down);
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Hold);
        InputManager.Instance.DisableEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Up);
      }
    }

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
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Down, abilityManager.Cast<Blast>);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Hold, abilityManager.Charge<Blast>);
        InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Up, abilityManager.ReleaseCharge<Blast>);
      }

      // Pausing
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Pause, InputButtonActions.Down, GameManager.Instance.Pause);
      InputManager.Instance.RegisterEvent(InputContexts.Paused, InputButtons.Pause, InputButtonActions.Down, GameManager.Instance.Unpause);
    }
  }
}