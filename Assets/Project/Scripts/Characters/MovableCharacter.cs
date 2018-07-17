using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovableCharacter : MonoBehaviour, IMovableCharacter {
  [SerializeField] private float maxSpeed = 10f;
  [SerializeField] private LayerMask groundLayer;
  [SerializeField] private float maxJumpTime = 1.0f;
  [SerializeField] private float jumpForce = 100f;

  private Rigidbody2D rigidBody;
  private BoxCollider2D collider;
  private Transform groundedPoint;
  private Vector2 groundedSize;
  private float curMoveScalar;
  private bool grounded;
  private bool jumping;
  private bool jumped;
  private float airTime;
  private bool releasedJump;

  public void Move(float moveScalar) {
    curMoveScalar = moveScalar;
  }

  public void Dash(bool dash) {
    // TODO
  }

  public void Jump(bool jump) {
    jumping = jump;
  }

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

  private void Update() {
    // Handle horizontal movement
    var newVelocity = new Vector2(curMoveScalar * maxSpeed, rigidBody.velocity.y);

    // Keep track of airtime while jumping
    if (grounded) {
      jumped = false;
      airTime = 0.0f;
    } else {
      airTime += Time.deltaTime;
    }

    if (shouldJump()) {
      jumped = true;
    }

    // If character jump is within the max, handle jumping, otherwise let physics handle the zenith
    if (jumped && airTime < maxJumpTime) {
      if (jumping) {
        newVelocity.y = jumpForce;
      } else {
        newVelocity.y = 0.0f;
        jumped = false;
      }
    }

     // Keep track of if character released jump before landing to avoid bounce
    if (jumped && !grounded && jumping && releasedJump) {
      releasedJump = false;
    }
    else if (!releasedJump && !jumping) {
      releasedJump = true;
    }

    // TODO: Handle dashing

    // Set velocity
    rigidBody.velocity = newVelocity;
  }

  private bool shouldJump() {
    return grounded && jumping && !jumped && releasedJump;
  }
}