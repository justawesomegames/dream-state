using System.Collections.Generic;
using UnityEngine;

namespace DreamState.Abilities {
  public abstract class SingleHorizontalShoot : Ability {
    [SerializeField] private Transform origin;
    [SerializeField] private BaseProjectile projectile;

    protected override void DoAbility() {
      var clone = ObjectPoolManager.Instance.Spawn(projectile, null, origin.position, origin.rotation);
      clone.Initialize(gameObject, this, damagesWhichTags);
    }
  }
}