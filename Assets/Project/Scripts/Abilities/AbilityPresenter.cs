using System;
using UnityEngine;

namespace DreamState.Abilities {
  [CreateAssetMenu(menuName = "Ability")]
  [Serializable]
  public class AbilityPresenter : ScriptableObject {
    public string DisplayName;
    public string Description;
    // Icon
  }
}