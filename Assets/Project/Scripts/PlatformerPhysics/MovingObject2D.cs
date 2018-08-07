using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(BoxCollider2D))]
  public abstract class MovingObject2D : MonoBehaviour {
    public Vector2 Velocity { get { return velocity; } }

    protected Vector2 velocity;

    private Dictionary<int, PlatformerPhysics2D> passengers = new Dictionary<int, PlatformerPhysics2D>();
    
    protected abstract Vector3 CalculateNewVelocity();

    private void Awake() {
      if (transform.parent == null) {
        Debug.LogError("MovingObject2D should be nested under a parent object!");
      }
    }

    private void Update() {
      velocity = CalculateNewVelocity();

      if (velocity == Vector2.zero) {
        return;
      }

      HandlePassengers();

      transform.parent.Translate(velocity);
    }

    private void HandlePassengers() {
      var detachables = new List<int>();
      foreach(var kvp in passengers) {
        var passenger = kvp.Value;

        // If passenger is grounded on this, let it be
        if (passenger.Collisions.Bottom.CollidingWith(gameObject)) {
          continue;
        }

        // Check if should detach from sides
        if ((passenger.transform.position.x < transform.position.x && passenger.TargetVelocity.x < velocity.x) ||
            (passenger.transform.position.x > transform.position.x && passenger.TargetVelocity.x > velocity.x)) {
          detachables.Add(kvp.Key);
        }
      }

      foreach(var key in detachables) {
        Detach(passengers[key]);
      }
    }

    private void Attach(PlatformerPhysics2D passenger) {
      var key = passenger.gameObject.GetInstanceID();
      if (passengers.ContainsKey(key)) {
        return;
      }
      passengers.Add(key, passenger);
      passenger.gameObject.transform.parent = transform.parent;
    }

    private void Detach(PlatformerPhysics2D passenger) {
      var key = passenger.gameObject.GetInstanceID();
      if (!passengers.ContainsKey(key)) {
        return;
      }
      passengers.Remove(key);
      passenger.gameObject.transform.parent = null;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      var passenger = collision.gameObject.GetComponent<PlatformerPhysics2D>();
      if (passenger != null) {
        Attach(passenger);
      }
    }

    private void OnCollisionExit2D(Collision2D collision) {
      var passenger = collision.gameObject.GetComponent<PlatformerPhysics2D>();
      if (passenger != null) {
        Detach(passenger);
      }
    }
  }
}