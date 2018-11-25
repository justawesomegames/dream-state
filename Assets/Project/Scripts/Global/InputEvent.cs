using System;
using UnityEngine;

namespace DreamState {
  public class InputEvent {
    private Action voidEvent;
    private Action<float> floatEvent;
    private bool enabled;

    public InputEvent(Action action, bool enabled = true) {
      voidEvent = action;
      this.enabled = enabled;
    }

    public InputEvent(Action<float> action, bool enabled = true) {
      floatEvent = action;
      this.enabled = enabled;
    }

    public void Enable() {
      enabled = true;
    }

    public void Disable() {
      enabled = false;
    }

    public void Trigger() {
      if (!enabled) return;
      voidEvent();
    }

    public void Trigger(float amt) {
      if (!enabled) return;
      floatEvent(amt);
    }
  }
}