using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(BoxRaycastCollider2D))]
  public abstract class PhysicsObject2D : MonoBehaviour {
    #region Public
    [SerializeField] private float gravityScalar = 1.0f;
    [SerializeField] private float maxFallSpeed = 10f;
    [SerializeField] private float maxSlopeAngle = 45f;
    #endregion

    #region Internal
    private BoxRaycastCollider2D raycastCollider;
    #endregion

    #region Abstract functions
    protected abstract Vector2 Move();
    #endregion

    private void Awake() {
      raycastCollider = GetComponent<BoxRaycastCollider2D>();
    }

    private void Update() {
      // Handle gravity
      // Handle slopes
      // Handle moving platforms
      // Get move from abstract and apply
    }
  }
}