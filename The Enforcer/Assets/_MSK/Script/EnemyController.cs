using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour,ICanTakeDamage
{
    public enum START_STATE { Stand, Crouch, Prone}
    public START_STATE startState = START_STATE.Stand;
    public enum MOVING { None, toProne, toCrouch, toStand, toRightSide, toLeftSide}
    public MOVING action;
    [Header("---MOVING---")]
    public float activeWhenPlayerNear = 1.5f;
    public float moveDistance = 2;
    public float moveSpeed = 2;

    [Space]
    public Animator anim;
    [ReadOnly] public bool isDead = false;
    public GameObject muzzleFX;
    public GameObject vcamBack;
    public AudioClip soundFire;
    public float soundFireVolume = 0.6f;
    public AudioClip[] soundDie;
    public float soundDieVolume = 0.5f;
    public GameObject bloodFX;

    void Start()
    {
        vcamBack.SetActive(false);
        muzzleFX.SetActive(false);

        if (anim == null)
            anim = GetComponent<Animator>();

        if (startState == START_STATE.Crouch)
            anim.SetBool("isCrouching", true);
        else if (startState == START_STATE.Prone)
        {
            anim.SetBool("prone", true);
        }

        if (action != MOVING.None)
            StartCoroutine(WaitingForActionCo());
    }

    IEnumerator WaitingForActionCo()
    {
        while (Mathf.Abs(transform.position.x - GameManager.Instance.Player.transform.position.x) > activeWhenPlayerNear) { yield return null; }
        Vector3 originalPos;
        Vector3 targetPos;

        switch (action)
        {
            case MOVING.toCrouch:
                anim.SetBool("isCrouching", true);
                anim.SetBool("prone", false);
                break;
            case MOVING.toProne:
                anim.SetBool("prone", true);
                anim.SetBool("isCrouching", false);
                break;
            case MOVING.toStand:
                anim.SetBool("isCrouching", false);
                anim.SetBool("prone", false);
                break;
            default:
                originalPos = transform.position;
                targetPos = originalPos + transform.right * moveDistance * (action == MOVING.toLeftSide ? -1 : 1);
                anim.SetBool("isMovingRight", action == MOVING.toRightSide);
                anim.SetFloat("speed", 1);
                while (true)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime / Time.timeScale);
                    yield return null;

                    if (transform.position.x == targetPos.x)
                    {
                        anim.SetFloat("speed", 0);
                        break;
                    }
                }
                break;
        }
    }


    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isDead)
            return;

        StopAllCoroutines();
        SoundManager.PlaySfx(soundDie[Random.Range(0, soundDie.Length)], soundDieVolume);
        isDead = true;
        anim.SetInteger("deadType", Random.Range(1, 4));
        GetComponent<CapsuleCollider>().enabled = false;
        if (bloodFX)
            Instantiate(bloodFX, hitPoint, Quaternion.identity);
    }

    public void Shoot()
    {
        StartCoroutine(ShootCo());
        StartCoroutine(LookAtPlayerCo());
    }

    IEnumerator ShootCo()
    {
        GameManager.Instance.Player.PrepareToDie();
        CinemachineController.Instance.SetBlendTime(0.2f);
        Time.timeScale = 0.05f;
        vcamBack.SetActive(true);
        yield return new WaitForSeconds(1 * Time.timeScale);
        anim.SetTrigger("shoot");
        muzzleFX.SetActive(false);
        muzzleFX.SetActive(true);
        SoundManager.PlaySfx(soundFire, soundFireVolume);
        yield return new WaitForSeconds(1f * Time.timeScale);
        vcamBack.SetActive(false);
        GameManager.Instance.Player.Die();
    }

    IEnumerator LookAtPlayerCo()
    {
        while (true)
        {
            transform.LookAt(GameManager.Instance.Player.transform);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if(action == MOVING.toLeftSide || action == MOVING.toRightSide)
        {
            var originalPos = transform.position + Vector3.up * 1;
            var targetPos = originalPos + transform.right * moveDistance * (action == MOVING.toLeftSide?-1:1);

            Gizmos.DrawLine(originalPos, targetPos);
            Gizmos.DrawCube(targetPos, new Vector3(0.5f, 2, 1));
        }
    }
}
