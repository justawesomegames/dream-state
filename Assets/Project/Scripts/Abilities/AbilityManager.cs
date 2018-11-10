using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamState {
  public class AbilityManager : MonoBehaviour {
    [SerializeField] private List<BaseAbility> abilities;
    [SerializeField] private Transform rangeAbilitySpawnPoint;
    [SerializeField] private List<string> damagesWhichTags;

    private CharacterStats characterStats;
    private float[] cooldowns;
    private int curAbilityIndex;
    private float curChargeTime;
    private bool facingRight = true;

    public void OnAbilityDown() {
      if (cooldowns[curAbilityIndex] > 0) {
        // TODO: Notify something about on cooldown
        return;
      }

      var ability = abilities[curAbilityIndex];
      if (characterStats != null) {
        if (!characterStats.CanExpendResource(ability.ResourceCost)) {
          // TODO: Notify something about insufficient resource
          return;
        }
        characterStats.ExpendResource(ability.ResourceCost);
      }

      ability.DoAbility(gameObject, rangeAbilitySpawnPoint, damagesWhichTags);
      cooldowns[curAbilityIndex] = ability.Cooldown;
    }

    public void OnAbilityHold() {
      curChargeTime += Time.deltaTime;
    }

    public void OnAbilityUp() {
      for (int i = abilities[curAbilityIndex].ChargeTimes.Length - 1; i >= 0; i--) {
        if (curChargeTime >= abilities[curAbilityIndex].ChargeTimes[i]) {
          abilities[curAbilityIndex].ChargeAbilities[i].DoAbility(gameObject, rangeAbilitySpawnPoint, damagesWhichTags);
          break;
        }
      }

      curChargeTime = 0;
    }

    public void NextAbility() {
      curAbilityIndex++;
      if (curAbilityIndex >= abilities.Count) curAbilityIndex = 0;
    }

    public void PreviousAbility() {
      curAbilityIndex--;
      if (curAbilityIndex < 0) curAbilityIndex = abilities.Count - 1;
    }

    public void AddAbility(BaseAbility ability) {
      if (abilities.Any(a => a.Name == ability.Name)) {
        Debug.LogError(String.Format("Ability {0} already exists on {1}", ability.Name, gameObject.name));
        return;
      }

      abilities.Add(ability);

      var newCooldowns = new float[abilities.Count];
      for (int i = 0; i < cooldowns.Length; i++) {
        newCooldowns[i] = cooldowns[i];
      }
      cooldowns = newCooldowns;
    }

    private void OnEnable() {
      cooldowns = new float[abilities.Count];
      characterStats = GetComponent<CharacterStats>();
    }

    private void Update() {
      for (int i = 0; i < cooldowns.Length; i++) {
        if (cooldowns[i] > 0) {
          cooldowns[i] -= Time.deltaTime;
        }
      }
    }
  }
}