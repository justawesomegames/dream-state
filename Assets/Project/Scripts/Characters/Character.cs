using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  public abstract class Character : MonoBehaviour, IFaceable {
    private FacingDir curFacingDir = FacingDir.Right;
    private PlatformerPhysics physics;

    public bool IsFacing(FacingDir dir) {
      return curFacingDir == dir;
    }

    public void SetFacingDir(FacingDir dir) {
      curFacingDir = dir;
      var newScale = gameObject.transform.localScale;
      newScale.x = dir == FacingDir.Right ? 1 : -1;
      gameObject.transform.localScale = newScale;
    }

    public FacingDir CurFacingDir() {
      return curFacingDir;
    }

    private void Awake() {
      physics = GetComponent<PlatformerPhysics>();
    }

    private void Update() {
      if (physics != null) {
        if (physics.TargetVelocity.x < 0.0f && curFacingDir == FacingDir.Right) {
          SetFacingDir(FacingDir.Left);
        } else if (physics.TargetVelocity.x > 0.0f && curFacingDir == FacingDir.Left) {
          SetFacingDir(FacingDir.Right);
        }
      }
    }
  }
}