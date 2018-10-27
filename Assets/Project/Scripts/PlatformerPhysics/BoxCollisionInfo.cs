namespace DreamState {
  /// <summary>
  /// BoxCollisionInfo conveniently stores the collision info of the four edges
  /// of a box.
  /// </summary>
  public class BoxCollisionInfo {
    public EdgeCollisionInfo Top;
    public EdgeCollisionInfo Bottom;
    public EdgeCollisionInfo Left;
    public EdgeCollisionInfo Right;

    public BoxCollisionInfo(PlatformerPhysics2D p) {
      Top = new EdgeCollisionInfo(p);
      Bottom = new EdgeCollisionInfo(p);
      Left = new EdgeCollisionInfo(p);
      Right = new EdgeCollisionInfo(p);
    }

    public void Reset() {
      Top.Reset();
      Bottom.Reset();
      Left.Reset();
      Right.Reset();
    }

    public void FinishColliding() {
      Top.FinishColliding();
      Bottom.FinishColliding();
      Left.FinishColliding();
      Right.FinishColliding();
    }
  }
}