using UnityEngine;

// --- INI BAGIAN YANG TADI HILANG ---
[System.Serializable]
public class ElementLayout
{
    public Vector2 position;
    public Vector2 size = new Vector2(100, 100);
}

[System.Serializable]
public class PairData
{
    public string pairID; // ID Pasangan (misal "1", "A")
    public ElementLayout pointA; // Posisi Titik Pertama
    public ElementLayout pointB; // Posisi Titik Kedua
}
// ------------------------------------

public enum ConnectLevelType
{
    Puzzle,
    TextOnly
}

[System.Serializable]
public class ConnectLevelData
{
    public string levelName;
    public ConnectLevelType levelType = ConnectLevelType.Puzzle; // Tipe Level

    [Header("Mode Puzzle: Konfigurasi")]
    public PairData[] pairs; 
    public Sprite[] decorationSprites;
    public ElementLayout[] decorationLayouts;

    [Header("Mode Text: Pesan")]
    [TextArea(3, 5)]
    public string intermissionText; // Teks untuk level bacaan
}