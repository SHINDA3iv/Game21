using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    //�������� �����
    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }

    //����� �� ����������
    public void ExitGame()
    {
        Application.Quit();    
    }
}