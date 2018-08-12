using System;
using System.Collections.Generic;

namespace DreamState {
  public class InputContext {
    private Dictionary<InputButtons, Action> buttonDownEvents;
    private Dictionary<InputButtons, Action> buttonHoldEvents;
    private Dictionary<InputButtons, Action> buttonUpEvents;
    private Dictionary<InputAxes, Action<float>> axisEvents;

    public InputContext() {
      buttonDownEvents = new Dictionary<InputButtons, Action>();
      buttonHoldEvents = new Dictionary<InputButtons, Action>();
      buttonUpEvents = new Dictionary<InputButtons, Action>();
      axisEvents = new Dictionary<InputAxes, Action<float>>();
    }

    public void RegisterButton(InputButtons button, InputButtonActions action, Action callback) {
      switch (action) {
        case InputButtonActions.Down:
          buttonDownEvents.Add(button, callback);
          break;
        case InputButtonActions.Hold:
          buttonHoldEvents.Add(button, callback);
          break;
        case InputButtonActions.Up:
          buttonUpEvents.Add(button, callback);
          break;
      }
    }

    public void RegisterAxis(InputAxes axis, Action<float> callback) {
      axisEvents.Add(axis, callback);
    }

    public void ButtonDown(InputButtons button) {
      Action a;
      if (buttonDownEvents.TryGetValue(button, out a)) {
        a();
      }
    }

    public void ButtonHold(InputButtons button) {
      Action a;
      if (buttonHoldEvents.TryGetValue(button, out a)) {
        a();
      }
    }

    public void ButtonUp(InputButtons button) {
      Action a;
      if (buttonUpEvents.TryGetValue(button, out a)) {
        a();
      }
    }

    public void Axis(InputAxes axis, float amt) {
      Action<float> a;
      if (axisEvents.TryGetValue(axis, out a)) {
        a(amt);
      }
    }
  }
}