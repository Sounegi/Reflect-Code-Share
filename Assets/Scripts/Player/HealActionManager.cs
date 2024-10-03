using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealActionManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AoEHealing AoeHeal;
    public DisintegrationShield Disintegration;
    public ReflectShield ShieldReflect;
    public GameObject AoEPrefab, ReflectShieldPrefab, DisintegratePrefab;
    public void StartHeal(PlayerControlScript mainController)
    {
        switch (mainController.HealID)
        {
            case 1:
                mainController.NormalPitchSource.PlayOneShot(mainController.normal_healing_audio_clip);
                PlayerManager.GetInstance().Heal(mainController.normalHeal);
                break;
            case 2:
                mainController.NormalPitchSource.PlayOneShot(mainController.aoe_healing_audio_clip);
                AoeHeal.CreateField(AoEPrefab, mainController);
                break;
            case 3:
                mainController.NormalPitchSource.PlayOneShot(mainController.special_healing_audio_clip);
                PlayerManager.GetInstance().Heal(mainController.normalHeal);
                Disintegration.CreateField(DisintegratePrefab, mainController);
                break;
            case 4:
                mainController.NormalPitchSource.PlayOneShot(mainController.special_healing_audio_clip);
                PlayerManager.GetInstance().Heal(mainController.normalHeal);
                ShieldReflect.CreateField(ReflectShieldPrefab, mainController);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
