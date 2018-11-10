using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// PlatformerPhysics handles 2D physics separately from the Unity physics engine
    /// by utilizing raycasts for collision detection.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformerPhysics : MonoBehaviour {
      [HideInInspector]
      public struct RaycastOrigins {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
      }

      [HideInInspector]
      public BoxCollisionInfo Collisions {
        get {
          if (collisions == null) collisions = new BoxCollisionInfo(this);
          return collisions;
        }
      }
      [HideInInspector] public LayerMask CollisionMask { get { return collisionMask; } }

      [SerializeField] private float gravityModifier = 1.0f;
      [SerializeField] private float horizontalAcceleration = 50f;
      [SerializeField] private float terminalVelocity = 20f;

      [Header("Collision Configuration")]
      [SerializeField] private LayerMask collisionMask;
      [SerializeField] private float skinWidth = 0.05f;
      [SerializeField] private float distanceBetweenRays = 0.25f;
      [SerializeField] private int horizontalRayCount = 3;
      [SerializeField] private int verticalRayCount = 3;
      [SerializeField] private float maxSlopeAngleDegrees = 45;

      public Vector2 CurrentVelocity { get { return currentVelocity; } }
      public Vector2 TargetVelocity { get { return targetVelocity; } }
      public float SkinWidth { get { return skinWidth; } }
      public float HorizontalAcceleration {
        get { return horizontalAcceleration; }
        set { horizontalAcceleration = value; }
      }

      private BoxCollisionInfo collisions;
      private BoxCollider2D boxCollider;
      private RaycastOrigins raycastOrigins;
      private float horizontalRaySpacing;
      private float verticalRaySpacing;
      private SpriteRenderer spriteRenderer;
      private bool facingRight = true;
      private Vector2 currentVelocity;
      private Vector2 targetVelocity;
      private List<Ability> abilities = new List<Ability>();
      private IFaceable faceable;

      /// <summary>
      /// Move object at a speed
      /// </summary>
      /// <param name="moveAmount">Speed to move object</param>
      /// <param name="instantX">Instantly set current x-velocity?</param>
      /// <param name="instantY">Instantly set current y-velocity?</param>
      public void Move(Vector2 moveAmount, bool instantX = false, bool instantY = false) {
        targetVelocity = moveAmount;
        if (instantX) {
          currentVelocity.x = moveAmount.x;
        }
        if (instantY) {
          currentVelocity.y = moveAmount.y;
        }
      }

      /// <summary>
      /// Immediately move the object
      /// </summary>
      /// <param name="moveAmount">Amount to move</param>
      public void MoveNow(Vector2 moveAmount) {
        SetVelocity(moveAmount);
        HandleNewMovement(moveAmount);
      }

      /// <summary>
      /// Immediately change velocity
      /// </summary>
      /// <param name="velocity">Velocity to set to</param>
      public void SetVelocity(Vector2 velocity) {
        SetVelocityX(velocity.x);
        SetVelocityY(velocity.y);
      }

      /// <summary>
      /// Immediately change x-velocity
      /// </summary>
      /// <param name="newX">New x velocity</param>
      public void SetVelocityX(float newX) {
        currentVelocity.x = newX;
      }

      /// <summary>
      /// Immediately change y-velocity
      /// </summary>
      /// <param name="newY">New y velocity</param>
      public void SetVelocityY(float newY) {
        currentVelocity.y = newY * -GravityDirection();
      }

      public void SetHorizontalAcceleration(float newAcceleration) {
        horizontalAcceleration = newAcceleration;
      }

      /// <summary>
      /// Based on current gravity direction, check top or bottom collider for collision
      /// </summary>
      /// <returns>True if object is grounded, false otherwise</returns>
      public bool Grounded() {
        return GravityDirection() == -1 ? collisions.Bottom.IsColliding() : collisions.Top.IsColliding();
      }

      /// <summary>
      /// Get current gravity direction
      /// </summary>
      /// <returns>-1.0 for downwards, 1.0 for upwards</returns>
      public float GravityDirection() {
        return Mathf.Sign(Physics2D.gravity.y * gravityModifier);
      }

      /// <summary>
      /// Adds an ability to this physics object
      /// </summary>
      /// <param name="ability"></param>
      public void RegisterAbility(Ability ability) {
        abilities.Add(ability);
      }

      /// <summary>
      /// Removes an ability from this physics object
      /// </summary>
      /// <param name="ability"></param>
      public void RemoveAbility(Ability ability) {
        abilities.Remove(ability);
      }

      private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        faceable = GetComponent<IFaceable>();

        var layerMask = collisionMask.value;
        if (layerMask == (layerMask | (1 << gameObject.layer))) {
          Debug.LogError(String.Format("GameObject {0} should not be on the same layer as the raycast collision mask!", gameObject.name));
        }
      }

      private void Start() {
        CalculateRaySpacing();
      }

      private void Update() {
        // First, let the physical world act on the object
        currentVelocity = CalculateNewVelocity();

        // Then account for any abilities this physical object may have
        abilities.ForEach(a => a.Do());

        // Finally, actually move the object
        HandleNewMovement(currentVelocity * Time.deltaTime);
      }

      /// <summary>
      /// Calculate spacing between rays
      /// </summary>
      private void CalculateRaySpacing() {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
      }

      /// <summary>
      /// Updates the origin of each ray
      /// </summary>
      private void UpdateRaycastOrigins() {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
      }

      private void HandleNewMovement(Vector2 newMove) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.LastMoveAmount = newMove;

        if (newMove.y < 0) {
          newMove = DescendSlope(newMove);
        }

        if (newMove.x != 0) {
          collisions.LastFacingDir = (int)Mathf.Sign(newMove.x);
        }

        newMove = HorizontalMove(newMove);

        if (newMove.y != 0) {
          newMove = VerticalMove(newMove);
        }

        transform.Translate(newMove);

        if (collisions.Top.IsColliding() || collisions.Bottom.IsColliding()) currentVelocity.y = 0;
        if (collisions.Left.IsColliding() || collisions.Right.IsColliding()) currentVelocity.x = 0;

        collisions.FinishColliding();
      }

      /// <summary>
      /// Calculate velocity based on current state, intended state, and outside forces
      /// </summary>
      /// <returns>New velocity</returns>
      private Vector2 CalculateNewVelocity() {
        Vector2 newVelocity = currentVelocity;

        // Apply gravity
        newVelocity += Physics2D.gravity * gravityModifier * Time.deltaTime;

        // Handle terminal velocity
        var gravityDir = GravityDirection();
        if (newVelocity.y < -terminalVelocity && gravityDir == -1) {
          newVelocity.y = -terminalVelocity;
        } else if (newVelocity.y > terminalVelocity && gravityDir == 1) {
          newVelocity.y = terminalVelocity;
        }

        // Horizontal velocity
        if (currentVelocity.x < targetVelocity.x) {
          newVelocity.x = Mathf.Min(newVelocity.x + horizontalAcceleration, targetVelocity.x);
        } else if (currentVelocity.x > targetVelocity.x) {
          newVelocity.x = Mathf.Max(newVelocity.x - horizontalAcceleration, targetVelocity.x);
        }

        return newVelocity;
      }

      private Vector2 VerticalMove(Vector2 moveVector) {
        var yDir = moveVector.y > 0 ? 1 : -1;

        var rayLength = Mathf.Abs(moveVector.y) + skinWidth;
        if (Mathf.Abs(moveVector.y) < skinWidth) rayLength = 2 * skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
          var rayOrigin = yDir == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
          rayOrigin += Vector2.right * (verticalRaySpacing * i + moveVector.x);
          var hit = Physics2D.Raycast(rayOrigin, Vector2.up * yDir, rayLength, collisionMask);

          Debug.DrawRay(rayOrigin, Vector2.up * yDir);

          // Check if there was actually a collision
          if (!hit) continue;

          // Let collision affect other ray collision detection
          moveVector.y = (hit.distance - skinWidth) * yDir;

          // Reduce ray length to avoid unnecessary collisions
          rayLength = hit.distance;

          if (collisions.ClimbingSlope) {
            moveVector.x = moveVector.y / Mathf.Tan(collisions.CurSlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveVector.y);
          }

          // Add collision to the appropriate collider
          if (yDir == -1) {
            Collisions.Bottom.AddHit(hit);
          } else {
            Collisions.Top.AddHit(hit);
          }
        }

        if (collisions.ClimbingSlope) {
          var xDir = Mathf.Sign(moveVector.x);
          rayLength = Mathf.Abs(moveVector.x) + skinWidth;
          var rayOrigin = ((xDir == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveVector.y;
          var hit = Physics2D.Raycast(rayOrigin, Vector2.right * xDir, rayLength, collisionMask);
          if (hit) {
            var angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle != collisions.CurSlopeAngle) {
              moveVector.x = (hit.distance - skinWidth) * xDir;
              collisions.CurSlopeAngle = angle;
            }
          }
        }

        return moveVector;
      }

      private Vector2 HorizontalMove(Vector2 moveVector) {
        var xDir = collisions.LastFacingDir;

        var rayLength = Mathf.Abs(moveVector.x) + skinWidth;
        if (Mathf.Abs(moveVector.x) < skinWidth) rayLength = 2 * skinWidth;

        for (int i = 0; i < horizontalRayCount; i++) {
          var rayOrigin = xDir == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
          rayOrigin += Vector2.up * (horizontalRaySpacing * i);
          var hit = Physics2D.Raycast(rayOrigin, Vector2.right * xDir, rayLength, collisionMask);

          Debug.DrawRay(rayOrigin, Vector2.right * xDir);

          // Check if there was actually a collision
          if (!hit) continue;
          if (hit.distance == 0) continue;

          var angle = Mathf.Abs(Vector2.Angle(hit.normal, Vector2.up));

          if (i == 0 && angle <= maxSlopeAngleDegrees) {
            if (collisions.DescendingSlope) {
              collisions.DescendingSlope = false;
              moveVector = collisions.LastMoveAmount;
            }

            var distanceToSlope = 0f;
            if (angle != collisions.LastSlopeAngle) {
              distanceToSlope = hit.distance - skinWidth;
              moveVector.x -= distanceToSlope * xDir;
            }

            // Climb slope
            var angleInRads = angle * Mathf.Deg2Rad;
            var absMoveX = Mathf.Abs(moveVector.x);
            var heightToClimb = Mathf.Sin(angleInRads) * absMoveX;
            if (moveVector.y <= heightToClimb) {
              moveVector.y = heightToClimb;
              moveVector.x = Mathf.Cos(angleInRads) * absMoveX * Mathf.Sign(moveVector.x);
              collisions.CurSlopeAngle = angle;
              collisions.ClimbingSlope = true;
              Collisions.Bottom.AddHit(hit);
            }

            moveVector.x += distanceToSlope * xDir;
          }

          if (!collisions.ClimbingSlope || angle > maxSlopeAngleDegrees) {
            // Let collision affect other ray collision detection
            moveVector.x = (hit.distance - skinWidth) * xDir;
            // Reduce ray length to avoid unnecessary collisions
            rayLength = hit.distance;

            if (collisions.ClimbingSlope) {
              moveVector.y = Mathf.Tan(collisions.CurSlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveVector.x);
            }

            // Add collision to the appropriate collider
            if (xDir == -1) {
              Collisions.Left.AddHit(hit);
            } else {
              Collisions.Right.AddHit(hit);
            }
          }
        }

        return moveVector;
      }

      private Vector2 DescendSlope(Vector2 moveVector) {
        var maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveVector.y) + skinWidth, collisionMask);
        var maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveVector.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
          moveVector = SlideDownMaxSlope(maxSlopeHitLeft, moveVector);
          moveVector = SlideDownMaxSlope(maxSlopeHitRight, moveVector);
        }

        var xDir = Mathf.Sign(moveVector.x);
        var rayOrigin = (xDir == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (!hit) return moveVector;

        var angle = Vector2.Angle(hit.normal, Vector2.up);
        if (angle != 0 && angle <= maxSlopeAngleDegrees && hit.distance - skinWidth <= Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Abs(moveVector.x)) {
          var absX = Mathf.Abs(moveVector.x);
          var descendMoveY = Mathf.Sin(angle * Mathf.Deg2Rad) * absX;
          moveVector.x = Mathf.Cos(angle * Mathf.Deg2Rad) * absX * xDir;
          moveVector.y -= descendMoveY;
          collisions.CurSlopeAngle = angle;
          collisions.DescendingSlope = true;
          Collisions.Bottom.AddHit(hit);
        }

        return moveVector;
      }

      private Vector2 SlideDownMaxSlope(RaycastHit2D hit, Vector2 moveVector) {
        if (!hit) return moveVector;

        var angle = Vector2.Angle(hit.normal, Vector2.up);
        if (angle > maxSlopeAngleDegrees) {
          moveVector.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveVector.y) - hit.distance) / Mathf.Tan(angle * Mathf.Deg2Rad);
          collisions.CurSlopeAngle = angle;
          collisions.SlidingDownMaxSlope = true;
        }

        return moveVector;
      }
    }
  }
}