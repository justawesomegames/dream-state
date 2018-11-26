using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DreamState {
  public static class CameraExtensions {

    public static CinemachineBrain Brain(this Camera camera) {
      return camera.GetComponent<CinemachineBrain>();
    }
  }
}