using UnityEngine;

namespace DreamState {
  public class BlastProjectile : BaseProjectile {
    private void Update() {
      transform.Translate(staticVelocity * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      Debug.Log(string.Format("I'm colliding with {0}", collision.gameObject.name));
    }
  }
}