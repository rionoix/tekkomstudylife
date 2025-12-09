using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RectTransformData
{
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta = new Vector2(100, 100);
}

// --- KELAS BARU: Untuk menangani kombinasi majemuk di Inspector ---
[System.Serializable]
public class ValidCombination
{
    [Tooltip("Kombinasi tombol (True/False). Pastikan urutannya sesuai toggle.")]
    public bool[] states = new bool[6];
}
// ------------------------------------------------------------------

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

    // --- BAGIAN INI BERUBAH ---
    [Header("Daftar Kombinasi Jawaban Benar")]
    [Tooltip("Isi Element sebanyak kemungkinan jawaban benar.")]
    public ValidCombination[] validCombinations; 
    // ---------------------------

    public RectTransformData[] toggleLayouts = new RectTransformData[6];

    [Header("Level selingan (TextOnly)")]
    [TextArea(2, 4)]
    public string intermissionText;
}

public class LevelManager : MonoBehaviour
{
    // --- EDITOR PREVIEW TOOLS ---
    [Header("--- EDITOR PREVIEW TOOLS ---")]
    public bool livePreview = true;
    public int previewLevelIndex = 0;

    [Header("Referensi UI")]
    public Image levelImageUI;
    public Image statusImageUI;
    public Toggle[] toggles; 
    public Button nextLevelButton;
    public TextMeshProUGUI intermissionTextUI;

    [Header("Data Level")]
    public LevelData[] levels;

    private int currentLevelIndex = 0;

    private void Start()
    {
        // Pasang listener otomatis agar toggle merespons klik
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
        if (index < 0 || index >= levels.Length) return;

        currentLevelIndex = index;
        LevelData level = levels[currentLevelIndex];

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);

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

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
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

    // --- LOGIKA PENGECEKAN BARU ---
    private void CheckCombination()
    {
        LevelData level = levels[currentLevelIndex];
        
        bool isMatched = false; // Penanda apakah ada salah satu kombinasi yang cocok

        // 1. Cek apakah developer lupa mengisi kombinasi?
        if (level.validCombinations == null || level.validCombinations.Length == 0)
        {
            // Kalau kosong, anggap saja salah (atau bisa kamu buat auto-win untuk debug)
            isMatched = false;
        }
        else
        {
            // 2. Loop semua kemungkinan kombinasi yang benar (ValidCombination)
            foreach (var combo in level.validCombinations)
            {
                bool thisComboCorrect = true;

                // Cek setiap toggle untuk kombinasi INI
                for (int i = 0; i < level.toggleCount; i++)
                {
                    // Pastikan array states cukup panjang untuk mencegah error
                    if (i >= combo.states.Length) break;

                    bool currentToggleState = toggles[i].isOn;
                    bool requiredState = combo.states[i];

                    if (currentToggleState != requiredState)
                    {
                        thisComboCorrect = false;
                        break; // Salah satu toggle salah di kombinasi ini, pindah ke kombinasi berikutnya
                    }
                }

                // Jika kombinasi ini benar semua
                if (thisComboCorrect)
                {
                    isMatched = true;
                    break; // Sudah ketemu yang benar, stop pengecekan
                }
            }
        }

        // 3. Tentukan hasil akhir (Menang / Kalah)
        if (isMatched)
        {
            // Benar
            if (statusImageUI != null) statusImageUI.sprite = level.statusSpriteB;
            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            // Salah
            if (statusImageUI != null) statusImageUI.sprite = level.statusSpriteA;
            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);
        }
    }
    // ------------------------------

    public void OnNextLevelButton()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex >= levels.Length) nextIndex = 0;
        LoadLevel(nextIndex);
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