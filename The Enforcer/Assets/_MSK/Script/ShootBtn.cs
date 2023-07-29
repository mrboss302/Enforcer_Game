using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBtn : MonoBehaviour
{
    public void Shoot()
    {
        GameManager.Instance.Player.Shoot();
    }
}
