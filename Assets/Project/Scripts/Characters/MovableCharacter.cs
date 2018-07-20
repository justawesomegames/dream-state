using System;
using UnityEngine;
using Global;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class MovableCharacter : MonoBehaviour {
  [SerializeField] protected float runSpeed = 10f;
  [SerializeField] protected LayerMask groundLayer;
  [SerializeField] protected float jumpForce = 800f;
  [SerializeField] protected Transform groundedPoint;
  [SerializeField] protected float dashSpeed = 15f;
  [SerializeField] protected float maxDashTime = 0.4f;

  protected Rigidbody2D rigidBody;
  protected BoxCollider2D collider;
  protected Animator animator;
  protected Vector2 groundedSize;
  protected float curMoveScalar;
  protected bool jumping;
  protected bool releasedJump;
  protected Constants.FacingDirection curFacingDir;
  protected bool dashing;
  protected float dashTime;

  private bool _grounded;
  protected bool grounded {
    get { 
      return _grounded; 
    } 
    set {
      _grounded = value;
      animator.SetBool("Grounded", value);
    }
  }

  public virtual void Move(float moveScalar) {
    curMoveScalar = moveScalar;
  }

  public virtual void Dash(bool dash) {
    dashing = dash;
  }

  public virtual void Jump(bool jump) {
    jumping = jump;
  }

  protected virtual void Awake() {
    rigidBody = GetComponent<Rigidbody2D>();
    collider = GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();
    groundedSize = new Vector2(collider.size.x * 0.98f, 0);
    curFacingDir = Constants.FacingDirection.RIGHT;
  }

  protected virtual void FixedUpdate() {
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

  protected virtual void Update() {
    // Handle horizontal movement
    var newVelocity = new Vector2(runSpeed * curMoveScalar, rigidBody.velocity.y);

    // Handle dashing
    if (dashing) {
      dashTime += Time.deltaTime;
      if (dashTime < maxDashTime) {
        // Can continue dashing
        newVelocity.x = dashSpeed;
        animator.SetBool("Dashing", true);
        if (curFacingDir == Constants.FacingDirection.LEFT) {
          newVelocity.x *= -1;
        }
      } else {
        // Trying to dash, but already reached max dash time
        animator.SetBool("Dashing", false);
      }
    } else if(dashTime > 0) {
      dashTime = 0.0f;
      animator.SetBool("Dashing", false);
    }

    // Set movement in animator
    animator.SetFloat("Speed", Mathf.Abs(newVelocity.x));

    // Handle which direction character should be facing
    if (curFacingDir == Constants.FacingDirection.LEFT && curMoveScalar > 0.0f) {
      setFacingDir(Constants.FacingDirection.RIGHT);
    } else if (curFacingDir == Constants.FacingDirection.RIGHT && curMoveScalar < 0.0f) {
      setFacingDir(Constants.FacingDirection.LEFT);
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

  protected virtual void OnDrawGizmosSelected() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawSphere(groundedPoint.transform.position, 0.1f);
  }

  private bool shouldJump() {
    return grounded && jumping && releasedJump;
  }

  private void setFacingDir(Constants.FacingDirection dir) {
    Vector3 newScale = transform.localScale;
    switch (dir) {
      case Constants.FacingDirection.RIGHT:
        newScale.x = 1;
        break;
      case Constants.FacingDirection.LEFT:
        newScale.x = -1;
        break;
    }
    curFacingDir = dir;
    transform.localScale = newScale;
  }
}