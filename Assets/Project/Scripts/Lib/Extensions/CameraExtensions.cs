using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DreamState {
  public static class CameraExtensions {
    public static CinemachineBrain Brain(this Camera camera) {
      return camera.GetComponent<CinemachineBrain>();
    }

    public static void SimpleShakeCurrentVCam(this Camera camera, float duration, float amplitude, float frequency) {
      var brain = camera.Brain();
      if (brain == null) {
        Debug.LogWarning("Couldn't find CinemachineBrain on camera.");
        return;
      };
      var shake = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<SimpleShake>();
      if (shake == null) {
        shake = brain.ActiveVirtualCamera.VirtualCameraGameObject.AddComponent<SimpleShake>();
      }
      shake.Shake(duration, amplitude, frequency);
    }
  }
}