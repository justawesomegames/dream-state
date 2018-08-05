using UnityEngine;

namespace DreamState {
  /// <summary>
  /// MovableObject2D is an object outside of the PlatformerPhysics2D simulation but still
  /// acting on objects within the simulation.
  /// </summary>
  [RequireComponent(typeof(BoxCollider2D))]
  public abstract class MovableObject2D : MonoBehaviour {
    public abstract Vector3 CurrentVelocity();
  }
}
