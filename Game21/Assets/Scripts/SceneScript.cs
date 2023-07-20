using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneScript : MonoBehaviour
{
    //Загрузка сцены
    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }

    //Выход из приложения
    public void ExitGame()
    {
        Application.Quit();    
    }
}
