using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public GameObject interactionPanel;
    public TMP_Text titleText;
    public Button cleanButton;
    public TMP_Text notificationText;

    [Header("Score UI")]
    public TMP_Text currentScoreText;     // contoh: "4/10"
    public TMP_Text totalCleanedText;     // contoh: "Total dibersihkan: 42"

    // Counters
    private int currentCleanCount = 0;    // jumlah yang dibersihkan di ronde sekarang
    private int totalCleanedCount = 0;    // total kumulatif (disimpan kalau pakai PlayerPrefs)
    private int totalTrashThisRound = 10; // jumlah sampah di ronde ini

    [Header("Persistence")]
    public bool usePlayerPrefs = true;    
    public string prefsKey = "TotalCleanedAllTime";

    private TrashInteract currentTrash;

    [Header("References")]
    public TrashSpawner trashSpawner;     // 🎯 reference ke spawner

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        if (notificationText != null)
            notificationText.gameObject.SetActive(false);

        if (cleanButton != null)
            cleanButton.onClick.AddListener(OnCleanClicked);
    }

    void Start()
    {
        if (usePlayerPrefs)
            totalCleanedCount = PlayerPrefs.GetInt(prefsKey, 0);

        if (trashSpawner == null)
            trashSpawner = FindObjectOfType<TrashSpawner>();

        if (trashSpawner != null)
            totalTrashThisRound = trashSpawner.trashCount;

        UpdateScoreUI();
        StartCoroutine(RefreshCountsNextFrame());
    }

    IEnumerator RefreshCountsNextFrame()
    {
        yield return null; // tunggu 1 frame biar spawner selesai spawn
        RefreshCountsFromScene();
        UpdateScoreUI();
    }

    // Hitung ulang hanya jumlah trash, TIDAK reset progress bersih
    void RefreshCountsFromScene()
    {
        var allTrash = FindObjectsOfType<TrashInteract>();
        totalTrashThisRound = allTrash.Length;
    }

    // Dipanggil dari TrashInteract ketika sampah dibersihkan
    public void AddCleaned(int amount = 1)
    {
        currentCleanCount += amount;
        totalCleanedCount += amount;

        if (usePlayerPrefs)
            PlayerPrefs.SetInt(prefsKey, totalCleanedCount);

        UpdateScoreUI();
        ShowNotification("Sampah dibersihkan!");
    }

    // Update tampilan teks
    private void UpdateScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = currentCleanCount + "/" + totalTrashThisRound;

        if (totalCleanedText != null)
            totalCleanedText.text = "Total dibersihkan: " + totalCleanedCount;
    }

    // Panel interaksi
    public void ShowInteraction(TrashInteract trash)
    {
        currentTrash = trash;
        if (interactionPanel != null)
            interactionPanel.SetActive(true);
        if (titleText != null)
            titleText.text = "N*o Sampah";
    }

    private void OnCleanClicked()
    {
        if (currentTrash != null)
        {
            currentTrash.CleanTrash();
            if (interactionPanel != null)
                interactionPanel.SetActive(false);

            currentTrash = null;
        }
    }

    // Notifikasi singkat
    public void ShowNotification(string message)
    {
        if (notificationText == null) return;

        notificationText.gameObject.SetActive(true);
        notificationText.text = message;

        CancelInvoke(nameof(HideNotification));
        Invoke(nameof(HideNotification), 2f);
    }

    private void HideNotification()
    {
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    // 🔄 Fungsi Reset: hapus + spawn ulang trash
    public void ResetGame()
    {
        if (trashSpawner != null)
        {
            trashSpawner.ResetTrash(); // hapus & spawn ulang
        }

        // Reset hitungan ronde
        currentCleanCount = 0;

        // Tunggu 1 frame biar sampah lama benar-benar hilang sebelum hitung ulang
        StartCoroutine(RefreshCountsAfterReset());

        ShowNotification("Sampah diacak ulang!");
    }

    private IEnumerator RefreshCountsAfterReset()
    {
        yield return null;
        RefreshCountsFromScene();
        UpdateScoreUI();
    }

    // ❌ Fungsi Exit Game
    public void ExitGame()
    {
        Debug.Log("Kembali ke menu...");

        SceneManager.LoadScene("menu");
    }

    // 🔄 Reset total kumulatif (opsional, misal untuk debug)
    public void ResetTotalCleanedPersistent()
    {
        totalCleanedCount = 0;
        if (usePlayerPrefs)
            PlayerPrefs.DeleteKey(prefsKey);
        UpdateScoreUI();
    }
}
