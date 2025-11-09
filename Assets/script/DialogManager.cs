using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Button btnMulaiKelas;

    private void Start()
    {
        dialogPanel.SetActive(false);
        btnMulaiKelas.gameObject.SetActive(false);
    }

    public void ShowDialog(string message, bool showButton = false)
    {
        dialogPanel.SetActive(true);
        dialogText.text = message;

        btnMulaiKelas.gameObject.SetActive(showButton);
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        btnMulaiKelas.gameObject.SetActive(false);
    }
}
