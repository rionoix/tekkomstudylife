using UnityEngine;
using System.Collections.Generic;

public class TrashSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject trashPrefab;   // prefab sampah
    public int trashCount = 10;      // jumlah spawn
    public float spawnRange = 5f;    // jangkauan acak (X dan Y)

    [Header("Spacing Settings")]
    public float minDistance = 1.5f; // jarak minimum antar sampah

    private List<Vector2> usedPositions = new List<Vector2>();
    private List<GameObject> spawnedTrash = new List<GameObject>();

    void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        int maxAttempts = 30; // batas percobaan cari posisi

        for (int i = 0; i < trashCount; i++)
        {
            Vector2 spawnPos = Vector2.zero;
            bool validPos = false;

            // coba beberapa kali sampai dapat posisi yang valid
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // 📌 Spawn selalu di sekitar (0,0), bukan di sekitar player
                Vector2 randomPos = new Vector2(
                    Random.Range(-spawnRange, spawnRange),
                    Random.Range(-spawnRange, spawnRange)
                );

                if (IsFarEnough(randomPos))
                {
                    spawnPos = randomPos;
                    validPos = true;
                    break;
                }
            }

            if (validPos)
            {
                GameObject newTrash = Instantiate(trashPrefab, spawnPos, Quaternion.identity);
                newTrash.tag = "Trash"; // penting supaya bisa dihapus UIManager
                spawnedTrash.Add(newTrash);
                usedPositions.Add(spawnPos);
            }
            else
            {
                Debug.LogWarning("Gagal menemukan posisi spawn yang cukup jauh, skip 1 sampah.");
            }
        }
    }

    bool IsFarEnough(Vector2 pos)
    {
        foreach (Vector2 usedPos in usedPositions)
        {
            if (Vector2.Distance(pos, usedPos) < minDistance)
                return false; // terlalu dekat
        }
        return true;
    }

    // === Fungsi Reset ===
    public void ResetTrash()
    {
        // Hapus semua sampah lama
        foreach (GameObject trash in spawnedTrash)
        {
            if (trash != null)
                Destroy(trash);
        }

        spawnedTrash.Clear();
        usedPositions.Clear();

        // Spawn ulang dengan posisi acak baru di sekitar (0,0)
        SpawnTrash();
    }
}
