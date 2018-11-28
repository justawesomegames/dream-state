using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public class HostileCollider : MonoBehaviour {
    [SerializeField] private float damage = 1f;
    [SerializeField] private List<string> damagesWhichTags;

    private void OnCollisionStay2D(Collision2D collision) {
      var hit = collision.gameObject;
      if (damagesWhichTags.Contains(hit.tag)) {
        var stats = hit.GetComponent<CharacterStats>();
        if (stats != null) {
          stats.Damage(damage);
        }
      }
    }
  }
}