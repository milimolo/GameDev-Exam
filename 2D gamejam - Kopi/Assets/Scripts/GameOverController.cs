using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    int sceneToLoad = 3;
    int oneMeansLostFight = 1;

    public void TryAgain()
    {
        PlayerPrefs.SetInt("Lost", oneMeansLostFight);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
