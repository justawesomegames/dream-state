using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  /// <summary>
  /// MovableObject2D is an object outside of the PlatformerPhysics2D simulation but still
  /// acting on objects within the simulation.
  /// </summary>
  [RequireComponent(typeof(BoxCollider2D))]
  public abstract class MovableObject2D : MonoBehaviour {
    public Vector2 Velocity { get { return velocity; } }

    protected Vector2 velocity;

    private Dictionary<int, PlatformerPhysics2D> movables = new Dictionary<int, PlatformerPhysics2D>();

    protected abstract Vector3 CalculateNewVelocity();

    protected void Update() {
      velocity = CalculateNewVelocity();

      if (velocity == Vector2.zero) {
        return;
      }

      foreach (var movable in movables) {
        var movablePhysics = movable.Value;
        // Handle object standing on top of this object
        if (movablePhysics.Collisions.Bottom.CollidingWith(gameObject)) {
          movablePhysics.MoveNow(velocity);
        }
        // Handle pushing objects horizontally
        else if ((movablePhysics.Collisions.Left.CollidingWith(gameObject) && velocity.x > 0) ||
                 (movablePhysics.Collisions.Right.CollidingWith(gameObject) && velocity.x < 0)) {
          movablePhysics.MoveNow(Vector2.right * velocity.x);
        }
      }

      transform.Translate(velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      var movable = collision.gameObject.GetComponent<PlatformerPhysics2D>();
      if (movable != null) {
        movables.Add(movable.GetInstanceID(), movable);
      }
    }

    private void OnCollisionExit2D(Collision2D collision) {
      var movable = collision.gameObject.GetComponent<PlatformerPhysics2D>();
      if (movable != null) {
        movables.Remove(movable.GetInstanceID());
      }
    }
  }
}
