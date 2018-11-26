using System;
using UnityEngine;

namespace DreamState {
  public class CharacterStats : MonoBehaviour {
    [SerializeField] private int level = 1;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float maxResource = 100;
    [SerializeField] private float currentHealth = 100;
    [SerializeField] private float currentResource = 100;
    [SerializeField] private bool invulnerable = false;

    private Character character;

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
      FloatingTextManager.Instance.Damage(gameObject, amt);

      if (currentHealth <= 0) {
        // TODO: Death animation?
        Destroy(gameObject);
      }

      if (character != null) {
        character.OnDamageTaken(amt);
      }

      return true;
    }

    public void SetInvulnerable(bool i) {
      invulnerable = i;
    }

    private void Awake() {
      character = GetComponent<Character>();
    }
  }
}