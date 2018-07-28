using System;
using System.Linq;
using UnityEngine;

namespace DreamState {
  using System.Collections.Generic;
  using Global;

  [RequireComponent(typeof(BoxCollider2D))]
  public class BoxRaycastCollider2D : MonoBehaviour {
    [HideInInspector] public CollisionInfo Collisions { get {
      // Singleton
      if (_collisions == null) _collisions = new CollisionInfo(this);
      return _collisions;
    } }
    [HideInInspector] public LayerMask CollisionMask { get { return collisionMask; } }
    
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private float distanceBetweenRays = 0.25f;
    [SerializeField] private int horizontalRayCount = 3;
    [SerializeField] private int verticalRayCount = 3;
    
    private CollisionInfo _collisions;
    private BoxCollider2D boxCollider;
    private RaycastOrigins raycastOrigins;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    
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
      ClearCollisions();
      UpdateRaycastOrigins();

      Vector2 v = moveVector;
      if (v.x != 0) v = UpdateHorizontalCollisions(v);
      if (v.y != 0) UpdateVerticalCollisions(v);
    }

    private Vector2 UpdateVerticalCollisions(Vector2 moveVector) {
      float yDir = Mathf.Sign(moveVector.y);
      var rayLength = Mathf.Abs(moveVector.y) + skinWidth;
      for (int i = 0; i < verticalRayCount; i++) {
        Vector2 rayOrigin = yDir == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
        rayOrigin += Vector2.right * (verticalRaySpacing * i + moveVector.x);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * yDir, rayLength, collisionMask);

        Debug.DrawRay(rayOrigin, Vector2.up * yDir, Color.red);

        // Check if there was actually a collision
        if (!hit) continue;

        // Let collision affect other ray collision detection
        moveVector.y = (hit.distance - skinWidth) * yDir;

        // Reduce ray length to avoid unnecessary collisions
        rayLength = hit.distance;

        // Add collision to the appropriate collider
        if (yDir == -1) {
          Collisions.Bottom.hits.Add(hit);
        } else {
          Collisions.Top.hits.Add(hit);
        }
      }

      return moveVector;
    }

    private Vector2 UpdateHorizontalCollisions(Vector2 moveVector) {
      float xDir = Mathf.Sign(moveVector.x);
      var rayLength = Mathf.Abs(moveVector.x) + skinWidth;

      // Don't cast farther than we need to
      if (Mathf.Abs(moveVector.x) < skinWidth) rayLength = 2 * skinWidth;

      for (int i = 0; i < horizontalRayCount; i++) {
        Vector2 rayOrigin = xDir == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        rayOrigin += Vector2.up * (horizontalRaySpacing * i);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * xDir, rayLength, collisionMask);

        // Check if there was actually a collision
        if (!hit) continue;
        if (hit.distance == 0) continue;

        // Let collision affect other ray collision detection
        moveVector.x = (hit.distance - skinWidth) * xDir;

        // Reduce ray length to avoid unnecessary collisions
        rayLength = hit.distance;

        // Add collision to the appropriate collider
        if (xDir == -1) {
          Collisions.Left.hits.Add(hit);
        } else {
          Collisions.Right.hits.Add(hit);
        }
      }

      return moveVector;
    }

    private void ClearCollisions() {
      Collisions.Top.Clear();
      Collisions.Bottom.Clear();
      Collisions.Left.Clear();
      Collisions.Right.Clear();
    }

    public struct RaycastOrigins {
      public Vector2 topLeft;
      public Vector2 topRight;
      public Vector2 bottomLeft;
      public Vector2 bottomRight;
    }

    public class CollisionInfo {
      public EdgeCollisionInfo Top;
      public EdgeCollisionInfo Bottom;
      public EdgeCollisionInfo Left;
      public EdgeCollisionInfo Right;

      public CollisionInfo(BoxRaycastCollider2D c) {
        Top = new EdgeCollisionInfo(c);
        Bottom = new EdgeCollisionInfo(c);
        Left = new EdgeCollisionInfo(c);
        Right = new EdgeCollisionInfo(c);
      }
    }

    public class EdgeCollisionInfo {
      public List<RaycastHit2D> hits;
      private BoxRaycastCollider2D collider;
      
      public EdgeCollisionInfo(BoxRaycastCollider2D c) {
        collider = c;
        hits = new List<RaycastHit2D>();
      }

      public float NearestCollision() {
        if (hits.Count < 1) return 0.0f;
        return hits.Select(h => h.distance).Min() - collider.skinWidth;
      }

      public bool IsColliding() {
        return hits.Count > 0;
      }

      public void Clear() {
        hits.Clear();
      }
    }
  }
}
