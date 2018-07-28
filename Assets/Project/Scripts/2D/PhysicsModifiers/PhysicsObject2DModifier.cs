using System;
using UnityEngine;

namespace DreamState {
  /// <summary>
  /// PhysicsObject2DModifier encapsulates any changes to velocity outside
  /// of the standard PhysicsObject2D handling.
  /// </summary>
  public abstract class PhysicsObject2DModifier {
    protected PhysicsObject2D target;
    private string guid;

    #region Abstract functions
    /// <summary>
    /// Modify velocity after physics has applied gravity and acceleration
    /// </summary>
    /// <param name="v">Velocity after gravity and acceleration have been applied</param>
    /// <returns>New velocity</returns>
    public abstract Vector2 ModifyVelocity(Vector2 v);

    /// <summary>
    /// Should this modifier be unique?
    /// </summary>
    /// <returns>If modifier is unique</returns>
    public abstract bool IsUnique();
    #endregion

    /// <summary>
    /// Set the physics object that this modifier will act upon
    /// </summary>
    /// <param name="o"></param>
    public void SetTarget(PhysicsObject2D o) {
      target = o;
    }

    public string GetGuid() {
      if (guid == null) {
        if (IsUnique()) {
          guid = GetType().Name;
        } else {
          guid = Guid.NewGuid().ToString();
        }
      }
      return guid;
    }
  }
}
