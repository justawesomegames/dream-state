using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamState {
  /// <summary>
  /// PlatformerPhysics2D handles 2D physics separately from the Unity physics engine
  /// by utilizing raycasts for collision detection. By default, the PlatformerPhysics2D
  /// will handle gravity and translating the object with the given inputs.
  /// </summary>
  [DisallowMultipleComponent]
  [RequireComponent(typeof(BoxRaycastCollider2D))]
  public class PlatformerPhysics2D : MonoBehaviour {
    public Vector2 CurrentVelocity { get { return currentVelocity; } }
    public Vector2 TargetVelocity { get { return targetVelocity; } }
    public BoxRaycastCollider2D.CollisionInfo Collisions { get { return raycastCollider.Collisions; } }

    [SerializeField] private float gravityModifier = 1.0f;
    [SerializeField] private float horizontalAcceleration = 2f;
    [SerializeField] private float terminalVelocity = 20f;
    
    private BoxRaycastCollider2D raycastCollider;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private Dictionary<string, PlatformerPhysics2DModifier> modifiers = new Dictionary<string, PlatformerPhysics2DModifier>();
    private Dictionary<int, MovableObject2D> movables = new Dictionary<int, MovableObject2D>();

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

    /// <summary>
    /// Based on current gravity direction, check top or bottom collider for collision
    /// </summary>
    /// <returns>True if object is grounded, false otherwise</returns>
    public bool Grounded() {
      return GravityDirection() == -1 ? raycastCollider.Collisions.Bottom.IsColliding() : raycastCollider.Collisions.Top.IsColliding();
    }

    /// <summary>
    /// Get current gravity direction
    /// </summary>
    /// <returns>-1.0 for downwards, 1.0 for upwards</returns>
    public float GravityDirection() {
      return Mathf.Sign(Physics2D.gravity.y * gravityModifier);
    }

    /// <summary>
    /// Add a modifier to this physics object
    /// </summary>
    /// <param name="m">Modifier to add</param>
    /// <returns>Guid of modifier</returns>
    public string RegisterModifier(PlatformerPhysics2DModifier newModifier) {
      if (modifiers.ContainsKey(newModifier.Guid)) {
        Debug.LogWarning(String.Format("Modifier {0} is already registered!", newModifier.Guid));
        return String.Empty;
      }
      newModifier.SetTarget(this);
      modifiers.Add(newModifier.Guid, newModifier);
      return newModifier.Guid;
    }

    /// <summary>
    /// Remove a modifier from this physics object
    /// </summary>
    /// <param name="modifier">Modifier to remove</param>
    public void RemoveModifier(PlatformerPhysics2DModifier modifier) {
      if (!modifiers.ContainsKey(modifier.Guid)) {
        Debug.LogWarning(String.Format("Modifier {0} does not exist!", modifier.Guid));
        return;
      }
      modifier.SetTarget(null);
      modifiers.Remove(modifier.Guid);
    }

    private void Awake() {
      raycastCollider = GetComponent<BoxRaycastCollider2D>();

      var layerMask = raycastCollider.CollisionMask.value;
      if (layerMask == (layerMask | (1 << gameObject.layer))) {
        Debug.LogError(String.Format("GameObject {0} should not be on the same layer as the raycast collision mask!", gameObject.name));
      }
    }

    private void Update() {
      currentVelocity = CalculateNewVelocity();
      HandleNewMovement(currentVelocity * Time.deltaTime);
    }

    private void HandleNewMovement(Vector2 newMove) {
      var velocityAfterCollisions = raycastCollider.HandleNewVelocity(newMove);
      transform.Translate(velocityAfterCollisions);

      if (raycastCollider.Collisions.Top.IsColliding() || raycastCollider.Collisions.Bottom.IsColliding()) currentVelocity.y = 0;
      if (raycastCollider.Collisions.Left.IsColliding() || raycastCollider.Collisions.Right.IsColliding()) currentVelocity.x = 0;
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

      // Account for any modifiers
      foreach(var kvp in modifiers) {
        newVelocity = kvp.Value.ModifyVelocity(newVelocity);
      }

      return newVelocity;
    }

    private Vector2 HandleMovableObjects(Vector2 velocity) {
      if (movables.Count > 0) {
        var velocities = movables.Values.Select(m => m.Velocity);
        if (Grounded()) {
          velocity.x += velocities.Select(v => v.x).Max();
          velocity.y = velocities.Select(v => v.y).Max();
        }
      }
      return velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      var movable = collision.gameObject.GetComponent<MovableObject2D>();
      if (movable != null) {
        movables.Add(movable.GetInstanceID(), movable);
      }
    }

    private void OnCollisionExit2D(Collision2D collision) {
      var movable = collision.gameObject.GetComponent<MovableObject2D>();
      if (movable != null) {
        movables.Remove(movable.GetInstanceID());
      }
    }
  }
}