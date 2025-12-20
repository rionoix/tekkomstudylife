using UnityEngine;
using UnityEngine.EventSystems;

public class PinPoint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int pinID; 
    public bool isConnected = false;
    
    private LineManager lineManager;

    void Start() {
        lineManager = FindObjectOfType<LineManager>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Mulai tarik garis hanya jika belum terhubung
        if (!isConnected && lineManager != null) {
            lineManager.StartDrawing(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        // LOGIKA BARU: Cek benda apa yang ada di bawah mouse saat dilepas
        GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

        if (hitObject != null)
        {
            // Cek apakah yang kena adalah PinPoint lain?
            PinPoint targetPin = hitObject.GetComponent<PinPoint>();
            
            if (targetPin != null)
            {
                // Sambungkan!
                lineManager.FinishDrawing(targetPin);
            }
            else
            {
                // Dilepas bukan di pin (misal di background) -> Batalkan
                lineManager.FinishDrawing(null);
            }
        }
        else
        {
            // Dilepas di tempat kosong -> Batalkan
            lineManager.FinishDrawing(null);
        }
    }
}