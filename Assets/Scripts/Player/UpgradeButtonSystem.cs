using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UpgradeButtonSystem : MonoBehaviour
{
    enum UpgradeType
    {
        Reflect,
        Dash,
        Heal,
    }
    [SerializeField] UpgradeType upgradeMode;

    [SerializeField] private Sprite icon;

    private ReflectUpgradeValues ChosenReflectUpgrade;
    private DashUpgradeValues ChosenDashUpgrade;
    private HealUpgradeValues ChosenHealUpgrade;

    PlayerControlScript playerControlInfo;
    PlayerManager playerManagerInfo;

    private void SetupButton()
    {
        foreach (Transform g in GetComponentsInChildren<Transform>())
        {
            switch (g.name)
            {
                case "Image":
                    if (icon != null)
                        g.transform.GetComponent<Image>().sprite = icon;
                    break;
                case "Title":
                    g.transform.GetComponent<TextMeshProUGUI>().text = GetUpgradeTitle();
                    break;
                case "Details":
                    g.transform.GetComponent<TextMeshProUGUI>().text = GetUpgradeDesc();
                    break;
                default:
                    g.transform.GetComponent<Button>().onClick.AddListener(() => {
                        ApplyUpgradeViaButton();
                        LevelManager.GetInstance().ExitUpgradeScene();
                        LevelManager.GetInstance().LoadRandomLevel();
                        }
                    );
                    break;
            }
        }
    }

    void Start()
    {
        Random.InitState((Mathf.RoundToInt(Time.realtimeSinceStartup)));
        playerManagerInfo = PlayerManager.GetInstance();
        playerControlInfo = PlayerControlScript.GetInstance();
        switch (upgradeMode)
        {
            case UpgradeType.Reflect:
                ChosenReflectUpgrade = GenerateRandomReflectAndStaminaUpgrades();
                break;
            case UpgradeType.Dash:
                ChosenDashUpgrade = GenerateRandomDashUpgrades();
                break;
            case UpgradeType.Heal:
                ChosenHealUpgrade = GenerateRandomHealUpgrades();
                break;
            default:
                Debug.LogError("Upgrade Not Avaiable!");
                break;
        }
        SetupButton();
    }   

    public void ApplyUpgradeViaButton()
    {
        Debug.Log("DONE " + this.name);
        switch (upgradeMode)
        {
            case UpgradeType.Reflect:
                ApplyReflectUpgrades(ChosenReflectUpgrade);
                break;
            case UpgradeType.Dash:
                ApplyDashUpgrades(ChosenDashUpgrade);
                break;
            case UpgradeType.Heal:
                ApplyHealUpgrades(ChosenHealUpgrade);
                break;
            default:
                Debug.LogError("Upgrade Not Avaiable!");
                break;
        }
    }
    public string GetUpgradeTitle()
    {
        switch (upgradeMode)
        {
            case UpgradeType.Dash:
                return "Dash";
            case UpgradeType.Heal:
                return "Heal";
            case UpgradeType.Reflect:
                return "Reflect";
            default:
                return "None";
        }
    }

    public string GetUpgradeDesc()
    {
        switch (upgradeMode)
        {
            case UpgradeType.Dash:
                return ChosenDashUpgrade.UpgradeDescription;
            case UpgradeType.Heal:
                return ChosenHealUpgrade.UpgradeDescription;
            case UpgradeType.Reflect:
                return ChosenReflectUpgrade.UpgradeDescription;
            default:
                return "None";
        }
    }

    #region Dash Upgrades
    public enum DashUpgradeType
    {
        DashCharge,
        DashSpeed,
        DashCooldown,
        BaseMovementSpeed
    }

    struct DashUpgradeValues
    {
        public float increaseSpeed;
        public float reduceCooldown;
        public float increaseWalkSpeed;
        public string UpgradeDescription;
        public DashUpgradeType UpgradeID;

        public DashUpgradeValues(DashUpgradeType id, float speed, float cooldown, float walkspeed, string desc)
        {
            UpgradeID = id;
            increaseSpeed = speed;
            reduceCooldown = cooldown;
            UpgradeDescription = desc;
            increaseWalkSpeed = walkspeed;
        }
    }

    DashUpgradeValues GenerateRandomDashUpgrades()
    {
        DashUpgradeType randomUpgrade = (DashUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(DashUpgradeType)).Length);
        string UpgradeDescription = "";
        float IncreaseSpeed = 0;
        float ReduceCooldown = 0;
        float MovementSpeedBonus = 0;
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
            case DashUpgradeType.BaseMovementSpeed:
                MovementSpeedBonus = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Movement Speed by {MovementSpeedBonus:F2}%";
                break;
            default:
                break;

        }
        DashUpgradeValues retval = new DashUpgradeValues(randomUpgrade, IncreaseSpeed, ReduceCooldown, MovementSpeedBonus, UpgradeDescription);
        return retval;
    }

    void ApplyDashUpgrades(DashUpgradeValues upgrade)
    {
        DashUpgradeType selected_upgrade = upgrade.UpgradeID;
        switch (selected_upgrade)
        {
            case DashUpgradeType.DashCharge:
                playerControlInfo.dashMaxCharge += 1;
                break;

            case DashUpgradeType.DashSpeed:
                playerControlInfo.dashSpeed *= (1 + upgrade.increaseSpeed / 100);
                break;

            case DashUpgradeType.DashCooldown:
                playerControlInfo.dashCooldown *= (1 - upgrade.reduceCooldown / 100);
                break;

            case DashUpgradeType.BaseMovementSpeed:
                playerControlInfo.movementspeed *= (1 + upgrade.increaseWalkSpeed / 100);
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
        BaseHealing,
        BaseHP
    }

    struct HealUpgradeValues
    {
        public HealUpgradeType upgradeID;
        public float VialCapacity;
        public float VialDrain;
        public float BaseHealing;
        public float RawHPBonus;
        public string UpgradeDescription;
        public int ChooseHealingID;

        public HealUpgradeValues(HealUpgradeType upgrade, float capac, float drain, float baseheal, float rawHP, string desc, int healmode)
        {
            upgradeID = upgrade;
            VialCapacity = capac;
            VialDrain = drain;
            BaseHealing = baseheal;
            UpgradeDescription = desc;
            ChooseHealingID = healmode;
            RawHPBonus = rawHP;
        }
    }

    HealUpgradeValues GenerateRandomHealUpgrades()
    {
        HealUpgradeType randomUpgrade = (HealUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(HealUpgradeType)).Length);
        string UpgradeDescription = "";

        // Declare variables to store upgrade values
        float capac = 0f;
        float drain = 0f;
        float baseheal = 0f;
        float bonusHP = 0f;
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

            case HealUpgradeType.BaseHP:
                bonusHP = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase HP by {bonusHP:F2}%";
                break;

            default:
                break;
        }

        HealUpgradeValues retval = new HealUpgradeValues(randomUpgrade, capac, drain, baseheal, bonusHP, UpgradeDescription, ChooseHealingID);
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
                float newCapacity = Mathf.Round(playerManagerInfo.maxVial * (1 + upgradeValues.VialCapacity / 100));
                playerManagerInfo.maxVial = newCapacity;
                break;

            case HealUpgradeType.VialResourceDrain:
                playerManagerInfo.useVial *= (1 - upgradeValues.VialDrain / 100);
                break;

            case HealUpgradeType.BaseHealing:
                playerControlInfo.normalHeal *= (1 + upgradeValues.BaseHealing / 100);
                playerControlInfo.normalHeal = Mathf.Round(playerControlInfo.normalHeal);
                break;

            case HealUpgradeType.BaseHP:
                float newHP = Mathf.Round(playerManagerInfo.maxHealth * (1 + upgradeValues.RawHPBonus / 100));
                playerManagerInfo.maxHealth = newHP;
                break;

            default:
                break;
        }
    }

    string ApplySpecificHeal(int ChooseHealingID)
    {
        string UpgradeDescription = "";
        if (ChooseHealingID == playerControlInfo.HealID)
        {
            if (ChooseHealingID == 2) //AoE healing HealActionManager id 2
            {
                float IncreaseRadius = Random.Range(10f, 30f);
                float IncreaseAoeHeal = Random.Range(10f, 30f);
                float CutAoeHealTime = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Overall AOE Total heal by {IncreaseAoeHeal:F2}%";
                playerControlInfo.aoeHealRadius *= (1f + IncreaseRadius / 100);
                playerControlInfo.aoeHealTime *= (1f - CutAoeHealTime / 100);
                playerControlInfo.aoeHealTotal *= (1f + IncreaseAoeHeal / 100);
            }
            else if (ChooseHealingID == 3)//HealActionManager Disintegration
            {
                float IncreaseDisintegrationDuration = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Disintegration Shield Duration by {IncreaseDisintegrationDuration:F2}%";
                playerControlInfo.DisintegrationDuration *= (1f + IncreaseDisintegrationDuration / 100);
            }
            else if (ChooseHealingID == 4)//HealActionManager ReflectShield
            {
                UpgradeDescription = $"Unlocks Reflecting Shield, Increase Shield Durability by 1";
                playerControlInfo.ReflectShieldHP += 1;
            }

        }
        else
        {
            playerControlInfo.HealID = ChooseHealingID;
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
        StaminaDecay,
        BulletSteal
    }

    struct ReflectUpgradeValues
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
    ReflectUpgradeValues GenerateRandomReflectAndStaminaUpgrades()
    {
        string UpgradeDescription = "";
        ReflectUpgradeType randomUpgrade = (ReflectUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(ReflectUpgradeType)).Length);

        if (randomUpgrade.Equals(ReflectUpgradeType.BulletSteal))
        {
            if(playerManagerInfo.bulletCaptureLimit <= 1)
            {
                randomUpgrade = (ReflectUpgradeType)Random.Range(0, System.Enum.GetValues(typeof(ReflectUpgradeType)).Length-1);
            }
        }

        float IncreaseRegenRate = 0;
        float ReduceDecayRate = 0;
        float finalStamina = playerManagerInfo.maxStamina;
        switch (randomUpgrade)
        {
            case ReflectUpgradeType.StaminaCapacity:
                float IncreaseCapacity = Random.Range(10f, 30f);
                UpgradeDescription = $"Increase Stamina Capacity by {IncreaseCapacity:F2}%";
                finalStamina = playerManagerInfo.maxStamina * (1 + IncreaseCapacity / 100);
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
                if (playerManagerInfo.multiply)
                {
                    UpgradeDescription = $"Increase Bullet Multiplication by 1";
                }
                else
                {
                    UpgradeDescription = $"Enable Bullet Multiplication";
                }
                break;
            case ReflectUpgradeType.RotatingShield:
                if (!playerControlInfo.mirrorRotate)
                {
                    UpgradeDescription = $"Enable Rotating Shields";
                }
                else
                {
                    UpgradeDescription = $"Increase Rotating Shields by 1";
                }
                break;
            case ReflectUpgradeType.BulletSteal:
                UpgradeDescription = $"Reduce Bullet Steal Activation from {playerManagerInfo.bulletCaptureLimit} by 1";
                break;
            default:
                Debug.LogError("Not an available "+ GetUpgradeTitle() + " upgrade choice.");
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
                playerManagerInfo.ModifyStaminaCapacity(Mathf.Round(upgradeValues.StaminaCapacity));
                break;
            case ReflectUpgradeType.StaminaRegen:
                playerControlInfo.stamina_regen *= (1 + upgradeValues.StaminaRegen / 100);
                break;
            case ReflectUpgradeType.StaminaDecay:
                playerControlInfo.stamina_decay *= (1 - upgradeValues.StaminaDecay / 100);
                break;

            case ReflectUpgradeType.Duplication:
                if (playerManagerInfo.multiply)
                {
                    playerManagerInfo.bulletMultiplier += 1;
                }
                else
                {
                    playerManagerInfo.multiply = true;
                }
                break;

            case ReflectUpgradeType.RotatingShield:
                if (!playerControlInfo.mirrorRotate)
                {
                    playerControlInfo.mirrorRotate = true;
                    playerControlInfo.numShields += 1;
                    playerControlInfo.CreateOrbitingShields();
                }
                else
                {
                    playerControlInfo.numShields += 1;
                    playerControlInfo.CreateOrbitingShields();
                }
                break;

            case ReflectUpgradeType.BulletSteal:
                if(playerManagerInfo.bulletCaptureLimit > 1)
                    playerManagerInfo.bulletCaptureLimit -= 1;
                break;

            default:
                break;
        }
    }

    #endregion
}
