using System;
using UnityEngine;

public class SimpleFollow : MonoBehaviour {
  public Transform target;
  
  private Vector3 initPos;

  private void Start() {
    initPos = transform.position;
  }

  private void Update() {
    transform.position = new Vector3(target.position.x, target.position.y, initPos.z);
  }
}