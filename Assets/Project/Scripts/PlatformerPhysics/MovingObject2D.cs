using UnityEngine;

namespace DreamState {
  [DisallowMultipleComponent]
  [RequireComponent(typeof(BoxCollider2D))]
  public abstract class MovingObject2D : MonoBehaviour {
    public Vector2 Velocity { get { return velocity; } }

    protected Vector2 velocity;
    
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

      transform.parent.Translate(velocity);
    }
  }
}