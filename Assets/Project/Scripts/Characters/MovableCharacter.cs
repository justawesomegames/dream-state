using DreamState.Global;
using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(PhysicsObject2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(SpriteRenderer))]
  public class MovableCharacter : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float maxDashTime = 0.4f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private WallStick wallStick;

    private PhysicsObject2D physics;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool lastJumpPress = false;

    private void Awake() {
      physics = GetComponent<PhysicsObject2D>();
      rigidBody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();

      physics.RegisterModifier(wallStick);
    }

    private void Update() {
      animator.SetBool("Grounded", physics.Grounded());
      animator.SetFloat("xSpeed", Mathf.Abs(physics.CurrentVelocity.x));
      animator.SetFloat("ySpeed", physics.CurrentVelocity.y);
      animator.SetBool("StickingToWall", wallStick.StickingToWall);
    }

    public void HorizontalMove(float moveScalar) {
      var newMove = Vector2.right * runSpeed * moveScalar;
      if (newMove.x != 0.0f) {
        physics.Move(newMove);
        spriteRenderer.flipX = newMove.x < 0.0f;
      }
    }

    public void Jump(bool jump) {
      // To prevent bouncing, can only jump if jumping is not being held
      if (jump && !lastJumpPress) {
        if(wallStick.StickingToWall) {
          wallStick.JumpOffWall();
        } else if (physics.Grounded()) {
          physics.SetVelocity(Vector2.up * jumpForce);
        }
      }

      if (!jump && physics.CurrentVelocity.y > 0.0f) {
        physics.SetVelocity(Vector2.zero);
      }

      lastJumpPress = jump;
    }
  }
}
