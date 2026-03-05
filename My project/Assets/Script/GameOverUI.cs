using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public Button resetButton;
    public Button exitButton;

    private void Start()
    {
        gameOverPanel.SetActive(false);

        resetButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(QuitGame);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void QuitGame()
    {
        Debug.Log("Keluar Game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

