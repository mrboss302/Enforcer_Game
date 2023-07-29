using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineController : MonoBehaviour
{
    public static CinemachineController Instance;

    void Start()
    {
        Instance = this;
    }

    public void SetBlendTime(float time)
    {
        GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = time;
    }
}
