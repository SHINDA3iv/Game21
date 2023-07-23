using UnityEngine;
using UnityEngine.SceneManagement;

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