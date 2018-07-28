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
      player.Jump(Input.GetButton(Global.Constants.Input.JUMP));
      // player.Dash(Input.GetButton(Global.Constants.Input.DASH));
    }
  }
}