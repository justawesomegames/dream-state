using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamState {
  /// <summary>
  /// PhysicsObject2D handles 2D physics separately from the Unity physics engine
  /// by utilizing raycasts for collision detection. By default, the PhysicsObject2D
  /// will handle gravity and translating the object with the given inputs.
  /// </summary>
  [RequireComponent(typeof(BoxRaycastCollider2D))]
  public class PhysicsObject2D : MonoBehaviour {
    #region Public
    public Vector2 CurrentVelocity { get { return currentVelocity; } }
    public BoxRaycastCollider2D.CollisionInfo Collisions { get { return raycastCollider.Collisions; } }
    #endregion

    #region Inspector
    [SerializeField] private float gravityModifier = 1.0f;
    [SerializeField] private float horizontalAcceleration = 0.3f;
    [SerializeField] private float terminalVelocity = 0.3f;
    #endregion

    #region Internal
    private BoxRaycastCollider2D raycastCollider;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private List<PhysicsObject2DModifier> modifiers = new List<PhysicsObject2DModifier>();
    #endregion

    /// <summary>
    /// Move object at a speed
    /// </summary>
    /// <param name="moveAmount">Speed to move object</param>
    public void Move(Vector2 moveAmount) {
      targetVelocity = moveAmount;
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
    /// Immediately apply a vertical force to the object
    /// </summary>
    /// <param name="force">Force to apply</param>
    public void Jump(float force) {
      currentVelocity.y = force * (GravityDirection() * -1);
    }

    /// <summary>
    /// Add a modifier to this physics object
    /// </summary>
    /// <param name="m">Modifier to add</param>
    public void RegisterModifier(PhysicsObject2DModifier newModifier) {
      if (modifiers.Exists(m => m.GetGuid() == newModifier.GetGuid())) {
        Debug.LogWarning(String.Format("Modifier {0} is already registered!", newModifier.GetGuid()));
        return;
      }
      newModifier.SetTarget(this);
      modifiers.Add(newModifier);
    }

    /// <summary>
    /// Remove a modifier from this physics object
    /// </summary>
    /// <param name="modifier">Modifier to remove</param>
    public void RemoveModifier(PhysicsObject2DModifier modifier) {
      if (!modifiers.Exists(m => m.GetGuid() == modifier.GetGuid())) {
        Debug.LogWarning(String.Format("Modifier {0} does not exist!", modifier.GetGuid()));
        return;
      }
      modifier.SetTarget(null);
      modifiers.Remove(modifier);
    }

    private void Awake() {
      raycastCollider = GetComponent<BoxRaycastCollider2D>();
    }

    private void Update() {
      currentVelocity = CalculateNewVelocity();

      // Account for any modifiers
      modifiers.ForEach(m => currentVelocity = m.ModifyVelocity(currentVelocity));

      var normalized = currentVelocity * Time.deltaTime;
      raycastCollider.UpdateCollisions(normalized);
      normalized = AdjustedVelocity(normalized);
      transform.Translate(normalized);
      currentVelocity = ZeroOutCollisions(currentVelocity);

      // TODO: Handle slopes
      // TODO: Handle moving platforms

      // Reset target velocity
      targetVelocity = Vector2.zero;
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

    /// <summary>
    /// Check each edge of the collider and adjust velocity accordingly
    /// </summary>
    /// <param name="velocity">Intended velocity to move</param>
    /// <returns>New velocity accounting for collisions</returns>
    private Vector2 AdjustedVelocity(Vector2 velocity) {
      if (raycastCollider.Collisions.Top.IsColliding()) velocity.y = raycastCollider.Collisions.Top.NearestCollision();
      if (raycastCollider.Collisions.Bottom.IsColliding()) velocity.y = -raycastCollider.Collisions.Bottom.NearestCollision();
      if (raycastCollider.Collisions.Right.IsColliding()) velocity.x = raycastCollider.Collisions.Right.NearestCollision();
      if (raycastCollider.Collisions.Left.IsColliding()) velocity.x = -raycastCollider.Collisions.Left.NearestCollision();
      return velocity;
    }

    /// <summary>
    /// Check each edge of the collider and set velocity to 0 if colliding
    /// </summary>
    /// <param name="velocity">Velocity to zero out</param>
    /// <returns>New velocity potentially zeroed out</returns>
    private Vector2 ZeroOutCollisions(Vector2 velocity) {
      if (raycastCollider.Collisions.Top.IsColliding() || raycastCollider.Collisions.Bottom.IsColliding()) velocity.y = 0;
      if (raycastCollider.Collisions.Left.IsColliding() || raycastCollider.Collisions.Right.IsColliding()) velocity.x = 0;
      return velocity;
    }
  }
}