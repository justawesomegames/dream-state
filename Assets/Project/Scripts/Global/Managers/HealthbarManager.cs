using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public class HealthbarManager : Singleton<HealthbarManager> {
    [SerializeField] private FloatingHealthBar floatingHealthBar;

    public FloatingHealthBar AttachFloatingHealthbar(CharacterStats stats, Transform anchor) {
      var newHealthbar = ObjectPoolManager.Instance.Spawn(floatingHealthBar, null, anchor.position, Quaternion.identity);
      newHealthbar.Initialize(stats, anchor);
      return newHealthbar;
    }
  }
}