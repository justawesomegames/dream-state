using System;
using UnityEngine;

namespace DreamState {
  /// <summary>
  /// PlatformerPhysics2DModifier encapsulates any changes to velocity outside
  /// of the standard PlatformerPhysics2D handling.
  /// </summary>
  public abstract class PlatformerPhysics2DModifier {
    public string Guid { get {
      if (guid == String.Empty) {
        if (IsUniqueModifier()) {
          guid = GetType().Name;
        } else {
          guid = System.Guid.NewGuid().ToString();
        }
      }
      return guid;
    } }
    protected PlatformerPhysics2D target;
    private string guid;

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
    public abstract bool IsUniqueModifier();

    /// <summary>
    /// Set the physics object that this modifier will act upon
    /// </summary>
    /// <param name="o"></param>
    public void SetTarget(PlatformerPhysics2D o) {
      target = o;
    }
  }
}
