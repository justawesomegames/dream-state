using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public class FloatingTextManager : Singleton<FloatingTextManager> {
    [SerializeField] private DamageText damageText;

    public void Damage(Vector3 position, float amt) {
      var newText = ObjectPoolManager.Instance.Spawn(damageText, null, position, Quaternion.identity);
      newText.SetText(amt);
    }
  }
}