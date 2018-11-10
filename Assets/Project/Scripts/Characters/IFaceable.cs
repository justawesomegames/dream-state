namespace DreamState {
  public enum FacingDir {
    Left,
    Right
  }

  interface IFaceable {
    bool IsFacing(FacingDir dir);
    void SetFacingDir(FacingDir dir);
    FacingDir CurFacingDir();
  }
}