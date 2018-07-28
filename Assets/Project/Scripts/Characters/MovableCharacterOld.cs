using UnityEngine;

namespace DreamState
{
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(BoxCollider2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(EdgeRaycastCollider))]
  public abstract class MovableCharacterOld : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] protected float runSpeed = 10f;
    [SerializeField] protected float jumpForce = 800f;
    [SerializeField] protected float dashSpeed = 15f;
    [SerializeField] protected float maxDashTime = 0.4f;

    [Header("Wall")]
    [SerializeField] protected float wallStickTime = 0.3f;
    [SerializeField] protected float wallFallSpeed = 6f;
    
    protected Rigidbody2D rigidBody;
    protected BoxCollider2D boxCollider;
    protected Animator animator;
    protected EdgeRaycastCollider raycastCollider;
    protected float curMoveScalar;
    protected bool jumping;
    protected bool releasedJump;
    protected bool releasedDash;
    protected Global.Constants.FacingDirection curFacingDir;
    protected bool dashing;
    protected float dashTime;
    protected float xVelocityInAir;
    protected float curWallStickTime;
    protected bool stickingToWall;

    protected bool grounded { get { return raycastCollider.Bottom.IsColliding(); } }

    protected virtual void OnAwake() {}
    protected virtual void OnUpdate() {}
    
    public void Move(float moveScalar) {
      curMoveScalar = moveScalar;
    }

    public void Dash(bool dash) {
      dashing = dash;
    }

    public void Jump(bool jump) {
      jumping = jump;
    }

    protected void Awake() {
      rigidBody = GetComponent<Rigidbody2D>();
      boxCollider = GetComponent<BoxCollider2D>();
      animator = GetComponent<Animator>();
      raycastCollider = GetComponent<EdgeRaycastCollider>();
      curFacingDir = Global.Constants.FacingDirection.RIGHT;
      xVelocityInAir = runSpeed;

      // Register collision events
      raycastCollider.Bottom.RegisterOnChange(groundedChange);

      OnAwake();
    }

    protected void FixedUpdate() {
      // Handle horizontal movement
      var newVelocity = new Vector2((grounded ? runSpeed : xVelocityInAir) * curMoveScalar, rigidBody.velocity.y);

      // Handle ground dashing
      if (dashing) {
        dashTime += Time.deltaTime;
        if (dashTime < maxDashTime && grounded && releasedDash) {
          // Can continue dashing
          newVelocity.x = dashSpeed;
          animator.SetBool("Dashing", true);
          if (curFacingDir == Global.Constants.FacingDirection.LEFT) {
            newVelocity.x *= -1;
          }
        } else {
          // Trying to dash, but already reached max dash time
          animator.SetBool("Dashing", false);
          releasedDash = false;
        }
      } else if(dashTime > 0) {
        dashTime = 0.0f;
        animator.SetBool("Dashing", false);
      }

      // Handle which direction character should be facing
      if (curFacingDir == Global.Constants.FacingDirection.LEFT && curMoveScalar > 0.0f) {
        setFacingDir(Global.Constants.FacingDirection.RIGHT);
      } else if (curFacingDir == Global.Constants.FacingDirection.RIGHT && curMoveScalar < 0.0f) {
        setFacingDir(Global.Constants.FacingDirection.LEFT);
      }

      // Immediately kill y-velocity if character wants to stop jumping
      if (!jumping && rigidBody.velocity.y > 0.0f) {
        newVelocity.y = 0.0f;
      }

      // Handle jumping from grounded
      if (grounded && jumping && releasedJump) {
        releasedJump = false;
        releasedDash = false;
        rigidBody.AddForce(new Vector2(0.0f, jumpForce));
      }

      // To avoid bounce, keep track of if character released jump before landing
      if (grounded && !jumping && !releasedJump) {
        releasedJump = true;
      }

      // Avoid dashing after being aerial when button held
      if (!grounded && dashing && releasedDash) {
        releasedDash = false;
      }
      else if (grounded && !dashing && !releasedDash) {
        releasedDash = true;
      }

      // Determine if character is sticking to a wall
      stickingToWall = ((raycastCollider.Left.IsColliding() && curMoveScalar < 0) || (raycastCollider.Right.IsColliding() && curMoveScalar > 0)) &&
                       rigidBody.velocity.y <= 0.0f && !grounded;
      animator.SetBool("StickingToWall", stickingToWall);

      if (stickingToWall) {
        curWallStickTime += Time.deltaTime;
        if (curWallStickTime < wallStickTime) {
          newVelocity.y = 0.0f;
        } else {
          newVelocity.y = -wallFallSpeed;
        }
      } else {
        curWallStickTime = 0.0f;
      }

      // Set new velocity
      rigidBody.velocity = newVelocity;
      animator.SetFloat("xSpeed", Mathf.Abs(newVelocity.x));
      animator.SetFloat("ySpeed", newVelocity.y);

      OnUpdate();
    }
  
    private void setFacingDir(Global.Constants.FacingDirection dir) {
      Vector3 newScale = transform.localScale;
      switch (dir) {
        case Global.Constants.FacingDirection.RIGHT:
          newScale.x = 1;
          break;
        case Global.Constants.FacingDirection.LEFT:
          newScale.x = -1;
          break;
      }
      curFacingDir = dir;
      transform.localScale = newScale;
    }

    private void groundedChange(bool newGrounded) {
      animator.SetBool("Grounded", newGrounded);
      if (!newGrounded) {
        xVelocityInAir = dashing && dashTime < maxDashTime ? dashSpeed : runSpeed;
      }
    }
  }
}