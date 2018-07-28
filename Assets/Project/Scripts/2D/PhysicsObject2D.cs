using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(BoxRaycastCollider2D))]
  public class PhysicsObject2D : MonoBehaviour {
    #region Public
    public Vector2 CurrentVelocity { get { return currentVelocity; } }
    #endregion

    #region Inspector
    [SerializeField] private float gravityScalar = 1.0f;
    [SerializeField] private float horizontalAcceleration = 0.3f;
    [SerializeField] private float terminalVelocity = 0.3f;
    #endregion

    #region Internal
    private BoxRaycastCollider2D raycastCollider;
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    #endregion

    public void Move(Vector2 moveAmount) {
      targetVelocity = moveAmount;
    }

    public bool Grounded() {
      return raycastCollider.Collisions.Bottom.IsColliding();
    }

    public void SetVerticalVelocity(float force) {
      currentVelocity.y = force;
    }

    private void Awake() {
      raycastCollider = GetComponent<BoxRaycastCollider2D>();
    }

    private void Update() {
      var newVelocity = CalculateNewVelocity();
      raycastCollider.UpdateCollisions(newVelocity);
      newVelocity = AdjustedVelocity(newVelocity);
      currentVelocity = newVelocity;
      transform.Translate(currentVelocity);

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

      // Horizontal velocity
      if (currentVelocity.x < targetVelocity.x) {
        newVelocity.x = Mathf.Min(newVelocity.x + horizontalAcceleration, targetVelocity.x);
      } else if (currentVelocity.x > targetVelocity.x) {
        newVelocity.x = Mathf.Max(newVelocity.x - horizontalAcceleration, targetVelocity.x);
      }

      // Apply gravity
      if (currentVelocity.y > -terminalVelocity) {
        newVelocity.y += Mathf.Clamp(Physics2D.gravity.y * gravityScalar * Time.deltaTime, -terminalVelocity, Mathf.Infinity);
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
  }
}