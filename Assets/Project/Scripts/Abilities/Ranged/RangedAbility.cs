using System.Collections.Generic;
using UnityEngine;

namespace DreamState.Abilities {
  public class RangedAbility : BaseAbility {
    public BaseProjectile Projectile;

    public override void DoAbility(GameObject caster, Transform origin, List<string> damagesWhat) {
      var projectile = ObjectPoolManager.Instance.Spawn(Projectile, null, origin.position, origin.rotation);
      projectile.Initialize(caster, this, damagesWhat);
    }
  }
}