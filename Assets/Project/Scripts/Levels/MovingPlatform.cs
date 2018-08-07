using System;
using UnityEngine;
using UnityEditor;

namespace DreamState {
  [DisallowMultipleComponent]
  public class MovingPlatform : MovingObject2D {
    [SerializeField] private Vector3[] waypoints = new Vector3[1];
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool cyclic;
    [SerializeField] private float waitTime;
    [SerializeField] [Range(0, 2)] private float easeAmount;

    private Vector3[] globalWaypoints;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;

    private void Start() {
      globalWaypoints = new Vector3[waypoints.Length];
      for (var i = 0; i < waypoints.Length; i++) {
        globalWaypoints[i] = waypoints[i] + transform.position;
      }
    }

    private float Ease(float x) {
      var a = easeAmount + 1;
      return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    protected override Vector3 CalculateNewVelocity() {
      if (Time.time < nextMoveTime) {
        return Vector3.zero;
      }

      fromWaypointIndex %= globalWaypoints.Length;
      var toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
      var distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
      percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
      percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
      var easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

      var newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

      if (percentBetweenWaypoints >= 1) {
        percentBetweenWaypoints = 0;
        fromWaypointIndex++;

        if (!cyclic) {
          if (fromWaypointIndex >= globalWaypoints.Length - 1) {
            fromWaypointIndex = 0;
            Array.Reverse(globalWaypoints);
          }
        }
        nextMoveTime = Time.time + waitTime;
      }

      return newPos - transform.position;
    }

    private void OnDrawGizmos() {
      if (waypoints == null) return;

      Gizmos.color = Color.red;
      var size = 0.3f;

      for (var i = 0; i < waypoints.Length; i++) {
        var globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : waypoints[i] + transform.position;
        Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
        Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
        Handles.Label(globalWaypointPos, i.ToString());
      }
    }
  }
}