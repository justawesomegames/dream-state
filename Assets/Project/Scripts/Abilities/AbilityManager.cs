using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DreamState.Abilities;

namespace DreamState {
  public class AbilityManager : MonoBehaviour {
    [SerializeField] private List<BaseAbility> abilities;
    [SerializeField] private Transform rangeAbilitySpawnPoint;
    [SerializeField] private List<string> damagesWhichTags;

    private CharacterStats characterStats;
    private float[] cooldowns;
    private float[] chargeTimes;

    public void Cast<T>() {
      var abilityIndex = IndexFor<T>();
      if (abilityIndex == -1) {
        return;
      }

      if (cooldowns[abilityIndex] > 0) {
        // TODO: Notify something about on cooldown
        return;
      }

      var ability = abilities[abilityIndex];
      if (characterStats != null) {
        if (!characterStats.CanExpendResource(ability.ResourceCost)) {
          // TODO: Notify something about insufficient resource
          return;
        }
        characterStats.ExpendResource(ability.ResourceCost);
      }

      ability.DoAbility(gameObject, rangeAbilitySpawnPoint, damagesWhichTags);
      cooldowns[abilityIndex] = ability.Cooldown;
    }

    public void Charge<T>() {
      var abilityIndex = IndexFor<T>();
      if (abilityIndex == -1) {
        return;
      }

      chargeTimes[abilityIndex] += Time.deltaTime;
    }

    public void ReleaseCharge<T>() {
      var abilityIndex = IndexFor<T>();
      if (abilityIndex == -1) {
        for (int i = 0; i < chargeTimes.Length; i++) {
          chargeTimes[i] = 0;
        }
        return;
      }

      for (int i = abilities[abilityIndex].ChargeTimes.Length - 1; i >= 0; i--) {
        if (chargeTimes[abilityIndex] >= abilities[abilityIndex].ChargeTimes[i]) {
          abilities[abilityIndex].ChargeAbilities[i].DoAbility(gameObject, rangeAbilitySpawnPoint, damagesWhichTags);
          break;
        }
      }

      chargeTimes[abilityIndex] = 0;
    }

    private void Awake() {
      cooldowns = new float[abilities.Count];
      chargeTimes = new float[abilities.Count];
      characterStats = GetComponent<CharacterStats>();
    }

    private void Update() {
      for (int i = 0; i < cooldowns.Length; i++) {
        if (cooldowns[i] > 0) {
          cooldowns[i] -= Time.deltaTime;
        }
      }
    }

    private int IndexFor<T>() {
      for (int i = 0; i < abilities.Count; i++) {
        if (abilities[i] is T) {
          return i;
        }
      }

      Debug.LogWarning(string.Format("Ability of type {0} is not found.", typeof(T).ToString()));
      return -1;
    }
  }
}