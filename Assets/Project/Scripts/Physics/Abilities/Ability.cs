using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {

  namespace Physics {
    public enum AbilityStates {
      Doing,
      Stopped
    }

    /// <summary>
    /// An Ability is a physical capability of an object.
    /// </summary>
    [RequireComponent(typeof(PlatformerPhysics))]
    public abstract class Ability : MonoBehaviour {
      public bool Doing { get { return state == AbilityStates.Doing; } }
      [SerializeField] private bool isOverridden = false;

      protected PlatformerPhysics physics;

      private AbilityStates state = AbilityStates.Stopped;
      private List<Action> onStartCallbacks = new List<Action>();
      private List<Action> onDoingCallbacks = new List<Action>();
      private List<Action> onStopCallbacks = new List<Action>();

      /// <summary>
      /// Provide callback for when an ability starts
      /// </summary>
      /// <param name="callback">Function to invoke when ability starts</param>
      public void OnStart(Action callback) {
        onStartCallbacks.Add(callback);
      }

      /// <summary>
      /// Provide callback each frame an ability is happening
      /// </summary>
      /// <param name="callback">Function to invoke while ability is happening</param>
      public void WhileDoing(Action callback) {
        onDoingCallbacks.Add(callback);
      }

      /// <summary>
      /// Provide callback for when an ability stops
      /// </summary>
      /// <param name="callback">Function to invoke when ability stops</param>
      public void OnStop(Action callback) {
        onStopCallbacks.Add(callback);
      }

      /// <summary>
      /// Called once per physics update to handle modifying physics and changing state
      /// </summary>
      public void DoAbility() {
        if (isOverridden) return;
        ProcessAbility();
      }

      /// <summary>
      /// Override the ability, immediately stopping usage
      /// </summary>
      /// <param name="o">Enable or disable override</param>
      public void Override(bool enabled) {
        if (enabled) {
          OnOverrideEnable();
        } else {
          OnOverrideDisable();
        }
        isOverridden = enabled;
      }

      /// <summary>
      /// Called once per physics update unless ability is overridden
      /// </summary>
      protected virtual void ProcessAbility() { }

      /// <summary>
      /// Called once when the ability is enabled
      /// </summary>
      protected virtual void Initialize() { }

      /// <summary>
      /// Called when ability is overridden
      /// </summary>
      protected virtual void OnOverrideEnable() { }

      /// <summary>
      /// Called when ability override is removed
      /// </summary>
      protected virtual void OnOverrideDisable() { }

      protected void ChangeState(AbilityStates newState) {
        if (state == newState) return;

        switch (newState) {
          case AbilityStates.Doing:
            if (state == AbilityStates.Stopped) {
              onStartCallbacks.ForEach(c => c());
            }
            onDoingCallbacks.ForEach(c => c());
            break;
          case AbilityStates.Stopped:
            onStopCallbacks.ForEach(c => c());
            break;
        }
        state = newState;
      }

      private void Awake() {
        physics = GetComponent<PlatformerPhysics>();
      }

      private void OnEnable() {
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