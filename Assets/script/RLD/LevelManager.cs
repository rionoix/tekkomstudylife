using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RectTransformData
{
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta = new Vector2(100, 100);
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

    [Header("Gambar utama (berubah tiap level)")]
    public Sprite levelImage;
    public RectTransformData levelImageLayout;

    [Header("Sprite status (A -> B kalau kombinasi benar)")]
    public Sprite statusSpriteA;
    public Sprite statusSpriteB;
    public RectTransformData statusImageLayout;

    [Header("Pengaturan toggle (untuk TogglePuzzle)")]
    [Range(0, 6)]
    public int toggleCount = 0;                 // 0–6 toggle
    public bool[] targetCombination = new bool[6];           // hanya 0..toggleCount-1 yang dipakai
    public RectTransformData[] toggleLayouts = new RectTransformData[6]; // layout masing2 toggle

    [Header("Level selingan (TextOnly)")]
    [TextArea(2, 4)]
    public string intermissionText;
}

public class LevelManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public Image levelImageUI;
    public Image statusImageUI;
    public Toggle[] toggles;                    // isi 6 toggle di Inspector
    public Button nextLevelButton;
    public TextMeshProUGUI intermissionTextUI;

    [Header("Data Level")]
    public LevelData[] levels;

    private int currentLevelIndex = 0;

    private void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Length)
        {
            Debug.LogError("Level index di luar range!");
            return;
        }

        currentLevelIndex = index;
        LevelData level = levels[currentLevelIndex];

        // Reset tombol Next
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);

        // Pilih setup berdasarkan tipe level
        if (level.levelType == LevelType.TextOnly)
        {
            SetupTextOnlyLevel(level);
        }
        else // TogglePuzzle
        {
            SetupTogglePuzzleLevel(level);
        }
    }

    // === TEXT ONLY LEVEL ===
    private void SetupTextOnlyLevel(LevelData level)
    {
        // Matikan gambar & toggle
        if (levelImageUI != null) levelImageUI.gameObject.SetActive(false);
        if (statusImageUI != null) statusImageUI.gameObject.SetActive(false);

        if (toggles != null)
        {
            foreach (var t in toggles)
            {
                if (t != null)
                    t.gameObject.SetActive(false);
            }
        }

        // Tampilkan teks selingan
        if (intermissionTextUI != null)
        {
            intermissionTextUI.gameObject.SetActive(true);
            intermissionTextUI.text = level.intermissionText;
        }

        // Tombol Next langsung ada
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
    }

    // === TOGGLE PUZZLE LEVEL ===
    private void SetupTogglePuzzleLevel(LevelData level)
    {
        // Aktifkan & atur gambar utama
        if (levelImageUI != null)
        {
            levelImageUI.gameObject.SetActive(true);
            levelImageUI.sprite = level.levelImage;
            ApplyRect(levelImageUI.rectTransform, level.levelImageLayout);
        }

        // Aktifkan & atur status image (pakai sprite A dulu)
        if (statusImageUI != null)
        {
            statusImageUI.gameObject.SetActive(true);
            statusImageUI.sprite = level.statusSpriteA;
            ApplyRect(statusImageUI.rectTransform, level.statusImageLayout);
        }

        // Matikan teks selingan
        if (intermissionTextUI != null)
            intermissionTextUI.gameObject.SetActive(false);

        // Atur toggle
        for (int i = 0; i < toggles.Length; i++)
        {
            bool aktif = i < level.toggleCount;

            if (toggles[i] == null) continue;

            toggles[i].gameObject.SetActive(aktif);

            if (aktif)
            {
                toggles[i].isOn = false; // reset
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

    // Dipanggil dari semua toggle (On Value Changed)
    public void OnToggleChanged()
    {
        LevelData level = levels[currentLevelIndex];
        if (level.levelType != LevelType.TogglePuzzle)
            return; // kalau lagi di level text, abaikan

        CheckCombination();
    }

    private void CheckCombination()
    {
        LevelData level = levels[currentLevelIndex];

        for (int i = 0; i < level.toggleCount; i++)
        {
            bool current = toggles[i].isOn;
            bool target = level.targetCombination[i];

            if (current != target)
            {
                // kombinasi salah
                if (statusImageUI != null)
                    statusImageUI.sprite = level.statusSpriteA;

                if (nextLevelButton != null)
                    nextLevelButton.gameObject.SetActive(false);

                return;
            }
        }

        // Kalau sampai sini, kombinasi benar
        if (statusImageUI != null)
            statusImageUI.sprite = level.statusSpriteB;

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
    }

    // Di-assign ke tombol Next Level
    public void OnNextLevelButton()
    {
        int nextIndex = currentLevelIndex + 1;

        if (nextIndex >= levels.Length)
        {
            // misal balik ke level pertama; bisa kamu ganti sesuai kebutuhan
            nextIndex = 0;
        }

        LoadLevel(nextIndex);
    }
}
