using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [HideInInspector] public int bulletsRemainAllowed = 1;
    public GameObject muzzleFX;
    
    public GameObject impactObjFx;
    [HideInInspector]  public int bulletsRemain;
    public LayerMask enemyLayer;
    public LayerMask layerCanHit;

    [Header("***SOUND***")]
    public AudioClip beginSound;
    public AudioClip soundShot;
    public AudioClip soundDropShell;
    public AudioClip soundHitFlesh, soundHitObject;
    public AudioClip soundEmpty;
    [ReadOnly] public EnemyController lastEnemyController;
    [ReadOnly] public float lastHitPosX;

    [HideInInspector] public bool allowShooting = false;

    public void Init(int _bulletsRemain)
    {
        bulletsRemain = _bulletsRemain;
        SoundManager.PlaySfx(beginSound, 0.8f);
    }

    private void Awake()
    {
        muzzleFX.gameObject.SetActive(false);
    }

    public void AllowShooting(bool allow)
    {
        allowShooting = allow;
    }

    private void Update()
    {
        if (allowShooting)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(GunCursorUI.Instance.cursorImage.transform.position);
         
            if (Physics.Raycast(ray, out hit, 100, enemyLayer))
            {
               
                var enemy = hit.collider.GetComponent<EnemyController>();

                if(lastEnemyController && !lastEnemyController.isDead && (lastEnemyController.gameObject!= enemy.gameObject))
                {
                    if (GameManager.Instance.gameState == GameManager.GameState.Playing)
                    {
                        lastEnemyController.Shoot();
                    }
                    allowShooting = false;
                    return;
                }

                if (enemy && !enemy.isDead)
                {
                    lastEnemyController = hit.collider.GetComponent<EnemyController>();
                    lastHitPosX = hit.point.x;
                }
            }
            else if (lastEnemyController)
            {
                //Debug.LogError(GunCursorUI.Instance.cursorImage.transform.position);
                if (!lastEnemyController.isDead && (lastHitPosX > lastEnemyController.transform.position.x))
                {
                    if (GameManager.Instance.gameState == GameManager.GameState.Playing)
                    {
                        lastEnemyController.Shoot();
                    }
                    allowShooting = false;
                }
            }
        }
    }

    public bool Shot()
    {
        if (!allowShooting)
            return false;

        if (bulletsRemain <= 0)
        {
            SoundManager.PlaySfx(soundEmpty);
            return false;
        }

        bulletsRemain--;
        UIBulletHolder.Instance.UpdateBulletRemain(bulletsRemain);
        SoundManager.PlaySfx(soundShot);
        SoundManager.PlaySfx(soundDropShell);
        GunCursorUI.Instance.Action();
        muzzleFX.gameObject.SetActive(false);
        muzzleFX.gameObject.SetActive(true);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(GunCursorUI.Instance.cursorImage.transform.position);

        if (Physics.Raycast(ray,out hit, 100, layerCanHit))
        {
            var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                SoundManager.PlaySfx(soundHitFlesh, 0.6f);
                takeDamage.TakeDamage(1, Vector2.zero, gameObject, hit.point);
            }
            else
            {
                SoundManager.PlaySfx(soundHitObject, 0.6f);
                if (impactObjFx)
                {
                    Instantiate(impactObjFx, hit.point, impactObjFx.transform.rotation);
                    
                }
            }

        }

        return true;
    }
}