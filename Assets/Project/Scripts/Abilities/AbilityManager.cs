using UnityEngine;

namespace DreamState {
  public class AbilityManager : MonoBehaviour {
    [SerializeField] private BaseAbility[] abilities;
    [SerializeField] private Transform rangeAbilitySpawnPoint;

    private float[] cooldowns;
    private int curAbilityIndex;
    private float curChargeTime;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    public void OnAbilityDown() {
      if (cooldowns[curAbilityIndex] <= 0) {
        abilities[curAbilityIndex].DoAbility(gameObject, rangeAbilitySpawnPoint);
        cooldowns[curAbilityIndex] = abilities[curAbilityIndex].Cooldown;
      }
    }

    public void OnAbilityHold() {
      curChargeTime += Time.deltaTime;
    }

    public void OnAbilityUp() {
      for (int i = abilities[curAbilityIndex].ChargeTimes.Length - 1; i >= 0; i--) {
        if (curChargeTime >= abilities[curAbilityIndex].ChargeTimes[i]) {
          abilities[curAbilityIndex].ChargeAbilities[i].DoAbility(gameObject, rangeAbilitySpawnPoint);
          break;
        }
      }

      curChargeTime = 0;
    }

    public void NextAbility() {
      curAbilityIndex++;
      if (curAbilityIndex >= abilities.Length) curAbilityIndex = 0;
    }

    public void PreviousAbility() {
      curAbilityIndex--;
      if (curAbilityIndex < 0) curAbilityIndex = abilities.Length - 1;
    }

    private void OnEnable() {
      cooldowns = new float[abilities.Length];
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
      for(int i = 0; i < cooldowns.Length; i++) {
        if (cooldowns[i] > 0) {
          cooldowns[i] -= Time.deltaTime;
        }
      }

      // If sprite has flipped, set spawn point appropriately
      if (spriteRenderer != null && !spriteRenderer.flipX != facingRight) {
        var curSpawn = rangeAbilitySpawnPoint.transform.localPosition;
        curSpawn.x *= -1;
        rangeAbilitySpawnPoint.transform.localPosition = curSpawn;
        facingRight = !spriteRenderer.flipX;
      }
    }
  }
}