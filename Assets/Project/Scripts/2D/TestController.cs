namespace DreamState {
  using Global;
  using System;
  using UnityEngine;

  [RequireComponent(typeof(PhysicsObject2D))]
  public class TestController : MonoBehaviour {
    #region Public
    [SerializeField] private float jumpForce = 0.1f;
    [SerializeField] private float moveSpeed = 0.2f;
    #endregion

    #region Internal
    private PhysicsObject2D physics;
    private Rigidbody2D rigidBody;
    #endregion

    private void Awake() {
      physics = GetComponent<PhysicsObject2D>();
      rigidBody = GetComponent<Rigidbody2D>();
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
