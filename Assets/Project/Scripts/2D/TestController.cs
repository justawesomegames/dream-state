using DreamState.Global;
using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(PhysicsObject2D))]
  public class TestController : MonoBehaviour {
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private WallStick wallStick;
    
    private PhysicsObject2D physics;

    private void Awake() {
      physics = GetComponent<PhysicsObject2D>();
      physics.RegisterModifier(wallStick);
    }

    private void Update() {
      if (Input.GetButtonDown(Constants.Input.JUMP)) {
        if (wallStick.StickingToWall) {
          wallStick.JumpOffWall();
        } else if(physics.Grounded()) {
          physics.SetVelocity(Vector2.up * jumpForce);
        }
      }
      var hInput = Input.GetAxis(Global.Constants.Input.HORIZONTAL_AXIS);
      if (hInput != 0.0f) physics.Move(Vector2.right * moveSpeed * hInput);
    }
  }
}
