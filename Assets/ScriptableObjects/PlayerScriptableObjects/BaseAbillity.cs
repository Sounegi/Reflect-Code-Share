using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BaseAbillity : ScriptableObject
{
    public string upgrade_name;
    public string upgrade_description;
    public int upgradeID;
    //public bool onCooldown;
    //public bool isActive;
    //public bool ready;

    public enum AbilityStatus
    {
        COOLDOWN, READY, ACTIVE
    };
    public AbilityStatus Ability_Status;
}
