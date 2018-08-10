using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Dreamer))]
  [RequireComponent(typeof(AbilityManager))]
  public class PlayerInputController : MonoBehaviour {
    private Dreamer player;
    private AbilityManager abilityManager;

    private void Awake() {
      player = GetComponent<Dreamer>();
      abilityManager = GetComponent<AbilityManager>();
    }

    private void Update() {
      player.HorizontalMove(Input.GetAxis(Global.Constants.Input.HORIZONTAL_AXIS));

      if (Input.GetButtonDown(Global.Constants.Input.JUMP)) {
        player.OnJumpPress();
      }
      if (Input.GetButton(Global.Constants.Input.JUMP)) {
        player.OnJumpHold();
      }
      if (Input.GetButtonUp(Global.Constants.Input.JUMP)) {
        player.OnJumpRelease();
      }

      if (Input.GetButtonDown(Global.Constants.Input.DASH)) {
        player.OnDashPress();
      }
      if (Input.GetButton(Global.Constants.Input.DASH)) {
        player.OnDashHold();
      }
      if (Input.GetButtonUp(Global.Constants.Input.DASH)) {
        player.OnDashRelease();
      }

      if (Input.GetButtonDown(Global.Constants.Input.ATTACK)) {
        abilityManager.OnAbilityDown();
      }
      if (Input.GetButton(Global.Constants.Input.ATTACK)) {
        abilityManager.OnAbilityHold();
      }
      if (Input.GetButtonUp(Global.Constants.Input.ATTACK)) {
        abilityManager.OnAbilityUp();
      }
    }
  }
}