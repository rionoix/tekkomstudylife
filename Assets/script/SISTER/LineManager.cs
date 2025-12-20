using UnityEngine;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    public GameObject linePrefab; 
    public Transform lineParent; 
    
    private PinPoint currentStartPin;
    private GameObject currentLineObj;
    private LineRenderer currentLineRenderer; // Tambahan untuk akses posisi garis
    
    public List<ConnectionPair> currentUserConnections = new List<ConnectionPair>();

    public void StartDrawing(PinPoint startPin)
    {
        currentStartPin = startPin;
        currentLineObj = Instantiate(linePrefab, lineParent);
        
        // Ambil komponen LineRenderer dari prefab yang baru dibuat
        currentLineRenderer = currentLineObj.GetComponent<LineRenderer>();
        
        // Set titik awal
        if(currentLineRenderer != null) {
            currentLineRenderer.positionCount = 2;
            currentLineRenderer.SetPosition(0, currentStartPin.transform.position);
            currentLineRenderer.SetPosition(1, currentStartPin.transform.position); // Ujungnya masih sama
        }
    }

    void Update()
    {
        if (currentStartPin != null && currentLineObj != null && currentLineRenderer != null)
        {
            // Ubah posisi mouse dari Screen ke World
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Pastikan Z tetap 0 untuk 2D
            
            // Update ujung garis mengikuti mouse
            currentLineRenderer.SetPosition(1, mousePos);
        }
    }

    public void FinishDrawing(PinPoint endPin)
    {
        if (currentStartPin != null && endPin != null && currentStartPin != endPin)
        {
            ConnectionPair newConnection = new ConnectionPair();
            newConnection.pinID_A = currentStartPin.pinID;
            newConnection.pinID_B = endPin.pinID;
            currentUserConnections.Add(newConnection);
            
            // Kunci posisi akhir ke pin tujuan
            currentLineRenderer.SetPosition(1, endPin.transform.position);
        }
        else
        {
            Destroy(currentLineObj); 
        }
        
        currentStartPin = null;
        currentLineObj = null;
        currentLineRenderer = null;
    }

    public void ClearLines()
    {
        // Hapus semua garis visual
        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }
        // Kosongkan data
        currentUserConnections.Clear();
        currentStartPin = null;
        currentLineObj = null;
        currentLineRenderer = null;
    }
}