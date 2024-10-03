using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamController : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        
        vcam.Follow = PlayerControlScript.GetInstance().gameObject.transform;
    }
}
