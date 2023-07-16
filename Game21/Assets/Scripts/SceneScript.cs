using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneScript : MonoBehaviour
{
    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }

    public void ExitGame()
    {
        Application.Quit();    // закрыть приложение
    }
}
