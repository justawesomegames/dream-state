using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public class FloatingTextManager : Singleton<FloatingTextManager> {
    [SerializeField] private DamageText damageText;

    public void Damage(GameObject target, float amt) {
      var spawnPosition = GetSpawnPoint(target);
      var newText = ObjectPoolManager.Instance.Spawn(damageText, null, spawnPosition, Quaternion.identity);
      newText.GetComponent<DamageText>().SetText(amt);
    }

    private Vector3 GetSpawnPoint(GameObject target) {
      var spawnObj = target.transform.Find("FloatingTextSpawn");
      if (spawnObj != null) {
        return spawnObj.transform.position;
      }

      return target.transform.position;
    }
  }
}