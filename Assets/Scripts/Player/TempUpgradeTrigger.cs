using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempUpgradeTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public string upgrade_mode;
    public HealUpgradeValues ChosenHealUpgrade;
    public ReflectUpgradeValues ChosenReflectUpgrade;
    public DashUpgradeValues ChosenDashUpgrade;
    PlayerControlScript player_control_info;
    PlayerManager player_manager_info;

    public void GenerateUpgrades()
    {
        player_manager_info = PlayerManager.GetInstance();
        player_control_info = PlayerControlScript.GetInstance();

        ChosenDashUpgrade = GenerateDashUpgrades();
        ChosenHealUpgrade = GenerateHealUpgrades();
        ChosenReflectUpgrade = GenerateReflectAndStaminaUpgrades();
    }

    public string GetUpgradeDesc()
    {
        if (upgrade_mode == "reflect")
            return ChosenReflectUpgrade.UpgradeDescription;
        else if (upgrade_mode == "dash")
            return ChosenDashUpgrade.UpgradeDescription;
        else if (upgrade_mode == "heal")
            return ChosenHealUpgrade.UpgradeDescription;
        else
            return "None";
    }

    #region DashUpgrade
    public enum DashUpgradeType
    {
        DashCharge,
        DashSpeed,
        DashCooldown,
        DashCastTime
    }

    public struct DashUpgradeValues
    {
        public float increaseSpeed;
        public float reduceCooldown;
        public float dashCastTime;
        public string UpgradeDescription;
        public DashUpgradeType UpgradeID; 

        public DashUpgradeValues(DashUpgradeType id, float speed, float cooldown, float casttime, string desc)
        {
            UpgradeID = id;
            increaseSpeed = speed;
            reduceCooldown = cooldown;
            UpgradeDescription = desc;
            dashCastTime = casttime;
        }
    }

    DashUpgradeValues GenerateDashUpgrades()
    {
        DashUpgradeType randomUpgrade = (DashUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(DashUpgradeType)).Length);
        string UpgradeDescription ="";
        float IncreaseSpeed = 0;
        float ReduceCooldown = 0;
        float ReduceCastTime = 0;
        switch (randomUpgrade)
        {
            case DashUpgradeType.DashCharge:
                UpgradeDescription = "Increase Dash Charge by 1.";
                break;

            case DashUpgradeType.DashSpeed:
                IncreaseSpeed = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Dash Speed by {IncreaseSpeed:F2}%";
                break;

            case DashUpgradeType.DashCooldown:
                ReduceCooldown = Random.Range(10f, 30f);
                UpgradeDescription = $"Reduce Dash Cooldown by {ReduceCooldown:F2}%";
                break;

            case DashUpgradeType.DashCastTime:
                ReduceCastTime = Random.Range(10f, 30f);
                if (player_control_info.dashID == 2)
                {
                    UpgradeDescription = $"Reduce Blink CastTime by {ReduceCastTime:F2}%";
                }
                else
                {
                    UpgradeDescription = $"Change into Blink Dash";
                }
                break;
            default:
                break;

        }
        Debug.Log("chosen dash upgrade is: "+ UpgradeDescription);
        DashUpgradeValues retval = new DashUpgradeValues(randomUpgrade, IncreaseSpeed, ReduceCooldown, ReduceCastTime, UpgradeDescription);
        return retval;
    }

    void ApplyDashUpgrades(DashUpgradeValues upgrade)
    {
        DashUpgradeType selected_upgrade = upgrade.UpgradeID;
        switch (selected_upgrade)
        {
            case DashUpgradeType.DashCharge:
                player_control_info.dashMaxCharge += 1;
                break;

            case DashUpgradeType.DashSpeed:
                player_control_info.dashSpeed *= (1 + upgrade.increaseSpeed / 100);
                break;

            case DashUpgradeType.DashCooldown:
                player_control_info.dashCooldown *= (1 - upgrade.reduceCooldown / 100);
                break;

            case DashUpgradeType.DashCastTime:
                if (player_control_info.dashID == 2)
                {
                    player_control_info.dashCastTime *= (1 - upgrade.dashCastTime / 100);
                }
                else
                {
                    player_control_info.dashID = 2;
                }
                break;
            default:
                break;

        }
    }
    #endregion

    #region Heal Upgrades
    public enum HealUpgradeType
    {
        HealID,
        VialCapacity,
        VialResourceDrain,
        BaseHealing
    }

    public struct HealUpgradeValues
    {
        public HealUpgradeType upgradeID;
        public float VialCapacity;
        public float VialDrain;
        public float BaseHealing;
        public string UpgradeDescription;
        public int ChooseHealingID;

        public HealUpgradeValues(HealUpgradeType upgrade, float capac, float drain, float baseheal, string desc, int healmode)
        {
            upgradeID = upgrade;
            VialCapacity = capac;
            VialDrain = drain;
            BaseHealing = baseheal;
            UpgradeDescription = desc;
            ChooseHealingID = healmode;
        }
    }

    HealUpgradeValues GenerateHealUpgrades()
    {
        HealUpgradeType randomUpgrade = (HealUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(HealUpgradeType)).Length);
        string UpgradeDescription = "";

        // Declare variables to store upgrade values
        float capac = 0f;
        float drain = 0f;
        float baseheal = 0f;
        int ChooseHealingID = 2;

        // Generate upgrade values based on the upgrade type
        switch (randomUpgrade)
        {
            case HealUpgradeType.HealID:
                ChooseHealingID = Random.Range(2, 5);
                switch (ChooseHealingID)
                {
                    case 2:
                        UpgradeDescription = $"Turn Heal into AoE Healing";
                        break;
                    case 3:
                        UpgradeDescription = $"Turn Heal into Disintegration Shield";
                        break;
                    case 4:
                        UpgradeDescription = $"Turn Heal into Reflecting Shield";
                        break;
                    default:
                        break;
                }
                break;

            case HealUpgradeType.VialCapacity:
                capac = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Vial Capacity by {capac:F2}%";
                break;

            case HealUpgradeType.VialResourceDrain:
                drain = Random.Range(10f, 30f);
                UpgradeDescription = $"Reduce Vial Usage by {drain:F2}%";
                break;

            case HealUpgradeType.BaseHealing:
                baseheal = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Base Healing by {baseheal:F2}%";
                break;

            default:
                break;
        }

        HealUpgradeValues retval = new HealUpgradeValues(randomUpgrade, capac, drain, baseheal, UpgradeDescription, ChooseHealingID);
        return retval;
    }

    void ApplyHealUpgrades(HealUpgradeValues upgradeValues)
    {
        HealUpgradeType chosenupgrade = upgradeValues.upgradeID;
        switch (chosenupgrade)
        {
            case HealUpgradeType.HealID:
                ApplySpecificHeal(upgradeValues.ChooseHealingID);
                break;
            case HealUpgradeType.VialCapacity:
                player_manager_info.maxVial *= (1 + upgradeValues.VialCapacity / 100);
                break;

            case HealUpgradeType.VialResourceDrain:
                player_manager_info.useVial *= (1 - upgradeValues.VialDrain / 100);
                break;

            case HealUpgradeType.BaseHealing:
                player_control_info.normalHeal *= (1 + upgradeValues.BaseHealing / 100);
                player_control_info.normalHeal = Mathf.Round(player_control_info.normalHeal);
                break;

            default:
                break;
        }
    }

    string ApplySpecificHeal(int ChooseHealingID)
    {
        string UpgradeDescription ="";
        if (ChooseHealingID == player_control_info.HealID)
        {
            if (ChooseHealingID == 2) //AoE healing HealActionManager id 2
            {
                float IncreaseRadius = Random.Range(10f, 30f);
                float IncreaseAoeHeal = Random.Range(10f, 30f);
                float CutAoeHealTime = Random.Range(10f, 30f);
                player_control_info.aoeHealRadius *= (1f + IncreaseRadius / 100);
                player_control_info.aoeHealTime *= (1f - CutAoeHealTime / 100);
                player_control_info.aoeHealTotal *= (1f + IncreaseAoeHeal / 100);
            }
            else if (ChooseHealingID == 3)//HealActionManager Disintegration
            {
                float IncreaseDisintegrationDuration = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Disintegration Shield Duration by {IncreaseDisintegrationDuration}%";
                player_control_info.DisintegrationDuration *= (1f + IncreaseDisintegrationDuration / 100);
            }
            else if (ChooseHealingID == 4)//HealActionManager ReflectShield
            {
                UpgradeDescription = $"Unlocks Reflecting Shield, Increase Shield Durability by 1";
                player_control_info.ReflectShieldHP += 1;
            }

        }
        else
        {
            player_control_info.HealID = ChooseHealingID;
            switch (ChooseHealingID)
            {
                case 2:
                    UpgradeDescription = $"Unlocks AoE Healing";
                    break;
                case 3:
                    UpgradeDescription = $"Unlocks Disintegration Shield";
                    break;
                case 4:
                    UpgradeDescription = $"Unlocks Reflecting Shield";
                    break;
            }

        }
        return UpgradeDescription;
    }


    #endregion

    #region Reflect and Stamina Upgrades
    public enum ReflectUpgradeType
    {
        RotatingShield,
        Duplication,
        StaminaRegen,
        StaminaCapacity,
        StaminaDecay
    }

    public struct ReflectUpgradeValues
    {
        public float StaminaRegen;
        public float StaminaCapacity;
        public float StaminaDecay;
        public string UpgradeDescription;
        public ReflectUpgradeType UpgradeID;

        public ReflectUpgradeValues(ReflectUpgradeType id, float regen, float capac, float decay, string desc)
        {
            UpgradeID = id;
            StaminaRegen = regen;
            StaminaCapacity = capac;
            StaminaDecay = decay;
            UpgradeDescription = desc;
        }
    }
    ReflectUpgradeValues GenerateReflectAndStaminaUpgrades()
    {
        string UpgradeDescription = "";
        ReflectUpgradeType randomUpgrade = (ReflectUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(ReflectUpgradeType)).Length);
        float IncreaseRegenRate = 0;
        float ReduceDecayRate = 0;
        float finalStamina = player_manager_info.maxStamina;
        switch (randomUpgrade)
        {
            case ReflectUpgradeType.StaminaCapacity:
                float IncreaseCapacity = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Stamina Capacity by {IncreaseCapacity:F2}%";
                finalStamina = player_manager_info.maxStamina * (1 + IncreaseCapacity / 100);
                break;
            case ReflectUpgradeType.StaminaRegen:
                IncreaseRegenRate = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Stamina Regeneration by {IncreaseRegenRate:F2}%";
                break;
            case ReflectUpgradeType.StaminaDecay:
                ReduceDecayRate = Random.Range(10f, 30f);
                UpgradeDescription = $"Reduce Stamina Usage by {ReduceDecayRate:F2}%";
                break;
            case ReflectUpgradeType.Duplication:
                if (player_manager_info.multiply)
                {
                    UpgradeDescription = $"Increase Bullet Multiplication by 1";
                }
                else
                {
                    UpgradeDescription = $"Enable Bullet Multiplication";
                }
                break;
            case ReflectUpgradeType.RotatingShield:
                if (!player_control_info.mirrorRotate)
                {
                    UpgradeDescription = $"Enable Rotating Shields";
                }
                else
                {
                    UpgradeDescription = $"Increase Rotating Shields by 1";
                }
                break;
        }
        ReflectUpgradeValues retval = new ReflectUpgradeValues(randomUpgrade, IncreaseRegenRate, finalStamina, ReduceDecayRate, UpgradeDescription);
        return retval;
    }

    void ApplyReflectUpgrades(ReflectUpgradeValues upgradeValues)
    {
        ReflectUpgradeType selectedUpgrade = upgradeValues.UpgradeID;
        switch (selectedUpgrade)
        {
            case ReflectUpgradeType.StaminaCapacity:
                player_manager_info.ModifyStaminaCapacity(Mathf.Round(upgradeValues.StaminaCapacity));
                break;
            case ReflectUpgradeType.StaminaRegen:
                player_control_info.stamina_regen *= (1 + upgradeValues.StaminaRegen / 100);
                break;

            case ReflectUpgradeType.StaminaDecay:
                player_control_info.stamina_decay *= (1 - upgradeValues.StaminaDecay / 100);
                break;

            case ReflectUpgradeType.Duplication:
                if (player_manager_info.multiply)
                {
                    player_manager_info.bulletMultiplier += 1;
                }
                else
                {
                    player_manager_info.multiply = true;
                }
                break;

            case ReflectUpgradeType.RotatingShield:
                if (!player_control_info.mirrorRotate)
                {
                    player_control_info.mirrorRotate = true;
                    player_control_info.numShields += 1;
                    player_control_info.CreateOrbitingShields();
                }
                else
                {
                    player_control_info.numShields += 1;
                    player_control_info.CreateOrbitingShields();
                }
                break;

            default:
                break;
        }
    }

    #endregion

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(upgrade_mode == "reflect")
            {
                Debug.Log("Player accessing reflect upgrade");
                Debug.Log("Player is currently locked to using: " + randomUpgrade);
                ApplyReflectUpgrades(ChosenReflectUpgrade);
                Debug.Log("Upgrade Applied");
            }
            else if(upgrade_mode == "heal")
            {
                Debug.Log("Player accessing heal upgrade");
                Debug.Log("Player is currently locked to using: " + randomHealUpgrade);
                ApplyHealUpgrades(ChosenHealUpgrade);
                Debug.Log("Upgrade Applied");
            }
            else if (upgrade_mode == "dash")
            {
                Debug.Log("Player accessing dash upgrade");
                Debug.Log("Player is currently locked to using: " + randomDashUpgrade);
                ApplyDashUpgrades(ChosenDashUpgrade);
                Debug.Log("Upgrade Applied");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (upgrade_mode == "reflect")
            {
                Debug.Log("Randomizing Reflect Upgrade");
                ChosenReflectUpgrade = GenerateReflectAndStaminaUpgrades();
                //Debug.Log("Player is currently locked to using: " + randomUpgrade);
            }
            else if (upgrade_mode == "heal")
            {
                Debug.Log("Randomizing Heal Upgrade");
                ChosenHealUpgrade = GenerateHealUpgrades();
                //Debug.Log("Player is currently locked to using: " + randomHealUpgrade);
            }
            else if (upgrade_mode == "dash")
            {
                Debug.Log("Randomizing Heal Upgrade");
                ChosenDashUpgrade = GenerateDashUpgrades();
                //Debug.Log("Player is currently locked to using: " + randomDashUpgrade);
            }
        }
    }

    public void ApplyUpgradeViaButton()
    {
        if (upgrade_mode == "reflect")
        {
            ApplyReflectUpgrades(ChosenReflectUpgrade);
            Debug.Log("finish upgrade reflect");
        }
        else if (upgrade_mode == "dash")
        {
            ApplyDashUpgrades(ChosenDashUpgrade);
            Debug.Log("finish upgrade dash");
        }
        else if (upgrade_mode == "heal")
        {
            ApplyHealUpgrades(ChosenHealUpgrade);
            Debug.Log("finish upgrade heal");
        }
    }
    */
}
/*

dependency:
string upgrade_mode

on enable:
GenerateReflectAndStaminaUpgrades();
GenerateHealUpgrades();
GenerateDashUpgrades();

ApplyReflectUpgrades(ChosenReflectUpgrade);

 
callback list:
ApplyReflectUpgrades(ChosenReflectUpgrade);
ApplyDashUpgrades(ChosenDashUpgrade);
ApplyHealUpgrades(ChosenHealUpgrade);



 */