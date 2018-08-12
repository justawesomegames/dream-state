using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamState {
  /// <summary>
  /// All valid contexts
  /// A context defines a discrete input space
  /// </summary>
  public enum InputContexts {
    Playing,
    Paused
  }

  /// <summary>
  /// All valid buttons
  /// Should map directly to a Unity input button name
  /// </summary>
  public enum InputButtons {
    Attack,
    Jump,
    Dash,
    Pause
  }

  public enum InputButtonActions {
    Down,
    Hold,
    Up
  }

  /// <summary>
  /// All valid axes
  /// Should map directly to a Unity input axis name
  /// </summary>
  public enum InputAxes {
    Horizontal,
    Vertical
  }

  /// <summary>
  /// Manages input contexts and dispatching raw input to the current context
  /// </summary>
  public class InputManager : Singleton<InputManager> {
    public InputContexts Context { get; private set; }

    private Dictionary<InputContexts, InputContext> contexts;
    private InputContext currentContext;
    public IEnumerable<InputButtons> buttons;
    public IEnumerable<InputAxes> axes;

    public void SetContext(InputContexts newContext) {
      Context = newContext;
      contexts.TryGetValue(newContext, out currentContext);
    }

    public void RegisterEvent(InputContexts context, InputButtons button, InputButtonActions action, Action callback) {
      InputContext addToContext;
      if (!contexts.TryGetValue(context, out addToContext)) {
        Debug.LogError(String.Format("Input Manager doesn't have {0} context. That's not supposed to happen.", context.ToString()));
        return;
      }
      addToContext.RegisterButton(button, action, callback);
    }

    public void RegisterEvent(InputContexts context, InputAxes axis, Action<float> callback) {
      currentContext.RegisterAxis(axis, callback);
    }

    public InputManager() {
      InitializeContexts();
      InitializeInputs();
    }

    private void InitializeContexts() {
      contexts = new Dictionary<InputContexts, InputContext>();
      foreach (var context in Enum.GetValues(typeof(InputContexts))) {
        contexts.Add((InputContexts)context, new InputContext());
      }

      SetContext(InputContexts.Playing);
    }
    
    private void InitializeInputs() {
      buttons = Enum.GetValues(typeof(InputButtons)).Cast<InputButtons>();
      axes = Enum.GetValues(typeof(InputAxes)).Cast<InputAxes>();
    }

    private void Update() {
      // Check all buttons
      foreach(var button in buttons) {
        if (Input.GetButtonDown(button.ToString())) {
          currentContext.ButtonDown(button);
        }
        if (Input.GetButton(button.ToString())) {
          currentContext.ButtonHold(button);
        }
        if (Input.GetButtonUp(button.ToString())) {
          currentContext.ButtonUp(button);
        }
      }

      // Check all axes
      foreach(var axis in axes) {
          currentContext.Axis(axis, Input.GetAxis(axis.ToString()));
      }
    }
  }
}