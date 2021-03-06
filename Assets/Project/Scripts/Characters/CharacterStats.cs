﻿using System;
using UnityEngine;

namespace DreamState {
  public class CharacterStats : MonoBehaviour {
    [SerializeField] private int level = 1;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float maxResource = 100;
    [SerializeField] private float currentHealth = 100;
    [SerializeField] private float currentResource = 100;
    [SerializeField] private bool invulnerable = false;
    [SerializeField] private Transform floatingTextSpawn;
    [SerializeField] private bool hasFloatingHealthBar = true;
    [SerializeField] private Transform healthbarAnchor;

    public event Action<float> OnDamageTaken = delegate { };
    public event Action<float, float> OnHealthChange = delegate { };
    public event Action OnDeath = delegate { };

    public bool CanExpendResource(float amt) {
      return currentResource > amt;
    }

    public void ExpendResource(float amt) {
      currentResource -= amt;
    }

    /// <summary>
    /// Inflict damage on this object
    /// </summary>
    /// <param name="amt">Amount of damage to inflict</param>
    /// <returns>True if damage successfully inflicted, false otherwise</returns>
    public bool Damage(float amt) {
      if (invulnerable) {
        return false;
      }

      currentHealth -= amt;
      FloatingTextManager.Instance.Damage(floatingTextSpawn != null ? floatingTextSpawn.position : transform.position, amt);
      OnDamageTaken(amt);
      OnHealthChange(currentHealth, maxHealth);

      if (currentHealth <= 0) {
        // TODO: Death animation?
        OnDeath();
        Destroy(gameObject);
        return true;
      }

      return true;
    }

    public void SetInvulnerable(bool i) {
      invulnerable = i;
    }

    private void Start() {
      if (hasFloatingHealthBar) {
        HealthbarManager.Instance.AttachFloatingHealthbar(this, healthbarAnchor);
      }
    }
  }
}