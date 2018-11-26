using System;
using System.Linq;
using System.Collections.Generic;

namespace DreamState {
  public class InputContext {
    private Dictionary<InputButtons, InputEvent> buttonDownEvents;
    private Dictionary<InputButtons, InputEvent> buttonHoldEvents;
    private Dictionary<InputButtons, InputEvent> buttonUpEvents;
    private Dictionary<InputAxes, InputEvent> axisEvents;

    public InputContext() {
      buttonDownEvents = new Dictionary<InputButtons, InputEvent>();
      buttonHoldEvents = new Dictionary<InputButtons, InputEvent>();
      buttonUpEvents = new Dictionary<InputButtons, InputEvent>();
      axisEvents = new Dictionary<InputAxes, InputEvent>();
    }

    public void RegisterButton(InputButtons button, InputButtonActions action, Action callback) {
      switch (action) {
        case InputButtonActions.Down:
          buttonDownEvents.Add(button, new InputEvent(callback));
          break;
        case InputButtonActions.Hold:
          buttonHoldEvents.Add(button, new InputEvent(callback));
          break;
        case InputButtonActions.Up:
          buttonUpEvents.Add(button, new InputEvent(callback));
          break;
      }
    }

    public void RegisterAxis(InputAxes axis, Action<float> callback) {
      axisEvents.Add(axis, new InputEvent(callback));
    }

    public void DisableButton(InputButtons button, InputButtonActions action) {
      switch (action) {
        case InputButtonActions.Down:
          buttonDownEvents.First(b => b.Key == button).Value.Disable();
          break;
        case InputButtonActions.Hold:
          buttonHoldEvents.First(b => b.Key == button).Value.Disable();
          break;
        case InputButtonActions.Up:
          buttonUpEvents.First(b => b.Key == button).Value.Disable();
          break;
      }
    }

    public void DisableAxis(InputAxes axis) {
      axisEvents.First(a => a.Key == axis).Value.Disable();
    }

    public void EnableButton(InputButtons button, InputButtonActions action) {
      switch (action) {
        case InputButtonActions.Down:
          buttonDownEvents.First(b => b.Key == button).Value.Enable();
          break;
        case InputButtonActions.Hold:
          buttonHoldEvents.First(b => b.Key == button).Value.Enable();
          break;
        case InputButtonActions.Up:
          buttonUpEvents.First(b => b.Key == button).Value.Enable();
          break;
      }
    }

    public void EnableAxis(InputAxes axis) {
      axisEvents.First(a => a.Key == axis).Value.Enable();
    }

    public void ButtonDown(InputButtons button) {
      InputEvent ie;
      if (buttonDownEvents.TryGetValue(button, out ie)) {
        ie.Trigger();
      }
    }

    public void ButtonHold(InputButtons button) {
      InputEvent ie;
      if (buttonHoldEvents.TryGetValue(button, out ie)) {
        ie.Trigger();
      }
    }

    public void ButtonUp(InputButtons button) {
      InputEvent ie;
      if (buttonUpEvents.TryGetValue(button, out ie)) {
        ie.Trigger();
      }
    }

    public void Axis(InputAxes axis, float amt) {
      InputEvent ie;
      if (axisEvents.TryGetValue(axis, out ie)) {
        ie.Trigger(amt);
      }
    }
  }
}