using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Transform))]
  [RequireComponent(typeof(BoxCollider2D))]
  public class EdgeRaycastCollider : MonoBehaviour {
    #region Public
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] [Range(0.0f, 1.0f)] private float tolerance = 0.98f;
    [SerializeField] protected bool top;
    [SerializeField] protected bool bottom;
    [SerializeField] protected bool left;
    [SerializeField] protected bool right;
    #endregion

    #region Internal
    private BoxCollider2D boxCollider;
    private bool collidingTop;
    private bool collidingBottom;
    private bool collidingLeft;
    private bool collidingRight;
    #endregion

    public bool Top() { return collidingTop; }
    public bool Bottom() { return collidingBottom; }
    public bool Left() { return collidingLeft; }
    public bool Right() { return collidingRight; }

    private void Awake() {
      boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate() {
      var offsetX = boxCollider.offset.x * gameObject.transform.localScale.x;
      var offsetY = boxCollider.offset.y * gameObject.transform.localScale.y;
      if (top) {
        collidingTop = getCollision(
          new Vector2(gameObject.transform.position.x + offsetX, gameObject.transform.position.y + (boxCollider.size.y / 2) + boxCollider.offset.y),
          new Vector2(boxCollider.size.x * tolerance, 0)
        );
      }
      if (bottom) {
        collidingBottom = getCollision(
          new Vector2(gameObject.transform.position.x + offsetX, gameObject.transform.position.y - (boxCollider.size.y / 2) - boxCollider.offset.y),
          new Vector2(boxCollider.size.x * tolerance, 0)
        );
      }
      if (right) {
        collidingRight = getCollision(
          new Vector2(gameObject.transform.position.x + (boxCollider.size.x / 2) + offsetX, gameObject.transform.position.y + boxCollider.offset.y),
          new Vector2(0, boxCollider.size.y * tolerance)
        );
      }
      if (left) {
        collidingLeft = getCollision(
          new Vector2(gameObject.transform.position.x - (boxCollider.size.x / 2) + offsetX, gameObject.transform.position.y + boxCollider.offset.y),
          new Vector2(0, boxCollider.size.y * tolerance)
        );
      }
    }

    private bool getCollision(Vector2 pos, Vector2 size) {
      var colliders = Physics2D.OverlapBoxAll(pos, size, 0, collisionLayer);
      foreach(var col in colliders) {
        if (col.gameObject != gameObject) {
          return true;
        }
      }
      return false;
    }
  }
}
