using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("prolog");
    }

    public void OpenOptions()
    {
        Debug.Log("Options dibuka");
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari game!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}   
