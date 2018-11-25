﻿using System.Collections;
using UnityEngine;
using DreamState.Physics;

namespace DreamState {
  [RequireComponent(typeof(PlayerInputController))]
  [RequireComponent(typeof(CharacterStats))]
  public class Dreamer : Character {
    [SerializeField] private float invulnerableAfterDamageTime = 2.0f;
    [SerializeField] private float recoveryAfterDamageTime = 0.5f;
    [SerializeField] private Vector2 damageImpulse = new Vector2(2f, 5f);

    PlayerInputController inputController;
    HorizontalMovement horizontalMovement;
    SpriteFlash spriteFlash;

    public override void OnDamageTaken(float amt) {
      horizontalMovement.Override(true);
      inputController.DisablePlayerInput();
      var newV = damageImpulse;
      newV.x *= curFacingDir == FacingDir.Right ? -1 : 1;
      physics.SetVelocity(newV);
      spriteFlash.StartFlash(invulnerableAfterDamageTime);

      StartCoroutine(TriggerInvulnerability());
      StartCoroutine(TriggerRecovery());
    }

    protected override void OnAwake() {
      inputController = GetComponent<PlayerInputController>();
      horizontalMovement = GetComponent<HorizontalMovement>();
      spriteFlash = GetComponent<SpriteFlash>();
    }

    private IEnumerator TriggerInvulnerability() {
      stats.SetInvulnerable(true);
      yield return new WaitForSeconds(invulnerableAfterDamageTime);
      stats.SetInvulnerable(false);
    }

    private IEnumerator TriggerRecovery() {
      yield return new WaitForSeconds(recoveryAfterDamageTime);

      horizontalMovement.Override(false);
      inputController.EnablePlayerInput();
    }
  }
}
