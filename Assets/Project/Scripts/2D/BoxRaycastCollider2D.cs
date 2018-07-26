using System;
using UnityEngine;

namespace DreamState {
  using System.Collections.Generic;
  using Global;

  [RequireComponent(typeof(BoxCollider2D))]
  public class BoxRaycastCollider2D : MonoBehaviour {
    #region Public
    [HideInInspector]
    public EdgeCollisionInfo Top;
    [HideInInspector]
    public EdgeCollisionInfo Bottom;
    [HideInInspector]
    public EdgeCollisionInfo Left;
    [HideInInspector]
    public EdgeCollisionInfo Right;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private float distanceBetweenRays = 0.25f;
    [SerializeField] private int horizontalRayCount = 3;
    [SerializeField] private int verticalRayCount = 3;
    #endregion

    #region Internal
    private BoxCollider2D boxCollider;
    private RaycastOrigins raycastOrigins;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    #endregion

    private void Awake() {
      boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start() {
      // Calculate spacing between rays
      Bounds bounds = boxCollider.bounds;
      bounds.Expand(skinWidth * -2);

      float boundsWidth = bounds.size.x;
      float boundsHeight = bounds.size.y;
      
      horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
      verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);
      
      horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
      verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private void UpdateRaycastOrigins() {
      // Update the origin of each ray
      Bounds bounds = boxCollider.bounds;
      bounds.Expand(skinWidth * -2);
      
      raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
      raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
      raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
      raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    /// <summary>
    /// Calculate collisions based on a potential movement
    /// </summary>
    /// <param name="moveAmount">Amount to move</param>
    public void UpdateCollisions(Vector2 moveVector) {
      UpdateRaycastOrigins();
      UpdateTopCollision(moveVector);
      UpdateBottomCollision(moveVector);
    }

    private void UpdateTopCollision(Vector2 moveVector) {
      Top.Clear();
      if (moveVector.y <= 0.0f) return;

      var rayLength = Mathf.Abs(moveVector.y) + skinWidth;
      for (int i = 0; i < verticalRayCount; i ++) {
        Vector2 rayOrigin = raycastOrigins.topLeft;
        rayOrigin += Vector2.right * (verticalRaySpacing * i + moveVector.x);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, collisionMask);
        if (hit) {
          Top.hits.Add(hit);
          rayLength = hit.distance;
        }
      }
    }

    private void UpdateBottomCollision(Vector2 moveVector) {
      Bottom.Clear();
      if (moveVector.y >= 0.0f) return;

      var rayLength = Mathf.Abs(moveVector.y) + skinWidth;
      for (int i = 0; i < verticalRayCount; i ++) {
        Vector2 rayOrigin = raycastOrigins.bottomLeft;
        rayOrigin += Vector2.right * (verticalRaySpacing * i + moveVector.x);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);
        if (hit) {
          Bottom.hits.Add(hit);
          rayLength = hit.distance;
        }
      }
    }

    public struct RaycastOrigins {
      public Vector2 topLeft;
      public Vector2 topRight;
      public Vector2 bottomLeft;
      public Vector2 bottomRight;
    }

    public struct EdgeCollisionInfo {
      public bool colliding;
      public List<RaycastHit2D> hits;

      public float DistanceToCollision() {
        // TODO
        return 0.0f;
      }

      public bool IsColliding() {
        return hits.Count > 0;
      }

      public void Clear() {
        colliding = false;
        hits.Clear();
      }
    }
  }
}
