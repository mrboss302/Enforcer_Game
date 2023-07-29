using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetData : MonoBehaviour
{

    public void Reset()
    {
        SoundManager.Click();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

       
    }
}
