using UnityEngine;

namespace DreamState
{
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(BoxCollider2D))]
  [RequireComponent(typeof(Animator))]
  public abstract class MovableCharacter : MonoBehaviour {
    #region Visible
    [Header("Movement configuration")]
    [SerializeField] protected float runSpeed = 10f;
    [SerializeField] protected float jumpForce = 800f;
    [SerializeField] protected float dashSpeed = 15f;
    [SerializeField] protected float maxDashTime = 0.4f;

    [Header("Collision configuration")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform groundedPoint;
    #endregion

    #region Internal
    protected Rigidbody2D rigidBody;
    protected BoxCollider2D collider;
    protected Animator animator;
    protected Vector2 groundedSize;
    protected float curMoveScalar;
    protected bool jumping;
    protected bool releasedJump;
    protected Global.Constants.FacingDirection curFacingDir;
    protected bool dashing;
    protected float dashTime;
    protected float xVelocityInAir;

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
    #endregion

    #region Virtual functions
    protected virtual void OnAwake() {}
    protected virtual void OnUpdate() {}
    #endregion

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
      collider = GetComponent<BoxCollider2D>();
      animator = GetComponent<Animator>();
      groundedSize = new Vector2(collider.size.x * 0.98f, 0);
      curFacingDir = Global.Constants.FacingDirection.RIGHT;
      xVelocityInAir = runSpeed;

      OnAwake();
    }

    protected void FixedUpdate() {
      // Determine if character is grounded
      var newGrounded = false;
      var groundedPos = new Vector2(groundedPoint.position.x, groundedPoint.position.y);
      var colliders = Physics2D.OverlapBoxAll(groundedPos, groundedSize, 0, groundLayer);
      foreach(var col in colliders) {
        if (col.gameObject != gameObject) {
          newGrounded = true;
        }
      }
      if (newGrounded != grounded) {
        grounded = newGrounded;
      }
    }

    protected void Update() {
      // Handle horizontal movement
      var newVelocity = new Vector2((grounded ? runSpeed : xVelocityInAir) * curMoveScalar, rigidBody.velocity.y);

      // Handle ground dashing
      if (dashing && grounded) {
        dashTime += Time.deltaTime;
        if (dashTime < maxDashTime) {
          // Can continue dashing
          newVelocity.x = dashSpeed;
          animator.SetBool("Dashing", true);
          if (curFacingDir == Global.Constants.FacingDirection.LEFT) {
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
      if (curFacingDir == Global.Constants.FacingDirection.LEFT && curMoveScalar > 0.0f) {
        setFacingDir(Global.Constants.FacingDirection.RIGHT);
      } else if (curFacingDir == Global.Constants.FacingDirection.RIGHT && curMoveScalar < 0.0f) {
        setFacingDir(Global.Constants.FacingDirection.LEFT);
      }

      // Immediately kill y-velocity if character wants to stop jumping
      if (!jumping && rigidBody.velocity.y > 0.0f) {
        newVelocity.y = 0.0f;
      }

      // Set new velocity
      rigidBody.velocity = newVelocity;

      if (shouldJump()) {
        grounded = false;
        releasedJump = false;
        rigidBody.AddForce(new Vector2(0.0f, jumpForce));

        // If character is dashing while jumping, character can move at dash speed in air
        xVelocityInAir = dashing ? dashSpeed : runSpeed;
      }

      // To avoid bounce, keep track of if character released jump before landing
      if (!jumping && !releasedJump) {
        releasedJump = true;
      }

      OnUpdate();
    }

    protected void OnDrawGizmosSelected() {
      // Draw a gizmo at the grounded position since it doesn't easily render otherwise
      Gizmos.DrawSphere(groundedPoint.transform.position, 0.1f);
    }

    private bool shouldJump() {
      return grounded && jumping && releasedJump;
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
  }
}