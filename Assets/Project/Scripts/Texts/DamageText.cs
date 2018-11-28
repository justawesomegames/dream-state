using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(TextMesh))]
  public class DamageText : PoolableObject {
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private List<Vector2> initialVelocities = new List<Vector2>();
    [SerializeField] private Vector2 targetVelocity = new Vector2(0, -2);

    private TextMesh textMesh;
    private Vector2 initialVelocity;
    private Vector2 currentVelocity;

    public void SetText(float amt) {
      textMesh.text = Mathf.Round(amt).ToString();
    }

    public override void OnSpawn() {
      initialVelocity = initialVelocities[Random.Range(0, initialVelocities.Count)];
      currentVelocity = initialVelocity;
      currentVelocity.x *= Random.Range(0, 10) > 4 ? -1 : 1;
      StartCoroutine(Fade());
      StartCoroutine(Move());
    }

    public override void OnDespawn() { }

    private void Awake() {
      textMesh = GetComponent<TextMesh>();
    }

    private IEnumerator Fade() {
      for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / lifetime) {
        var newColor = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, Mathf.Lerp(1.0f, 0.0f, t));
        textMesh.color = newColor;
        yield return null;
      }

      ObjectPoolManager.Instance.Despawn(this);
    }

    private IEnumerator Move() {
      for (var t = 0.0f; t < lifetime; t += Time.deltaTime / lifetime) {
        currentVelocity.x = Mathf.Lerp(initialVelocity.x, targetVelocity.x, t);
        currentVelocity.y = Mathf.Lerp(initialVelocity.y, targetVelocity.y, t);
        transform.Translate(currentVelocity * Time.deltaTime);
        yield return null;
      }
    }
  }
}