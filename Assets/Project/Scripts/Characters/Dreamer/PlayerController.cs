using System;
using UnityEngine;
using Global;

[RequireComponent(typeof(Dreamer))]
public class PlayerController : MonoBehaviour {
  private Dreamer player;

  private void Awake() {
    player = GetComponent<Dreamer>();
  }

  private void Update() {
    player.Move(Input.GetAxis(Constants.Input.HORIZONTAL_AXIS));
    player.Jump(Input.GetButton(Constants.Input.JUMP));
    player.Dash(Input.GetButton(Constants.Input.DASH));
  }
}