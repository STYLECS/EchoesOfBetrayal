using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Score Settings")]
    [SerializeField] private int skor = 0;
    [SerializeField] private Text skorText;

    [Header("Mobile UI")]
    [SerializeField] private Button fireButton;

    [Header("Respawn Settings")]
    public GameObject playerPrefab;
    public Transform respawnPoint;
    private GameObject currentPlayer;

    private PlayerShoot playerShoot;

    private void Awake()
    {
        // ===== SINGLETON PER SCENE (AMAN TANPA DontDestroyOnLoad) =====
        instance = this;
    }

    private void OnDestroy()
    {
        // 🔥 bersihkan instance saat scene ditutup
        if (instance == this)
            instance = null;
    }

    private void Start()
    {
        FindPlayerShoot();

        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (resetButton) resetButton.onClick.AddListener(RestartGame);
        if (exitButton) exitButton.onClick.AddListener(QuitGame);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(BackToMainMenu);

        if (fireButton)
            fireButton.onClick.RemoveAllListeners();

        UpdateSkorUI();
    }

    // ================= PLAYER =================
    private void FindPlayerShoot()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        currentPlayer = player;
        playerShoot = player.GetComponent<PlayerShoot>();
    }

    public void KillPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        StartCoroutine(RespawnDelay());
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(1f);
        SafeRespawn();
    }

    private void SafeRespawn()
    {
        if (!playerPrefab || !respawnPoint) return;

        currentPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
    }

    // ================= SCORE =================
    public void TambahSkor(int jumlah)
    {
        skor += jumlah;
        UpdateSkorUI();
    }

    private void UpdateSkorUI()
    {
        if (skorText)
            skorText.text = "Skor: " + skor;
    }

    // ================= GAME OVER =================
    public void GameOver()
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
