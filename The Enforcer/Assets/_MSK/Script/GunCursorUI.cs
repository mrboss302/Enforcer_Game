using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCursorUI : MonoBehaviour
{
    public static GunCursorUI Instance;
    public GameObject cursorImage;
    Animator anim;

    void Start()
    {
        Instance = this;
        anim = GetComponent<Animator>();
    }

    public void Action()
    {
        anim.SetTrigger("action");
    }

    void Update()
    {
        cursorImage.SetActive(GameManager.Instance.Player.playerGun.allowShooting);
    }
}