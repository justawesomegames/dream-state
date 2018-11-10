using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// BoxCollisionInfo conveniently stores the collision info of the four edges
    /// of a box.
    /// </summary>
    public class BoxCollisionInfo {
      public EdgeCollisionInfo Top;
      public EdgeCollisionInfo Bottom;
      public EdgeCollisionInfo Left;
      public EdgeCollisionInfo Right;
      public float CurSlopeAngle;
      public float LastSlopeAngle;
      public bool ClimbingSlope;
      public bool DescendingSlope;
      public bool SlidingDownMaxSlope;
      public Vector2 LastMoveAmount;
      public int LastFacingDir;


      public BoxCollisionInfo(PlatformerPhysics p) {
        Top = new EdgeCollisionInfo(p);
        Bottom = new EdgeCollisionInfo(p);
        Left = new EdgeCollisionInfo(p);
        Right = new EdgeCollisionInfo(p);
        LastFacingDir = 1;
      }

      public void Reset() {
        Top.Reset();
        Bottom.Reset();
        Left.Reset();
        Right.Reset();
        LastSlopeAngle = CurSlopeAngle;
        CurSlopeAngle = 0;
        ClimbingSlope = false;
        DescendingSlope = false;
        SlidingDownMaxSlope = false;
      }

      public void FinishColliding() {
        Top.FinishColliding();
        Bottom.FinishColliding();
        Left.FinishColliding();
        Right.FinishColliding();
      }
    }
  }
}