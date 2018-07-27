using System;
using UnityEngine;

namespace DreamState {
  using Global;

  [RequireComponent(typeof(PhysicsObject2D))]
  public class TestController : MonoBehaviour {
    #region Public
    [SerializeField] private float jumpForce = 0.1f;
    [SerializeField] private float moveSpeed = 0.2f;
    #endregion

    #region Internal
    private PhysicsObject2D physics;
    #endregion

    private void Awake() {
      physics = GetComponent<PhysicsObject2D>();
    }

    private void Update() {
      if (physics.Grounded() && Input.GetButton(Constants.Input.JUMP)) {
        physics.AddForceAbsolute(Vector2.up * jumpForce);
      }
      var hInput = Input.GetAxis(Global.Constants.Input.HORIZONTAL_AXIS);
      if (hInput != 0.0f) physics.Move(Vector2.right * moveSpeed * hInput);
    }
  }
}
