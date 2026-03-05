using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerLevelComplete : MonoBehaviour
{
    public static GameManagerLevelComplete instance;

    [Header("Level Selesai UI")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton; // ✅ tombol main menu

    [Header("Level Settings")]
    [SerializeField] private string nextLevelName;
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // ✅ nama scene menu

    private int totalEnemies = 0;
    private int defeatedEnemies = 0;
    private int totalCoins = 0;
    private int collectedCoins = 0;

    private bool levelSelesai = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (levelCompletePanel)
            levelCompletePanel.SetActive(false);

        if (nextLevelButton)
            nextLevelButton.onClick.AddListener(NextScene);

        if (mainMenuButton)
            mainMenuButton.onClick.AddListener(LoadMainMenu); // ✅
    }

    // ================= ENEMY & COIN =================
    public void RegisterEnemy() => totalEnemies++;
    public void RegisterCoin() => totalCoins++;

    public void EnemyDefeated()
    {
        defeatedEnemies++;
        CheckProgress();
    }

    public void CoinCollected()
    {
        collectedCoins++;
        CheckProgress();
    }

    private void CheckProgress()
    {
        if (!levelSelesai &&
            defeatedEnemies >= totalEnemies &&
            collectedCoins >= totalCoins)
        {
            ShowLevelCompleteUI();
        }
    }

    private void ShowLevelCompleteUI()
    {
        levelSelesai = true;

        if (levelCompletePanel)
        {
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void NextScene()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextLevelName))
            SceneManager.LoadScene(nextLevelName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // ================= MAIN MENU =================
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
