using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

#if UNITY_EDITOR
using UnityEditor; // Wajib ada untuk fix error OnValidate
#endif

public class ConnectManager : MonoBehaviour
{
    [Header("--- EDITOR PREVIEW ---")]
    public bool livePreview = true;
    [Range(0, 50)] 
    public int previewIndex = 0; 

    [Header("Referensi Prefab (WAJIB DIISI)")]
    public GameObject dotPrefab;
    public GameObject linePrefab;

    [Header("Container UI (WAJIB DIISI)")]
    public Transform gameplayArea;   
    public Transform decorationArea; 
    
    [Header("UI Controls")]
    public Button nextLevelButton;
    public TextMeshProUGUI infoTextUI; 

    [Header("Settings")]
    public Color[] lineColors;

    [Header("Data Level")]
    public ConnectLevelData[] levels;

    // State Internal
    private int currentLevelIndex = 0;
    private DotPoint startDot;
    private GameObject currentLineObj;
    private RectTransform currentLineRect;
    private List<GameObject> activeObjects = new List<GameObject>();
    private int completedPairs = 0;
    private int totalPairs = 0;

    private void Start()
    {
        ClearPreviewObjects(); // Bersihkan preview saat play dimulai
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
        LoadLevel(0);
    }

    public void LoadLevel(int index)
    {
        ClearRuntimeObjects(); 
        if (index >= levels.Length) index = 0;
        currentLevelIndex = index;
        ConnectLevelData data = levels[currentLevelIndex];

        if (data.levelType == ConnectLevelType.TextOnly) SetupTextLevel(data);
        else SetupPuzzleLevel(data);
    }

    void SetupTextLevel(ConnectLevelData data)
    {
        if(gameplayArea) gameplayArea.gameObject.SetActive(false);
        if(decorationArea) decorationArea.gameObject.SetActive(false);
        if(infoTextUI) { infoTextUI.gameObject.SetActive(true); infoTextUI.text = data.intermissionText; }
        if(nextLevelButton) nextLevelButton.gameObject.SetActive(true);
    }

    void SetupPuzzleLevel(ConnectLevelData data)
    {
        if(gameplayArea) gameplayArea.gameObject.SetActive(true);
        if(decorationArea) decorationArea.gameObject.SetActive(true);
        if(infoTextUI) infoTextUI.gameObject.SetActive(false);
        if(nextLevelButton) nextLevelButton.gameObject.SetActive(false);

        completedPairs = 0;
        SpawnDecorations(data);

        if (data.pairs != null)
        {
            totalPairs = data.pairs.Length;
            for (int i = 0; i < data.pairs.Length; i++)
            {
                PairData pair = data.pairs[i];
                Color c = (lineColors != null && lineColors.Length > 0) ? lineColors[i % lineColors.Length] : Color.white;
                CreateDot(pair.pointA, pair.pairID, c);
                CreateDot(pair.pointB, pair.pairID, c);
            }
        }
    }

    private void SpawnDecorations(ConnectLevelData data)
    {
        if (data.decorationSprites == null || decorationArea == null) return;
        for (int i = 0; i < data.decorationSprites.Length; i++)
        {
            if (i >= data.decorationLayouts.Length) break;
            GameObject imgObj = new GameObject("Decoration_" + i);
            imgObj.transform.SetParent(decorationArea, false);
            Image img = imgObj.AddComponent<Image>();
            img.sprite = data.decorationSprites[i];
            RectTransform rt = imgObj.GetComponent<RectTransform>();
            rt.anchoredPosition = data.decorationLayouts[i].position;
            rt.sizeDelta = (data.decorationLayouts[i].size == Vector2.zero) ? new Vector2(100, 100) : data.decorationLayouts[i].size;
            activeObjects.Add(imgObj);
        }
    }

    private void CreateDot(ElementLayout layout, string id, Color color)
    {
        if (dotPrefab == null || gameplayArea == null) return;
        GameObject dotObj = Instantiate(dotPrefab, gameplayArea);
        RectTransform rt = dotObj.GetComponent<RectTransform>();
        
        rt.anchoredPosition = layout.position;
        // PENTING: Jika size 0, paksa jadi 100 agar terlihat
        rt.sizeDelta = (layout.size == Vector2.zero) ? new Vector2(100, 100) : layout.size;

        DotPoint dotScript = dotObj.GetComponent<DotPoint>();
        if (dotScript != null) {
            dotScript.pairID = id;
            dotScript.manager = this;
            if (dotScript.myImage) dotScript.myImage.color = color;
        }
        activeObjects.Add(dotObj);
    }

    private void ClearRuntimeObjects()
    {
        foreach (var obj in activeObjects) if (obj != null) Destroy(obj);
        activeObjects.Clear();
    }

    // --- LOGIKA GAMEPLAY ---
    public void StartDrawing(DotPoint dot)
    {
        if (linePrefab == null) return;
        startDot = dot;
        currentLineObj = Instantiate(linePrefab, gameplayArea);
        currentLineObj.transform.SetAsFirstSibling();
        currentLineRect = currentLineObj.GetComponent<RectTransform>();
        Image lineImg = currentLineObj.GetComponent<Image>();
        if (lineImg != null) lineImg.color = dot.myImage.color;
        currentLineRect.anchoredPosition = dot.GetComponent<RectTransform>().anchoredPosition;
        currentLineRect.sizeDelta = new Vector2(0, 10);
        activeObjects.Add(currentLineObj);
    }

    private void Update()
    {
        if (startDot != null && currentLineRect != null)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameplayArea as RectTransform, Input.mousePosition, null, out localMousePos);
            Vector2 startPos = startDot.GetComponent<RectTransform>().anchoredPosition;
            Vector2 direction = localMousePos - startPos;
            currentLineRect.sizeDelta = new Vector2(direction.magnitude, 10);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentLineRect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void OnEnterDot(DotPoint targetDot)
    {
        if (startDot == null || targetDot == startDot) return;
        if (targetDot.pairID == startDot.pairID)
        {
            startDot.isConnected = true;
            targetDot.isConnected = true;
            Vector2 startPos = startDot.GetComponent<RectTransform>().anchoredPosition;
            Vector2 endPos = targetDot.GetComponent<RectTransform>().anchoredPosition;
            Vector2 direction = endPos - startPos;
            currentLineRect.sizeDelta = new Vector2(direction.magnitude, 10);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentLineRect.rotation = Quaternion.Euler(0, 0, angle);
            startDot = null;
            currentLineRect = null;
            currentLineObj = null;
            CheckWinCondition();
        }
    }

    public void StopDrawing()
    {
        if (startDot != null && currentLineObj != null)
        {
            activeObjects.Remove(currentLineObj);
            Destroy(currentLineObj);
            startDot = null;
            currentLineObj = null;
        }
    }

    private void CheckWinCondition()
    {
        completedPairs++;
        if (completedPairs >= totalPairs) if(nextLevelButton) nextLevelButton.gameObject.SetActive(true);
    }

    public void NextLevel() { LoadLevel(currentLevelIndex + 1); }


    // =========================================================
    //               EDITOR PREVIEW LOGIC (FIXED & SAFE)
    // =========================================================
    
    private void OnValidate()
    {
        // KITA TIDAK BOLEH MEMANGGIL UPDATE LANGSUNG DI SINI
        // Kita gunakan EditorApplication.delayCall agar Unity menjalankannya 
        // SETELAH proses inspektor selesai. Ini menghilangkan error DestroyImmediate.
        
        if (!Application.isPlaying && livePreview)
        {
            #if UNITY_EDITOR
            // Batalkan panggilan sebelumnya agar tidak menumpuk (fix spam saat drag slider)
            EditorApplication.delayCall -= UpdateEditorPreview;
            // Jadwalkan update baru
            EditorApplication.delayCall += UpdateEditorPreview;
            #endif
        }
    }

    private void UpdateEditorPreview()
    {
        // Cek jika komponen sudah didestroy atau slot kosong, jangan lanjut
        if (this == null || gameplayArea == null || decorationArea == null || dotPrefab == null) return;

        ClearPreviewObjects(); 

        if (levels == null || levels.Length == 0) return;
        previewIndex = Mathf.Clamp(previewIndex, 0, levels.Length - 1);
        ConnectLevelData data = levels[previewIndex];

        if (data.levelType == ConnectLevelType.TextOnly)
        {
            if(infoTextUI) { infoTextUI.gameObject.SetActive(true); infoTextUI.text = data.intermissionText; }
            if(gameplayArea) gameplayArea.gameObject.SetActive(false);
            if(decorationArea) decorationArea.gameObject.SetActive(false);
        }
        else
        {
            if(infoTextUI) infoTextUI.gameObject.SetActive(false);
            if(gameplayArea) gameplayArea.gameObject.SetActive(true);
            if(decorationArea) decorationArea.gameObject.SetActive(true);

            // Spawn Dekorasi
            if (data.decorationSprites != null)
            {
                for (int i = 0; i < data.decorationSprites.Length; i++)
                {
                    if (i >= data.decorationLayouts.Length) break;
                    GameObject imgObj = new GameObject("Prev_Deco");
                    imgObj.transform.SetParent(decorationArea, false);
                    Image img = imgObj.AddComponent<Image>();
                    img.sprite = data.decorationSprites[i];
                    RectTransform rt = imgObj.GetComponent<RectTransform>();
                    rt.anchoredPosition = data.decorationLayouts[i].position;
                    rt.sizeDelta = (data.decorationLayouts[i].size == Vector2.zero) ? new Vector2(100, 100) : data.decorationLayouts[i].size;
                }
            }
            // Spawn Titik
            if (data.pairs != null)
            {
                for (int i = 0; i < data.pairs.Length; i++)
                {
                    PairData pair = data.pairs[i];
                    Color c = (lineColors != null && lineColors.Length > 0) ? lineColors[i % lineColors.Length] : Color.white;
                    CreatePreviewDot(pair.pointA, c);
                    CreatePreviewDot(pair.pointB, c);
                }
            }
        }
    }

    private void CreatePreviewDot(ElementLayout layout, Color c)
    {
        if (dotPrefab == null) return;
        GameObject dot = null;
        
        #if UNITY_EDITOR
        // Instantiate Prefab aman di dalam delayCall
        dot = (GameObject)PrefabUtility.InstantiatePrefab(dotPrefab, gameplayArea);
        #endif

        if (dot != null)
        {
            RectTransform rt = dot.GetComponent<RectTransform>();
            rt.anchoredPosition = layout.position;
            // FIX SIZE: Otomatis set 100 jika user lupa mengisi size
            rt.sizeDelta = (layout.size == Vector2.zero) ? new Vector2(100, 100) : layout.size;
            
            Image img = dot.GetComponent<Image>(); 
            if(img == null) img = dot.GetComponentInChildren<Image>();
            if(img != null) img.color = c;
        }
    }

    private void ClearPreviewObjects()
    {
        // Loop terbalik + while memastikan bersih total
        if (gameplayArea != null)
        {
            while (gameplayArea.childCount > 0)
            {
                GameObject child = gameplayArea.GetChild(0).gameObject;
                if (Application.isPlaying) Destroy(child);
                else DestroyImmediate(child); // Sekarang aman dipanggil karena ada delayCall
            }
        }

        if (decorationArea != null)
        {
            while (decorationArea.childCount > 0)
            {
                GameObject child = decorationArea.GetChild(0).gameObject;
                if (Application.isPlaying) Destroy(child);
                else DestroyImmediate(child);
            }
        }
    }
}