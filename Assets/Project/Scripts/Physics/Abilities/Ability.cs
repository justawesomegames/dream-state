using UnityEngine;

namespace DreamState {
  namespace Physics {
    /// <summary>
    /// An Ability is a physical capability of an object.
    /// </summary>
    [RequireComponent(typeof(PlatformerPhysics))]
    public abstract class Ability : MonoBehaviour {
      protected PlatformerPhysics physics;

      /// <summary>
      /// Called once when the ability is enabled
      /// </summary>
      public virtual void Initialize() { }

      /// <summary>
      /// Called once per frame when velocity is being calculated
      /// </summary>
      public virtual void Do() { }

      private void OnEnable() {
        physics = GetComponent<PlatformerPhysics>();
        physics.RegisterAbility(this);
        Initialize();
      }

      private void OnDestroy() {
        physics.RemoveAbility(this);
      }

      private void OnDisable() {
        physics.RemoveAbility(this);
      }
    }
  }
}