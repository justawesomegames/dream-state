using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DreamState {
  [RequireComponent(typeof(CanvasGroup))]
  public class FloatingHealthBar : PoolableObject {
    [SerializeField] private Image foreground;
    [SerializeField] private float timeToStartFade = 1.0f;
    [SerializeField] private float timeToFade = 0.5f;

    private CanvasGroup canvasGroup;
    private Transform anchor;
    private Coroutine currentFade;

    public override void OnSpawn() {
      canvasGroup.alpha = 0.0f;
    }

    public override void OnDespawn() {
      canvasGroup.alpha = 0.0f;
    }

    public void Initialize(CharacterStats stats, Transform anchor) {
      stats.OnHealthChange += OnHealthChange;
      stats.OnDeath += OnDeath;
      this.anchor = anchor;
    }

    private void Awake() {
      canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    private void LateUpdate() {
      transform.position = anchor.position;
    }

    private void OnHealthChange(float currentHealth, float maxHealth) {
      var newHealthPercent = Mathf.Clamp(currentHealth / maxHealth, 0, maxHealth);
      foreground.fillAmount = newHealthPercent;
      if (currentFade != null) {
        StopCoroutine(currentFade);
      }
      currentFade = StartCoroutine(WaitThenFade());
    }

    private void OnDeath() {
      ObjectPoolManager.Instance.Despawn(this);
    }

    private IEnumerator WaitThenFade() {
      canvasGroup.alpha = 1.0f;
      yield return new WaitForSeconds(timeToStartFade);
      for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / timeToFade) {
        canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, t);
        yield return null;
      }
    }
  }
}
