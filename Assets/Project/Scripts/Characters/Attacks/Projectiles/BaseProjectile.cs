using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(BoxCollider2D))]
  public abstract class BaseProjectile : MonoBehaviour {
    [SerializeField] protected float damage;
    [SerializeField] protected Vector2 staticVelocity;
  }
}