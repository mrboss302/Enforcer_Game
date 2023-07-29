using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBulletHolder : MonoBehaviour
{
    public static UIBulletHolder Instance;
    public Image bulletItemUI;
    public Sprite imageRemain, imageEmpty;
    public Transform bulletContainer;
    [ReadOnly] public List<Image> bulletList;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;
        bulletList = new List<Image>();

        for (int i = 0; i < 20; i++)
        {
            bulletList.Add(Instantiate(bulletItemUI, bulletContainer));
        }
        bulletList.Reverse();

        canvasGroup = bulletContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = bulletContainer.gameObject.AddComponent<CanvasGroup>();
        }

        Destroy(bulletItemUI.gameObject);
        canvasGroup.alpha = 0;
    }
    
    public void Setup()
    {
        foreach(var obj in bulletList)
        {
            obj.gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.Instance.Player.currentTrigger.allowBullet; i++)
        {
            bulletList[i].gameObject.SetActive(true);
        }

        UpdateBulletRemain(GameManager.Instance.Player.currentTrigger.allowBullet);
    }

    //called by PlayerGun
    public void UpdateBulletRemain(int remain)
    {
        for (int i = 0; i < bulletList.Count; i++)
        {
            bulletList[i].sprite = (i <= remain - 1) ? imageRemain : imageEmpty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Player.currentTrigger)
            canvasGroup.alpha = 1;
        else
            canvasGroup.alpha = 0;
    }
}
