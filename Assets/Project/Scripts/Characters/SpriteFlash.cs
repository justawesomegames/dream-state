using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(SpriteRenderer))]
  public class SpriteFlash : MonoBehaviour {
    [SerializeField] private Color target = Color.red;
    [SerializeField] private float period = 0.05f;

    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    public void StartFlash(float flashTime) {
      StartCoroutine(Flash(flashTime));
    }

    private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
      initialColor = spriteRenderer.color;
    }

    private IEnumerator Flash(float flashTime) {
      var curFlashTime = 0.0f;
      while (curFlashTime < flashTime) {
        curFlashTime += Time.deltaTime;
        spriteRenderer.color = Color.Lerp(initialColor, target, Mathf.PingPong(Time.time / period, 1));
        yield return null;
      }

      spriteRenderer.color = initialColor;
    }
  }
}