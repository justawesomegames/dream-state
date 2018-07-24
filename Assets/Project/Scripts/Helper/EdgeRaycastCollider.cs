using System;
using UnityEngine;

namespace DreamState {
  [RequireComponent(typeof(Transform))]
  [RequireComponent(typeof(BoxCollider2D))]
  public class EdgeRaycastCollider : MonoBehaviour {
    #region Public
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] [Range(0.0f, 1.0f)] private float tolerance = 0.98f;
    public EdgeCollider Top;
    public EdgeCollider Bottom;
    public EdgeCollider Left;
    public EdgeCollider Right;
    #endregion

    private void Awake() {
      Top = new EdgeCollider(
        gameObject,
        (t, c) => { return new Vector2(t.position.x + (c.offset.x * t.localScale.x), t.position.y + (c.size.y / 2) + c.offset.y); },
        (c) => { return new Vector2(c.size.x * tolerance, 0); }
      );
      Bottom = new EdgeCollider(
        gameObject,
        (t, c) => { return new Vector2(t.position.x + (c.offset.x * t.localScale.x), t.position.y - (c.size.y / 2) - c.offset.y); },
        (c) => { return new Vector2(c.size.x * tolerance, 0); }
      );
      Right = new EdgeCollider(
        gameObject,
        (t, c) => { return new Vector2(t.position.x + (c.size.x / 2) + (c.offset.x * t.localScale.x), t.position.y + c.offset.y); },
        (c) => { return new Vector2(0, c.size.y * tolerance); }
      );
      Left = new EdgeCollider(
        gameObject,
        (t, c) => { return new Vector2(t.position.x - (c.size.x / 2) + (c.offset.x * t.localScale.x), t.position.y + c.offset.y); },
        (c) => { return new Vector2(0, c.size.y * tolerance); }
      );
    }

    private void FixedUpdate() {
      Top.Update();
      Bottom.Update();
      Left.Update();
      Right.Update();
    }

    public class EdgeCollider {
      private GameObject gameObject;
      private EdgeRaycastCollider collider;
      private BoxCollider2D boxCollider;
      private bool colliding;
      private Func<Transform, BoxCollider2D, Vector2> position;
      private Func<BoxCollider2D, Vector2> size;
      private Action<bool> onChange;

      public EdgeCollider(GameObject g, Func<Transform, BoxCollider2D, Vector2> pFunc, Func<BoxCollider2D, Vector2> sFunc) {
        gameObject = g;
        collider = g.GetComponent<EdgeRaycastCollider>();
        boxCollider = g.GetComponent<BoxCollider2D>();
        position = pFunc;
        size = sFunc;
      }

      public void Update() {
        var newColliding = false;
        var colliders = Physics2D.OverlapBoxAll(position(gameObject.transform, boxCollider), size(boxCollider), 0, collider.collisionLayer);
        foreach(var col in colliders) {
          if (col.gameObject != gameObject) {
            newColliding = true;
          }
        }

        if (newColliding != colliding) {
          colliding = newColliding;
          if (onChange != null) {
            onChange(newColliding);
          }
        }
      }

      public bool IsColliding() {
        return colliding;
      }

      public void RegisterOnChange(Action<bool> a) {
        onChange = a;
      }
    }
  }
}
