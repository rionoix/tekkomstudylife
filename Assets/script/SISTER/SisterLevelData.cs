using UnityEngine;
using System.Collections.Generic;

// Perhatikan nama class berubah menjadi SisterLevelData
[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Sister Level Data")]
public class SisterLevelData : ScriptableObject
{
    [Header("Pengaturan Umum")]
    public bool isTextOnly; 
    [TextArea] public string systemDescription; 

    [Header("Layout Level")]
    public GameObject levelPrefab; 

    [Header("Kunci Jawaban (Logic)")]
    public List<ConnectionPair> correctConnections;
}

[System.Serializable]
public struct ConnectionPair
{
    public int pinID_A;
    public int pinID_B;
}