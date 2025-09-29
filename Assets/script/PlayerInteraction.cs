using UnityEngine;
using UnityEngine.InputSystem; // Input System baru

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 2f;       // jarak interaksi
    public LayerMask interactableLayer;       // filter layer Interactable

    // ==== DIPANGGIL OLEH PlayerInput (PC/Web/Console) ====
#if UNITY_STANDALONE || UNITY_WEBGL
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            TryInteract();
        }
    }
#endif

    // ==== DIPAKAI UNTUK ANDROID/IOS ====
#if UNITY_ANDROID || UNITY_IOS
    // Fungsi ini bisa dipanggil dari tombol UI (Button OnClick di Canvas)
    public void OnInteractButton()
    {
        TryInteract();
    }
#endif

    // ==== LOGIKA INTERAKSI ====
    public void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
                Debug.Log("Interaksi berhasil dengan: " + hit.name);
            }
        }
        else
        {
            Debug.Log("Tidak ada objek interaksi di dekat player");
        }
    }

    // Untuk debug, supaya kelihatan lingkaran interaksi
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
