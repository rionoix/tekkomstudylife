using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // UPDATE: Kita ganti jadi Vector3 supaya nilai Z ikut tersimpan
    private static Vector3 targetPosisi;
    private static bool harusPindahPosisi = false;

    // Nilai Z agar player terlihat (tidak tertimpa background)
    private float nilaiZPlayer = -0.0101f;

    // --- FUNGSI UNTUK TOMBOL DI DALAM KELAS ---

    public void KeluarDariSister()
    {
        // Koordinat Kelas Sister + Z
        targetPosisi = new Vector3(9.38f, -36.15f, nilaiZPlayer); 
        harusPindahPosisi = true;
        SceneManager.LoadScene("gedung H");
    }

    public void KeluarDariRLD()
    {
        // Koordinat Kelas RLD + Z
        targetPosisi = new Vector3(28.49f, -11.58f, nilaiZPlayer);
        harusPindahPosisi = true;
        SceneManager.LoadScene("gedung H");
    }

    public void KeluarDariPetekkom()
    {
        // Koordinat Kelas Petekkom + Z
        targetPosisi = new Vector3(50.56f, -11.58f, nilaiZPlayer);
        harusPindahPosisi = true;
        SceneManager.LoadScene("gedung H");
    }

    // --- LOGIKA SAAT SCENE GEDUNG H DIMUAT ---

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // Cek apakah di Gedung H dan harus pindah
        if (currentScene.name == "gedung H" && harusPindahPosisi)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                // Script ini sekarang akan memaksa player ke Z = -0.0101
                player.transform.position = targetPosisi;
                
                harusPindahPosisi = false; 
            }
            else
            {
                Debug.LogWarning("Player tidak ditemukan! Pastikan objek player memiliki Tag 'Player'.");
            }
        }
    }

    // --- KODINGAN LAINNYA ---

    public void PlayGame()
    {
        SceneManager.LoadScene("gedung H");
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}