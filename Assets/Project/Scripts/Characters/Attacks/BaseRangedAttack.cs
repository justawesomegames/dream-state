using UnityEngine;

namespace DreamState {
  public abstract class BaseRangedAttack : BaseAttack {
    [SerializeField] protected BaseProjectile projectile;
    [SerializeField] protected Transform spawnPoint;
  }
}