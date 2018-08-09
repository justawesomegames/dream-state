using UnityEngine;

namespace DreamState {
  public abstract class BaseAttack : MonoBehaviour {
    [SerializeField] private float attackTimeout = 0.1f;

    [Header("Charging")]
    [SerializeField] private BaseAttack[] chargeAttacks;
    [SerializeField] private float[] chargeTimes;

    private float curAttackTimeout;
    private float curChargeTime;

    public abstract void DoAttack();

    public void OnAttackDown() {
      if (curAttackTimeout < 0) {
        DoAttack();
        curAttackTimeout = attackTimeout;
      }
    }

    public void OnAttackHold() {
      curChargeTime += Time.deltaTime;
    }

    public void OnAttackUp() {
      for(int i = chargeTimes.Length - 1; i >= 0; i--) {
        if (curChargeTime >= chargeTimes[i]) {
          chargeAttacks[i].DoAttack();
          break;
        }
      }

      curChargeTime = 0;
    }

    private void Update() {
      if (curAttackTimeout > 0) {
        curAttackTimeout -= Time.deltaTime;
      }
    }
  }
}