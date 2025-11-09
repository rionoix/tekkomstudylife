using UnityEngine;
using UnityEngine.SceneManagement;  // untuk ganti scene
using UnityEngine.UI;               // untuk tombol UI

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("gedung H");
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();

        // Tambahan supaya saat di editor Unity tetap terlihat bekerja
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
