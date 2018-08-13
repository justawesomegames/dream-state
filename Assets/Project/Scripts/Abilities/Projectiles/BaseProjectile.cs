﻿using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Rigidbody2D))]
  public abstract class BaseProjectile : MonoBehaviour {
    [SerializeField] protected Vector2 velocity = Vector2.right;
    [SerializeField] protected LayerMask destroysOnWhat;

    private SpriteRenderer spriteRenderer;
    private float damage;
    private GameObject caster;
    private bool facingRight = true;
    private List<string> damagesWhat;

    public void Initialize(GameObject caster, BaseAbility ability, List<string> damagesWhat) {
      damage = ability.Damage;
      this.caster = caster;
      this.damagesWhat = damagesWhat;

      var casterSpriteRenderer = caster.GetComponent<SpriteRenderer>();
      if (casterSpriteRenderer != null) {
        facingRight = !casterSpriteRenderer.flipX;
      }

      if (spriteRenderer != null) {
        spriteRenderer.flipX = !facingRight;
      }

      AfterInitialize();
    }

    public virtual void AfterInitialize() {
      if (!facingRight) {
        velocity.x *= -1;
      }
    }

    public virtual void Step() {
      transform.Translate(velocity * Time.deltaTime);
    }

    private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
      // If out of bounds, just destroy this projectile
      if (spriteRenderer != null && !spriteRenderer.IsVisibleFrom(Camera.main)) {
        DestroyProjectile();
        return;
      }
      Step();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      var hit = collision.gameObject;
      // Ignore collisions with the character who casted this projectile
      if (hit == caster) return;

      // Destroy if colliding with whatever destroys this
      if (destroysOnWhat == (destroysOnWhat | (1 << hit.layer))) {
        DestroyProjectile();
        return;
      }

      // Damage the object if it has stats
      if (damagesWhat.Contains(hit.tag)) {
        var stats = hit.GetComponent<CharacterStats>();
        if (stats != null) {
          stats.Damage(damage);
          DestroyProjectile();
        }
      }
    }

    private void DestroyProjectile() {
      // TODO: Object pooling
      Destroy(gameObject);
    }
  }
}