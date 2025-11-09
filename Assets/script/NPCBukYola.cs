using UnityEngine;

public class NPCBukYola : MonoBehaviour
{
    public GameObject player;
    public DialogManager dialogManager;

    private bool hasTalked = false; // supaya dialog tidak muncul terus-terusan

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !hasTalked)
        {
            hasTalked = true;
            dialogManager.ShowDialog("Duduklah nak, kelas akan dimulai", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            dialogManager.HideDialog();
            hasTalked = false; // biar bisa muncul lagi kalau mendekat lagi
        }
    }
}
