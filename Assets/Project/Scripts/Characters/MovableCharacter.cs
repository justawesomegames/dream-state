using System;
using UnityEngine;

public enum FacingDirection {
  LEFT,
  RIGHT
}

[RequireComponent(typeof(BoxCollider2D))]
public class MovableCharacter : MonoBehaviour, IMovableCharacter {
  [SerializeField] private float maxSpeed = 10f;
  [SerializeField] private LayerMask groundLayer;
  [SerializeField] private float jumpForce = 800f;

  private Rigidbody2D rigidBody;
  private BoxCollider2D collider;
  private Animator animator;
  private Transform groundedPoint;
  private Vector2 groundedSize;
  private float curMoveScalar;
  private bool grounded;
  private bool jumping;
  private float airTime;
  private bool releasedJump;
  private FacingDirection curFacingDir;

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
    animator = GetComponent<Animator>();
    groundedPoint = transform.Find("GroundedPoint");
    groundedSize = new Vector2(collider.size.x * 0.98f, 0);
    curFacingDir = FacingDirection.RIGHT;
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

    // Set movement in animator
    animator.SetFloat("Speed", Mathf.Abs(curMoveScalar));

    // Handle which direction character should be facing
    if (curFacingDir == FacingDirection.LEFT && curMoveScalar > 0.0f) {
      setFacingDir(FacingDirection.RIGHT);
    } else if (curFacingDir == FacingDirection.RIGHT && curMoveScalar < 0.0f) {
      setFacingDir(FacingDirection.LEFT);
    }

    // Immediately kill vertical movement if character wants to stop jumping
    if (!jumping && rigidBody.velocity.y > 0.0f) {
      newVelocity.y = 0.0f;
    }

    // Set velocity
    rigidBody.velocity = newVelocity;

    if (shouldJump()) {
      grounded = false;
      rigidBody.AddForce(new Vector2(0.0f, jumpForce));
    }

     // To avoid bounce, keep track of if character released jump before landing
    if (!grounded && jumping && releasedJump) {
      releasedJump = false;
    }
    else if (!releasedJump && !jumping) {
      releasedJump = true;
    }

    // TODO: Handle dashing
  }

  private bool shouldJump() {
    return grounded && jumping && releasedJump;
  }

  private void setFacingDir(FacingDirection dir) {
    Vector3 newScale = transform.localScale;
    switch (dir) {
      case FacingDirection.RIGHT:
        newScale.x = 1;
        break;
      case FacingDirection.LEFT:
        newScale.x = -1;
        break;
    }
    curFacingDir = dir;
    transform.localScale = newScale;
  }
}