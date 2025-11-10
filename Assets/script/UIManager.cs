using UnityEngine;
using UnityEngine.SceneManagement;  // untuk ganti scene

public class UIManager : MonoBehaviour
{
    // Fungsi ini akan dipanggil saat tombol diklik
    public void KembaliKeMenu()
    {
        // Ganti "menu" dengan nama scene menu kamu
        SceneManager.LoadScene("menu");
    }

    public void kelaspetekkom()
    {
        // Ganti "menu" dengan nama scene menu kamu
        SceneManager.LoadScene("petekkom");
    }

    public void kelasrld()
    {
        // Ganti "menu" dengan nama scene menu kamu
        SceneManager.LoadScene("rld");
    }public void kelassister()
    {
        // Ganti "menu" dengan nama scene menu kamu
        SceneManager.LoadScene("sister");
    }

}
