using System;
using UnityEngine;

namespace DreamState {
  public class CharacterStats : MonoBehaviour {
    [SerializeField] private int level = 1;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float maxResource = 100;

    private float currentHealth;
    private float currentResource;

    public bool CanExpendResource(float amt) {
      return currentResource > amt;
    }

    public void ExpendResource(float amt) {
      currentResource -= amt;
    }

    public void Damage(float amt) {
      currentHealth -= amt;
      if (currentHealth <= 0) {
        // TODO: Death animation?
        Destroy(gameObject);
      }
    }

    private void Awake() {
      currentHealth = maxHealth;
      currentResource = maxResource;
    }
  }
}