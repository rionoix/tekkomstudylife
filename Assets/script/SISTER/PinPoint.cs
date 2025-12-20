using UnityEngine;
using UnityEngine.EventSystems;

public class PinPoint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int pinID; // ID unik, misal: Arduino 5V = 1, Sensor VCC = 2
    public bool isConnected = false;
    
    // Referensi ke Manager untuk memberi tahu sedang ditarik garis
    private LineManager lineManager;

    void Start() {
        lineManager = FindObjectOfType<LineManager>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!isConnected) lineManager.StartDrawing(this);
    }

    public void OnPointerUp(PointerEventData eventData) {
        // Logika saat mouse dilepas di pin ini ditangani oleh LineManager
    }
}