using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Dreamer))]
  public class PlayerController : MonoBehaviour {
    private Dreamer player;

    private void Awake() {
      player = GetComponent<Dreamer>();
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
    }
  }
}