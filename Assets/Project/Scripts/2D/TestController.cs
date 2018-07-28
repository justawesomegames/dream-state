using DreamState.Global;
using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(PhysicsObject2D))]
  public class TestController : MonoBehaviour {
    #region Public
    [SerializeField] private float jumpForce = 0.1f;
    [SerializeField] private float moveSpeed = 0.2f;
    [SerializeField] private WallStick wallStick;
    #endregion

    #region Internal
    private PhysicsObject2D physics;
    #endregion

    private void Awake() {
      physics = GetComponent<PhysicsObject2D>();
      physics.RegisterModifier(wallStick);
    }

    private void Update() {
      if (physics.Grounded() && Input.GetButtonDown(Constants.Input.JUMP)) {
        physics.Jump(jumpForce);
      }
      var hInput = Input.GetAxis(Global.Constants.Input.HORIZONTAL_AXIS);
      if (hInput != 0.0f) physics.Move(Vector2.right * moveSpeed * hInput);
    }
  }
}
