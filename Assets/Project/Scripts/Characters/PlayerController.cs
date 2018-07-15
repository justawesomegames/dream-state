using System;
using UnityEngine;

[RequireComponent(typeof(MovableCharacter))]
public class PlayerController : MonoBehaviour {
  private MovableCharacter player;

  private void Awake() {
    player = GetComponent<MovableCharacter>();
  }

  private void Update() {
    player.Jump(Input.GetButtonDown("Jump"));
    player.Dash(Input.GetButtonDown("Dash"));
    player.Move(Input.GetAxis("Horizontal"));
  }
}