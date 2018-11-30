using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState.Abilities {
  public abstract class Ability : MonoBehaviour {
    [SerializeField] private AbilityPresenter presenter;
    [SerializeField] private float baseMinimumDamage;
    [SerializeField] private float baseMaximumDamage;
    [SerializeField] private float cooldown;
    [SerializeField] private float resourceCost;
    [SerializeField] protected List<string> damagesWhichTags;

    private List<Ability> chargeAbilities = new List<Ability>();
    private List<float> chargeTimes = new List<float>();
    private CharacterStats stats;
    private float currentCooldown;
    private float currentChargeTime;
    private int currentChargeLevel = 0;

    public void Cast() {
      if (currentCooldown > 0) {
        return;
      }

      if (stats != null) {
        if (!stats.CanExpendResource(resourceCost)) {
          return;
        }
        stats.ExpendResource(resourceCost);
      }

      currentCooldown = cooldown;
      DoAbility();
    }

    public virtual float CalculateDamage() {
      return Random.Range(baseMinimumDamage, baseMaximumDamage);
    }

    public void Charge() {
      currentChargeTime += Time.deltaTime;
      for (int i = 0; i < chargeAbilities.Count; i++) {
        if (currentChargeTime > chargeTimes[i] && currentChargeLevel < i + 1) {
          OnChargeReached(++currentChargeLevel);
        }
      }
    }

    public void ReleaseCharge() {
      if (currentChargeLevel > 0) {
        chargeAbilities[currentChargeLevel - 1].Cast();
      }
      OnChargeReleased();
      currentChargeTime = 0.0f;
      currentChargeLevel = 0;
    }

    protected void AddChargedAbility(Ability chargedAbility, float chargeTime) {
      chargeAbilities.Add(chargedAbility);
      chargeTimes.Add(chargeTime);
    }

    protected abstract void DoAbility();

    protected virtual void OnChargeReached(int level) { }

    protected virtual void OnChargeReleased() { }

    protected virtual void OnStart() { }

    private void Start() {
      stats = GetComponent<CharacterStats>();
      OnStart();
    }

    private void Update() {
      if (currentCooldown > 0) {
        currentCooldown -= Time.deltaTime;
      }
    }
  }
}
