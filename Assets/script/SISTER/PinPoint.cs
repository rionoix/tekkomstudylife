using UnityEngine;
using UnityEngine.EventSystems;

public class PinPoint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int pinID; 
    
    [Header("Status Koneksi")]
    public bool isConnected = false;
    public PinPoint connectedPin; // Siapa pasangan saya?
    public GameObject lineObject; // Garis mana yang nempel di saya?
    
    private LineManager lineManager;

    void Start() {
        lineManager = FindAnyObjectByType<LineManager>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (lineManager == null) return;

        // --- DETEKSI KLIK KANAN (UNTUK HAPUS) ---
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Hanya hapus jika memang sudah tersambung
            if (isConnected)
            {
                lineManager.DeleteConnection(this);
            }
        }
        // --- DETEKSI KLIK KIRI (UNTUK TARIK GARIS) ---
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Hanya boleh tarik garis jika pin ini BELUM terhubung
            if (!isConnected) {
                lineManager.StartDrawing(this);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        // Logika PointerUp hanya berjalan untuk KLIK KIRI (saat selesai drag)
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
                PinPoint targetPin = hitObject.GetComponent<PinPoint>();
                
                if (targetPin != null)
                {
                    lineManager.FinishDrawing(targetPin);
                }
                else
                {
                    lineManager.FinishDrawing(null);
                }
            }
            else
            {
                lineManager.FinishDrawing(null);
            }
        }
    }
}