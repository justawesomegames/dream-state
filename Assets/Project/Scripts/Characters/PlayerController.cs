using System;
using UnityEngine;

[RequireComponent(typeof(MovableCharacter))]
public class PlayerController : MonoBehaviour {
  private MovableCharacter player;

  private void Awake() {
    player = GetComponent<MovableCharacter>();
  }

  private void Update() {
    player.Jump(Input.GetButton("Jump"));
    player.Dash(Input.GetButton("Dash"));
    player.Move(Input.GetAxis("Horizontal"));
  }
}