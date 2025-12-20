using UnityEngine;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    public GameObject linePrefab; 
    public Transform lineParent; 
    
    private PinPoint currentStartPin;
    private GameObject currentLineObj;
    private LineRenderer currentLineRenderer; 
    
    public List<ConnectionPair> currentUserConnections = new List<ConnectionPair>();

    public void StartDrawing(PinPoint startPin)
    {
        currentStartPin = startPin;
        currentLineObj = Instantiate(linePrefab, lineParent);
        
        currentLineRenderer = currentLineObj.GetComponent<LineRenderer>();
        
        if(currentLineRenderer != null) {
            currentLineRenderer.positionCount = 2;
            currentLineRenderer.useWorldSpace = true; 
            
            // Posisi Awal
            Vector3 startPos = currentStartPin.transform.position;
            startPos.z = -1f; // Paksa muncul di depan kertas
            
            currentLineRenderer.SetPosition(0, startPos);
            currentLineRenderer.SetPosition(1, startPos);
        }
    }

    void Update()
    {
        if (currentStartPin != null && currentLineObj != null && currentLineRenderer != null)
        {
            // PERBAIKAN PENTING DI SINI:
            // Kita harus memberi tahu jarak Z dari kamera ke kertas (10f)
            // agar konversi posisi mouse akurat.
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = 10f; // Jarak Plane Distance kamera Anda

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);
            mousePos.z = -1f; // Paksa Z tetap di depan kertas (-1)
            
            currentLineRenderer.SetPosition(1, mousePos);
        }
    }

    public void FinishDrawing(PinPoint endPin)
    {
        // Validasi: Pin tujuan ada, dan bukan pin asal
        if (currentStartPin != null && endPin != null && currentStartPin != endPin)
        {
            ConnectionPair newConnection = new ConnectionPair();
            newConnection.pinID_A = currentStartPin.pinID;
            newConnection.pinID_B = endPin.pinID;
            currentUserConnections.Add(newConnection);
            
            // Kunci posisi akhir
            Vector3 endPos = endPin.transform.position;
            endPos.z = -1f;
            
            currentLineRenderer.SetPosition(1, endPos);
            
            // Opsional: Tandai pin sudah terhubung agar tidak ditarik lagi
            // currentStartPin.isConnected = true;
            // endPin.isConnected = true;
        }
        else
        {
            // Batalkan garis jika tidak valid
            Destroy(currentLineObj); 
        }
        
        currentStartPin = null;
        currentLineObj = null;
        currentLineRenderer = null;
    }

    public void ClearLines()
    {
        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }
        currentUserConnections.Clear();
        currentStartPin = null;
        currentLineObj = null;
        currentLineRenderer = null;
    }
}