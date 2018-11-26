using UnityEngine;

namespace DreamState {
  public class GameManager : Singleton<GameManager> {
    public bool Paused { get; private set; }

    public void Pause() {
      Time.timeScale = 0.0f;
      InputManager.Instance.SetContext(InputContexts.Paused);
      Paused = true;
    }

    public void Unpause() {
      Time.timeScale = 1.0f;
      InputManager.Instance.SetContext(InputContexts.Playing);
      Paused = false;
    }
  }
}