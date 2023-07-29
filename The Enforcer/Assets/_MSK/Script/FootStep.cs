using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    public AudioClip soundFootStep;
    [Range(0f, 1f)]
    public float soundFootStepVolume = 0.5f;
    
    public void Sound()
    {
        SoundManager.PlaySfx(soundFootStep, soundFootStepVolume);
    }
}
