using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

namespace DreamState {
  [RequireComponent(typeof(CinemachineVirtualCamera))]
  public class SimpleShake : MonoBehaviour {
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    private Coroutine currentShake;

    public void Shake(float duration, float amplitude, float frequency) {
      StopShake();
      currentShake = StartCoroutine(DoShake(duration, amplitude, frequency));
    }

    public void StopShake() {
      if (currentShake != null) {
        StopCoroutine(currentShake);
      }
    }

    private void Awake() {
      virtualCamera = GetComponent<CinemachineVirtualCamera>();
      virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private IEnumerator DoShake(float duration, float amplitude, float frequency) {
      virtualCameraNoise.m_AmplitudeGain = amplitude;
      virtualCameraNoise.m_FrequencyGain = frequency;
      yield return new WaitForSeconds(duration);
      virtualCameraNoise.m_AmplitudeGain = 0f;
      virtualCameraNoise.m_FrequencyGain = 0f;
    }
  }
}