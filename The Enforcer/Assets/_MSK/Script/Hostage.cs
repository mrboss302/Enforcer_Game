using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostage : MonoBehaviour, ICanTakeDamage
{
    public enum START_STATE { Stand, Crouch }
    public START_STATE startState;
    public float activeWhenPlayerNear = 2f;
    public Animator anim;
    public AudioClip[] soundDie;
    public float soundDieVolume = 0.5f;
    public GameObject bloodFX;
    bool isDead = false;

    public GameObject vcamFront;

    void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (startState == START_STATE.Stand)
            anim.SetBool("stand", true);

        StartCoroutine(WaitingForActionCo());
    }

    IEnumerator WaitingForActionCo()
    {
        while (Mathf.Abs(transform.position.x - GameManager.Instance.Player.transform.position.x) > activeWhenPlayerNear) { yield return null; }

        anim.SetBool("stand", true);
    }

    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isDead)
            return;

        StopAllCoroutines();
        SoundManager.PlaySfx(soundDie[Random.Range(0, soundDie.Length)], soundDieVolume);
        isDead = true;
        anim.SetTrigger("die");

        if (bloodFX)
            Instantiate(bloodFX, hitPoint, Quaternion.identity);


        vcamFront.SetActive(true);
        GameManager.Instance.GameOver();
    }

}
