using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  public abstract class Character : MonoBehaviour, IFaceable {
    [SerializeField] private FacingDir InitialFacingDir = FacingDir.Right;

    protected FacingDir curFacingDir;
    protected PlatformerPhysics physics;
    protected CharacterStats stats;

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

    /// <summary>
    /// Called when the character receives damage
    /// </summary>
    /// <param name="amt">Amount of damage taken</param>
    public virtual void OnDamageTaken(float amt) { }

    /// <summary>
    /// Called after awake
    /// </summary>
    protected virtual void OnAwake() { }

    /// <summary>
    /// Called during each update frame
    /// </summary>
    protected virtual void OnUpdate() { }

    private void Awake() {
      physics = GetComponent<PlatformerPhysics>();
      stats = GetComponent<CharacterStats>();
      stats.OnDamageTaken += OnDamageTaken;
      curFacingDir = InitialFacingDir;
      OnAwake();
    }

    private void Update() {
      if (physics != null) {
        if (physics.TargetVelocity.x < 0.0f && curFacingDir == FacingDir.Right) {
          SetFacingDir(FacingDir.Left);
        } else if (physics.TargetVelocity.x > 0.0f && curFacingDir == FacingDir.Left) {
          SetFacingDir(FacingDir.Right);
        }
      }

      OnUpdate();
    }
  }
}