using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Rigidbody2D))]
  public abstract class BaseProjectile : MonoBehaviour {
    [SerializeField] protected Vector2 velocity = Vector2.right;
    [SerializeField] protected LayerMask destroysOnWhat;

    private float damage;
    private GameObject caster;
    private bool facingRight = true;

    public void Initialize(GameObject caster, BaseAbility ability) {
      damage = ability.Damage;
      this.caster = caster;

      var casterSpriteRenderer = caster.GetComponent<SpriteRenderer>();
      if (casterSpriteRenderer != null) {
        facingRight = !casterSpriteRenderer.flipX;
      }

      var spriteRenderer = GetComponent<SpriteRenderer>();
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

    private void OnEnable() {
      
    }

    private void Update() {
      Step();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      // Ignore collisions with the character who casted this projectile
      if (collision.gameObject == caster) return;

      // Destroy if colliding with whatever destroys this
      if (destroysOnWhat == (destroysOnWhat | (1 << collision.gameObject.layer))) {
        // TODO: Object pooling, just deactivate
        Destroy(gameObject);
        return;
      }

      // TODO: Handle damaging other characters
    }
  }
}