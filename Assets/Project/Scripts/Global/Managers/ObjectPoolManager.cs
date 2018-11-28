using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {

  [Serializable]
  public class ObjectPoolBootstrap {
    public PoolableObject Object;
    public int Size;
  }

  public class ObjectPoolManager : Singleton<ObjectPoolManager> {
    [SerializeField] private ObjectPool objectPoolPrefab;
    [SerializeField] private int defaultPoolSize = 5;
    [SerializeField] private List<ObjectPoolBootstrap> initialPools = new List<ObjectPoolBootstrap>();

    private Dictionary<string, ObjectPool> objectPools = new Dictionary<string, ObjectPool>();
    private GameObject poolParent;

    public T Spawn<T>(T g, Transform parent, Vector3 position, Quaternion rotation) where T : PoolableObject {
      var ret = GetPoolFor(g).Next();
      ret.transform.SetParent(parent);
      ret.transform.position = position;
      ret.transform.rotation = rotation;
      ret.gameObject.SetActive(true);
      ret.OnSpawn();
      return ret as T;
    }

    public void Despawn(PoolableObject g) {
      ObjectPool pool;
      if (!objectPools.TryGetValue(g.name, out pool)) {
        Debug.LogError(string.Format("Pool not found for {0}.", g.name));
        return;
      }

      pool.DeactivatePoolObject(g);
      g.OnDespawn();
    }

    private void Awake() {
      poolParent = new GameObject("ObjectPools");
      foreach (var pool in initialPools) {
        AddPool(pool.Object, pool.Size);
      }
    }

    private ObjectPool GetPoolFor(PoolableObject g) {
      ObjectPool pool;
      if (!objectPools.TryGetValue(g.name, out pool)) {
        return AddPool(g, defaultPoolSize);
      }
      return pool;
    }

    private ObjectPool AddPool(PoolableObject g, int size) {
      ObjectPool pool;
      if (objectPools.TryGetValue(g.name, out pool)) {
        Debug.LogWarning(string.Format("Object pool for {0} already exists.", g.name));
        return pool;
      }

      pool = Instantiate<ObjectPool>(objectPoolPrefab, poolParent.transform);
      pool.name = string.Format("{0}Pool", g.name);
      pool.Initialize(g, size);
      objectPools.Add(g.name, pool);
      return pool;
    }
  }
}