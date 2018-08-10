using UnityEngine;

namespace DreamState {
  public abstract class BaseAbility : ScriptableObject {
    public float Damage = 1.0f;
    public float Cooldown = 0.1f;
    public BaseAbility[] ChargeAbilities;
    public float[] ChargeTimes;

    public abstract void DoAbility(GameObject caster, Transform origin);
  }
}