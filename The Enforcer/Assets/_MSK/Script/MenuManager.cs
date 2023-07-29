using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public GameObject uI, gameOver, finish, LoadingUI;
    public Text[] txtLevels;
    public GameObject[] butNext;
    public GameObject beginText;
    [Header("Progressing Bar")]
    public Slider progressSlider;
    float startPos, finishPos;

    #region STARS
    [Header("Sound and Music")]
    public Image soundImage;
    public Image musicImage;
    public Sprite soundImageOn, soundImageOff, musicImageOn, musicImageOff;

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        uI.SetActive(false);
        gameOver.SetActive(false);
        
        LoadingUI.SetActive(false);
        beginText.SetActive(true);

        if (Time.timeScale == 0)
            Time.timeScale = 1;

        if (GameManager.Instance)
            startPos = GameManager.Instance.Player.transform.position.x;

        var hasFinishPoint = GameObject.FindGameObjectWithTag("Finish");
        if (hasFinishPoint)
            finishPos = hasFinishPoint.transform.position.x;
        else
            Debug.LogError("NO FINISH POINT ON SCENE!");

        foreach (var txt in txtLevels)
        {
            if (GlobalValue.levelPlaying == -1)
                txt.text = "TEST ALL FEATURES";
            else
                txt.text = "Level " + GlobalValue.levelPlaying;
        }

        if (soundImage)
            soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
        if (musicImage)
            musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
        if (!GlobalValue.isSound)
            SoundManager.SoundVolume = 0;
        if (!GlobalValue.isMusic)
            SoundManager.MusicVolume = 0;
    }

    public void Play()
    {
        uI.SetActive(true);
        beginText.SetActive(false);
    }

    #region Music and Sound
    public void TurnSound()
    {
        GlobalValue.isSound = !GlobalValue.isSound;
        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

        SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
        SoundManager.Click();
    }

    public void TurnMusic()
    {
        GlobalValue.isMusic = !GlobalValue.isMusic;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

        SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
        SoundManager.Click();
    }
    #endregion

    void Update()
    {
        progressSlider.value = Mathf.InverseLerp(startPos, finishPos, GameManager.Instance.Player.transform.position.x);
    }

    public void Finish()
    {
        Invoke("FinishCo", 2);
    }

    void FinishCo()
    {
        foreach (var but in butNext)
        {
            but.SetActive(GlobalValue.levelPlaying < GlobalValue.LevelHighest);
        }

        uI.SetActive(false);
        finish.SetActive(true);
    }

    public void GameOver()
    {
        Invoke("GameOverCo", 1);
    }

    void GameOverCo()
    {
        foreach (var but in butNext)
        {
            but.SetActive(GlobalValue.levelPlaying < GlobalValue.LevelHighest);
        }

        uI.SetActive(false);
        gameOver.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        GlobalValue.levelPlaying++;
        LoadingUI.SetActive(true);
        SceneManager.LoadSceneAsync("Level " + GlobalValue.levelPlaying);
    }

    public void Home()
    {
        GlobalValue.levelPlaying = -1;
        LoadingUI.SetActive(true);
        SceneManager.LoadSceneAsync("HomeScene");
    }
}
