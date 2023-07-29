using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBarrel : MonoBehaviour,ICanTakeDamage
{
    public float radius = 5;
    public GameObject explosionFX;
    public AudioClip sound;
    bool isWorked = false;
    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isWorked)
            return;

        isWorked = true;
        SoundManager.PlaySfx(sound);
        Instantiate(explosionFX, transform.position, explosionFX.transform.rotation);
        var hits = Physics.OverlapSphere(transform.position, radius);
        foreach(var hit in hits)
        {
            var takeDamage = (ICanTakeDamage)hit.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                takeDamage.TakeDamage(1, Vector2.zero, gameObject, hit.gameObject.transform.position);
            }
        }

        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
