using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DreamState {
  public class FloatingHealthBar : MonoBehaviour {
    [SerializeField] private Image foreground;

    private CharacterStats stats;

    private void Awake() {
      stats = GetComponentInParent<CharacterStats>();
      if (stats == null) {
        Debug.LogError("Health bar present without associated CharacterStats.");
        return;
      }
      stats.OnHealthChange += OnHealthChange;
    }

    private void OnHealthChange(float currentHealth, float maxHealth) {
      foreground.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, maxHealth);
    }
  }
}
