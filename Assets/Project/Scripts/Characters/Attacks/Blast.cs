namespace DreamState {
  public class Blast : BaseRangedAttack {
    public override void DoAttack() {
      // TODO: Some kind of spawn pool
      Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
    }
  }
}