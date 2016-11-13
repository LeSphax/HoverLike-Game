using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    private List<Ability> abilities = new List<Ability>();

    public List<AbilityEffect> UpdateAbilities()
    {
        List<AbilityEffect> effects = new List<AbilityEffect>();
        foreach (Ability ability in abilities)
        {
            List<AbilityEffect> result = ability.UpdateAbility();
            ability.ResetInputs();
            if (result != null)
                effects.AddRange(result);
        }
        return effects;
    }

    public void RegisterAbility(Ability ability)
    {
        abilities.Add(ability);
    }

    public void UnregisterAbility(Ability ability)
    {
        abilities.Remove(ability);
    }

    public void Reset()
    {
        abilities.Clear();
    }



}
