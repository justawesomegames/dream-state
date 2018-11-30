using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState.Abilities {
  [RequireComponent(typeof(BlastCharge1))]
  [RequireComponent(typeof(BlastCharge2))]
  public class Blast : SingleHorizontalShoot {
    [SerializeField] private float chargedTime = 0.75f;
    [SerializeField] private float superchargedTime = 1.5f;
    [SerializeField] private ParticleSystem chargedParticles;
    [SerializeField] private ParticleSystem superchargedParticles;

    protected override void OnStart() {
      AddChargedAbility(GetComponent<BlastCharge1>(), chargedTime);
      AddChargedAbility(GetComponent<BlastCharge2>(), superchargedTime);
    }

    protected override void OnChargeReached(int level) {
      if (level == 1) {
        chargedParticles.Play();
      } else if (level == 2) {
        superchargedParticles.Play();
      }
    }

    protected override void OnChargeReleased() {
      chargedParticles.Stop();
      superchargedParticles.Stop();
    }
  }
}