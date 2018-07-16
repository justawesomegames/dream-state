using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovableCharacter : MonoBehaviour, IMovableCharacter {
  [SerializeField] private float maxSpeed = 10f;
  [SerializeField] private LayerMask groundLayer;

  private Rigidbody2D rigidBody;
  private BoxCollider2D collider;
  private Transform groundedPoint;
  private Vector2 groundedSize;
  private bool grounded;

  private void Awake() {
    rigidBody = GetComponent<Rigidbody2D>();
    collider = GetComponent<BoxCollider2D>();
    groundedPoint = transform.Find("GroundedPoint");
    groundedSize = new Vector2(collider.size.x * 0.98f, 0);
  }

  private void FixedUpdate() {
    // Determine if character is grounded
    grounded = false;
    var groundedPos = new Vector2(groundedPoint.position.x, groundedPoint.position.y);
    var colliders = Physics2D.OverlapBoxAll(groundedPos, groundedSize, 0, groundLayer);
    foreach(var col in colliders) {
      if (col.gameObject != gameObject) {
        grounded = true;
      }
    }
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