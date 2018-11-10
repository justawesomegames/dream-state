using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  [RequireComponent(typeof(AbilityManager))]
  [RequireComponent(typeof(HorizontalMovement))]
  [RequireComponent(typeof(Jump))]
  public class PlayerInputController : MonoBehaviour {
    private AbilityManager abilityManager;
    private ChargeDash chargeDash;
    private HorizontalMovement horizontalMovement;
    private Jump jump;

    private void Awake() {
      abilityManager = GetComponent<AbilityManager>();
      chargeDash = GetComponent<ChargeDash>();
      horizontalMovement = GetComponent<HorizontalMovement>();
      jump = GetComponent<Jump>();
      RegisterInputHandlers();
    }

    private void RegisterInputHandlers() {
      // Horizontal movement
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputAxes.Horizontal, (moveAmt) => {
        horizontalMovement.Move(moveAmt);
      });

      // Jumping
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Down, jump.OnJumpPress);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Hold, jump.OnJumpHold);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Up, jump.OnJumpRelease);

      // Dashing
      // InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Down, player.OnDashPress);
      // InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Hold, player.OnDashHold);
      // InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Up, player.OnDashRelease);

      // Attacking
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Down, abilityManager.OnAbilityDown);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Hold, abilityManager.OnAbilityHold);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Attack, InputButtonActions.Up, abilityManager.OnAbilityUp);

      // Pausing
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Pause, InputButtonActions.Down, GameManager.Instance.Pause);
      InputManager.Instance.RegisterEvent(InputContexts.Paused, InputButtons.Pause, InputButtonActions.Down, GameManager.Instance.Unpause);
    }
  }
}