using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public abstract class PoolableObject : MonoBehaviour {
    /// <summary>
    /// Called when the object is spawned by the object pool
    /// </summary>
    public abstract void OnSpawn();

    /// <summary>
    /// Called when the object is despawned by the object pool
    /// </summary>
    public abstract void OnDespawn();
  }
}