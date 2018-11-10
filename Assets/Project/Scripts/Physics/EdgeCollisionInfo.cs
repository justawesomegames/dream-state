using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// EdgeCollisionInfo contains collision info for a given edge of a BoxCollider2D,
    /// specifically related to collisions via raycasting.
    /// </summary>
    public class EdgeCollisionInfo {
      private List<RaycastHit2D> hits;
      private PlatformerPhysics physics;
      private List<Action<bool>> callbacks;
      private bool lastCollisionState;

      public EdgeCollisionInfo(PlatformerPhysics p) {
        physics = p;
        hits = new List<RaycastHit2D>();
        callbacks = new List<Action<bool>>();
      }

      public void AddHit(RaycastHit2D hit) {
        hits.Add(hit);
      }

      public float NearestCollision() {
        if (hits.Count < 1) return 0.0f;
        return hits.Select(h => h.distance).Min() - physics.SkinWidth;
      }

      public bool IsColliding() {
        return hits.Count > 0;
      }

      public IEnumerable<GameObject> CollisionObjects() {
        return hits.Select(h => h.transform.gameObject).ToList().Distinct();
      }

      public IEnumerable<int> CollidingObjectIds() {
        return hits.Select(h => h.transform.gameObject.GetInstanceID()).Distinct();
      }

      public bool CollidingWith(GameObject g) {
        var ids = CollidingObjectIds();
        return hits.Count > 0 && ids.Contains(g.GetInstanceID());
      }

      public void Reset() {
        hits.Clear();
      }

      public void FinishColliding() {
        var colliding = IsColliding();

        if (colliding != lastCollisionState) {
          callbacks.ForEach(c => c(colliding));
        }

        lastCollisionState = colliding;
      }

      public void RegisterCallback(Action<bool> callback) {
        callbacks.Add(callback);
      }
    }
  }
}