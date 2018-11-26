using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public class ObjectPool {
    private PoolableObject objectToPool;
    private int poolSize;

    private List<PoolableObject> pool;

    public ObjectPool(PoolableObject objectToPool, int poolSize) {
      this.objectToPool = objectToPool;
      this.poolSize = poolSize;

      pool = new List<PoolableObject>(poolSize);
      for (int i = 0; i < poolSize; i++) {
        pool.Add(InstantiatePoolObject());
      }
    }

    public PoolableObject Next() {
      var initialCount = pool.Count;
      foreach (var g in pool) {
        if (!g.gameObject.activeInHierarchy) {
          return g;
        }
      }

      // Double pool size
      ExpandPool(pool.Count * 2);
      return pool[initialCount];
    }

    public void DeactivatePoolObject(PoolableObject objectToDeactivate) {
      foreach (var g in pool) {
        if (g == objectToDeactivate) {
          g.gameObject.SetActive(false);
          return;
        }
      }

      Debug.LogError(string.Format("Object {0} not found in object pool.", objectToDeactivate.name));
    }

    private void ExpandPool(int amtToExpand) {
      for (int i = 0; i < amtToExpand; i++) {
        pool.Add(InstantiatePoolObject());
      }
    }

    private PoolableObject InstantiatePoolObject() {
      var obj = GameObject.Instantiate(objectToPool);
      obj.name = objectToPool.name;
      obj.gameObject.SetActive(false);
      // obj.transform.SetParent(transform);
      return obj;
    }
  }
}