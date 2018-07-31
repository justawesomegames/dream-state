using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Dreamer))]
  [DisallowMultipleComponent]
  public class DreamerAnimator : MonoBehaviour {
    private Dreamer dreamer;
    private PlatformerPhysics2D physics;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    
    private void Awake() {
      dreamer = GetComponent<Dreamer>();
      physics = GetComponent<PlatformerPhysics2D>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
      animator.SetBool("Grounded", physics.Grounded());
      animator.SetFloat("xSpeed", Mathf.Abs(physics.TargetVelocity.x));
      animator.SetFloat("ySpeed", physics.CurrentVelocity.y);
      animator.SetBool("StickingToWall", dreamer.WallStick.StickingToWall);
      animator.SetBool("Dashing", dreamer.Dashing);

      spriteRenderer.flipY = physics.GravityDirection() == 1;


      if (physics.CurrentVelocity.x < 0.0f && facingRight) {
        Flip(false);
      } else if (physics.CurrentVelocity.x > 0.0f && !facingRight) {
        Flip(true);
      }

    }

    private void Flip(bool right) {
      if (right != facingRight) {
        var newScale = transform.localScale;
        newScale.x = right ? 1 : -1;
        transform.localScale = newScale;
      }
      facingRight = right;
    }
  }
}
