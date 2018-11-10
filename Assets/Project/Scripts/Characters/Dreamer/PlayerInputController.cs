using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  [RequireComponent(typeof(Dreamer))]
  [RequireComponent(typeof(AbilityManager))]
  public class PlayerInputController : MonoBehaviour {
    private Dreamer player;
    private AbilityManager abilityManager;
    private ChargeDash chargeDash;

    private void Awake() {
      player = GetComponent<Dreamer>();
      abilityManager = GetComponent<AbilityManager>();
      chargeDash = GetComponent<ChargeDash>();
      RegisterInputHandlers();
    }

    private void RegisterInputHandlers() {
      // Horizontal movement
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputAxes.Horizontal, (moveAmt) => {
        player.HorizontalMove(moveAmt);
      });

      // Jumping
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Down, player.OnJumpPress);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Hold, player.OnJumpHold);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Jump, InputButtonActions.Up, player.OnJumpRelease);

      // Dashing
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Down, player.OnDashPress);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Hold, player.OnDashHold);
      InputManager.Instance.RegisterEvent(InputContexts.Playing, InputButtons.Dash, InputButtonActions.Up, player.OnDashRelease);

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