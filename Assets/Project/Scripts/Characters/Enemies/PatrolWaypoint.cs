using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamState {
  public class PatrolWaypoint : MonoBehaviour {
    [SerializeField] private BasicPatrol waypointForWho;

    private void Awake() {
      if (waypointForWho == null) {
        Debug.LogWarning("PatrolWaypoint empty!");
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (waypointForWho == null) return;
      if (collision.gameObject != waypointForWho.gameObject) return;
      waypointForWho.Reverse();
    }
  }
}