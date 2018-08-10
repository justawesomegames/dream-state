using UnityEngine;

namespace DreamState {
  [CreateAssetMenu(menuName = "Abilities/RangedAbility")]
  public class RangedAbility : BaseAbility {
    public BaseProjectile Projectile;

    public override void DoAbility(GameObject caster, Transform origin) {
      var projectile = Instantiate(Projectile, origin.position, origin.rotation);
      projectile.Initialize(caster, this);
    }
  }
}