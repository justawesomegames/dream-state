using System;
using UnityEngine;

public class MovableCharacter : MonoBehaviour, IMovableCharacter {
  [SerializeField] private float maxSpeed = 10f;

  private Rigidbody2D rigidBody;

  private void Awake() {
    rigidBody = GetComponent<Rigidbody2D>();
  }

  public void Move(float speedScalar) {
    rigidBody.velocity = new Vector2(speedScalar * maxSpeed, rigidBody.velocity.y);
  }

  public void Dash(bool dashing) {
    // TODO
  }

  public void Jump(bool jumping) {
    // TODO
  }

}