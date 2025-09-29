using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
{
    Debug.Log("Tombol Start ditekan!");
    SceneManager.LoadScene("ruang kelas");
}


    public void QuitGame()
    {
        Debug.Log("Keluar game...");
        Application.Quit();
    }
}
