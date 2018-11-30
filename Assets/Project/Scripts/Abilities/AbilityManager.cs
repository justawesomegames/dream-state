using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DreamState.Abilities;

/// <summary>
/// The AbilityManager provides type safety in casting abilities for an object.
/// In doing so, the AbilityManager acts as a list of known abilities for the object.
/// </summary>
namespace DreamState {
  public class AbilityManager : MonoBehaviour {
    public void Cast<T>() where T : Ability {
      var ability = GetAbility<T>();
      if (ability == null) {
        return;
      }
      ability.Cast();
    }

    public void Charge<T>() where T : Ability {
      var ability = GetAbility<T>();
      if (ability == null) {
        return;
      }
      ability.Charge();
    }

    public void ReleaseCharge<T>() where T : Ability {
      var ability = GetAbility<T>();
      if (ability == null) {
        return;
      }
      ability.ReleaseCharge();
    }

    private Ability GetAbility<T>() where T : Ability {
      var ability = GetComponent<T>();
      if (ability == null) {
        Debug.LogWarning(string.Format("{0} does not know {1}.", gameObject.name, typeof(T).ToString()));
        return null;
      }
      return ability as T;
    }
  }
}