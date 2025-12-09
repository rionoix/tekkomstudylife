using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RectTransformData
{
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta = new Vector2(100, 100);
}

[System.Serializable]
public class ValidCombination
{
    [Tooltip("Kombinasi tombol (True/False). Pastikan urutannya sesuai toggle.")]
    public bool[] states = new bool[6];
}

public enum LevelType
{
    TogglePuzzle,
    TextOnly
}

[System.Serializable]
public class LevelData
{
    public string levelName;

    [Header("Tipe Level")]
    public LevelType levelType = LevelType.TogglePuzzle;

    [Header("Gambar utama")]
    public Sprite levelImage;
    public RectTransformData levelImageLayout;

    [Header("Sprite status")]
    public Sprite statusSpriteA; // Belum benar
    public Sprite statusSpriteB; // Benar
    public RectTransformData statusImageLayout;

    [Header("Pengaturan Toggle")]
    [Range(0, 6)]
    public int toggleCount = 0;

    [Header("Daftar Kombinasi Jawaban Benar")]
    public ValidCombination[] validCombinations; 

    public RectTransformData[] toggleLayouts = new RectTransformData[6];

    [Header("Level selingan (TextOnly)")]
    [TextArea(2, 4)]
    public string intermissionText;
}

public class LevelManager : MonoBehaviour
{
    [Header("--- EDITOR PREVIEW TOOLS ---")]
    public bool livePreview = true;
    public int previewLevelIndex = 0;

    [Header("Referensi UI Utama")]
    public Image levelImageUI;
    public Image statusImageUI;
    public Toggle[] toggles; 
    public TextMeshProUGUI intermissionTextUI;
    
    [Header("Tombol Navigasi")]
    public Button nextLevelButton; // Tombol Next biasa

    [Header("Tombol End Game (Muncul saat tamat)")]
    public GameObject endGamePanel;   // Panel pembungkus tombol tamat (opsional)
    public Button restartButton;      // Tombol Ulang dari 0
    public Button startLevel10Button; // Tombol Mulai Level 10

    [Header("Data Level")]
    public LevelData[] levels;

    private int currentLevelIndex = 0;

    private void Start()
    {
        // Setup Listener Tombol End Game secara otomatis
        if (restartButton != null) 
            restartButton.onClick.AddListener(() => LoadLevel(0));

        if (startLevel10Button != null) 
            startLevel10Button.onClick.AddListener(() => LoadLevel(10)); // Akan error jika level < 11, tapi aman ada cek di LoadLevel

        // Setup Listener Toggle
        if (toggles != null)
        {
            foreach (var t in toggles)
            {
                if (t != null) 
                    t.onValueChanged.AddListener(delegate { OnToggleChanged(); });
            }
        }

        LoadLevel(0);
    }

    public void LoadLevel(int index)
    {
        // Validasi Index
        if (index < 0 || index >= levels.Length) 
        {
            Debug.LogError($"Level {index} tidak ditemukan! Pastikan jumlah level di inspector cukup.");
            return;
        }

        currentLevelIndex = index;
        LevelData level = levels[currentLevelIndex];

        // 1. Reset Semua Tombol Navigasi (Sembunyikan semua dulu)
        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);
        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (startLevel10Button != null) startLevel10Button.gameObject.SetActive(false);

        // 2. Setup Tampilan Level
        if (level.levelType == LevelType.TextOnly)
            SetupTextOnlyLevel(level);
        else 
            SetupTogglePuzzleLevel(level);
    }

    private void SetupTextOnlyLevel(LevelData level)
    {
        if (levelImageUI != null) levelImageUI.gameObject.SetActive(false);
        if (statusImageUI != null) statusImageUI.gameObject.SetActive(false);

        if (toggles != null)
            foreach (var t in toggles) if (t != null) t.gameObject.SetActive(false);

        if (intermissionTextUI != null)
        {
            intermissionTextUI.gameObject.SetActive(true);
            intermissionTextUI.text = level.intermissionText;
        }

        // Kalau text only, langsung cek apakah ini level terakhir?
        CheckLevelComplete(true);
    }

    private void SetupTogglePuzzleLevel(LevelData level)
    {
        if (levelImageUI != null)
        {
            levelImageUI.gameObject.SetActive(true);
            levelImageUI.sprite = level.levelImage;
            ApplyRect(levelImageUI.rectTransform, level.levelImageLayout);
        }

        if (statusImageUI != null)
        {
            statusImageUI.gameObject.SetActive(true);
            statusImageUI.sprite = level.statusSpriteA;
            ApplyRect(statusImageUI.rectTransform, level.statusImageLayout);
        }

        if (intermissionTextUI != null)
            intermissionTextUI.gameObject.SetActive(false);

        for (int i = 0; i < toggles.Length; i++)
        {
            bool aktif = i < level.toggleCount;
            if (toggles[i] == null) continue;

            toggles[i].gameObject.SetActive(aktif);

            if (aktif)
            {
                if (Application.isPlaying) toggles[i].isOn = false; 
                ApplyRect(toggles[i].transform as RectTransform, level.toggleLayouts[i]);
            }
        }
    }

    private void ApplyRect(RectTransform rt, RectTransformData data)
    {
        if (rt == null || data == null) return;
        rt.anchoredPosition = data.anchoredPosition;
        rt.sizeDelta = data.sizeDelta;
    }

    public void OnToggleChanged()
    {
        if (!Application.isPlaying) return; 

        LevelData level = levels[currentLevelIndex];
        if (level.levelType != LevelType.TogglePuzzle) return;

        CheckCombination();
    }

    private void CheckCombination()
    {
        LevelData level = levels[currentLevelIndex];
        bool isMatched = false;

        if (level.validCombinations != null)
        {
            foreach (var combo in level.validCombinations)
            {
                bool thisComboCorrect = true;
                for (int i = 0; i < level.toggleCount; i++)
                {
                    if (i >= combo.states.Length) break;
                    if (toggles[i].isOn != combo.states[i])
                    {
                        thisComboCorrect = false;
                        break; 
                    }
                }
                if (thisComboCorrect)
                {
                    isMatched = true;
                    break; 
                }
            }
        }

        // Tampilkan feedback visual
        if (statusImageUI != null)
            statusImageUI.sprite = isMatched ? level.statusSpriteB : level.statusSpriteA;

        // Panggil logika tombol
        CheckLevelComplete(isMatched);
    }

    // --- LOGIKA MENAMPILKAN TOMBOL (NEXT vs END GAME) ---
    private void CheckLevelComplete(bool isComplete)
    {
        // Jika belum selesai, sembunyikan semua tombol lanjut
        if (!isComplete)
        {
            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);
            if (endGamePanel != null) endGamePanel.SetActive(false);
            // Tombol restart/level10 dimatikan via endGamePanel atau individual
            if (restartButton != null) restartButton.gameObject.SetActive(false);
            if (startLevel10Button != null) startLevel10Button.gameObject.SetActive(false);
            return;
        }

        // JIKA SELESAI (BENAR):
        // Cek apakah ini level TERAKHIR?
        bool isLastLevel = (currentLevelIndex >= levels.Length - 1);

        if (isLastLevel)
        {
            // Tampilkan UI Tamat
            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false); // Next hilang
            
            if (endGamePanel != null) endGamePanel.SetActive(true); // Panel muncul
            if (restartButton != null) restartButton.gameObject.SetActive(true);
            if (startLevel10Button != null) startLevel10Button.gameObject.SetActive(true);
        }
        else
        {
            // Level biasa, tampilkan Next
            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(true);
            
            if (endGamePanel != null) endGamePanel.SetActive(false); // Panel hilang
            if (restartButton != null) restartButton.gameObject.SetActive(false);
            if (startLevel10Button != null) startLevel10Button.gameObject.SetActive(false);
        }
    }
    // ---------------------------------------------------

    public void OnNextLevelButton()
    {
        // Tombol Next hanya muncul jika bukan level terakhir, jadi aman increment
        LoadLevel(currentLevelIndex + 1);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && livePreview) UpdateEditorPreview();
    }

    private void UpdateEditorPreview()
    {
        if (levels == null || levels.Length == 0) return;
        previewLevelIndex = Mathf.Clamp(previewLevelIndex, 0, levels.Length - 1);
        LevelData levelData = levels[previewLevelIndex];

        if (levelData.levelType == LevelType.TextOnly) SetupTextOnlyLevel(levelData);
        else SetupTogglePuzzleLevel(levelData);
    }
}