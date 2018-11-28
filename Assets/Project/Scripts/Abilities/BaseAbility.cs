using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState.Abilities {
  public abstract class BaseAbility : MonoBehaviour {
    public string DisplayName;
    public float Damage = 1.0f;
    public float Cooldown = 0.1f;
    public float ResourceCost = 0.0f;
    public BaseAbility[] ChargeAbilities;
    public float[] ChargeTimes;

    public abstract void DoAbility(GameObject caster, Transform origin, List<string> damagesWhat);
  }
}