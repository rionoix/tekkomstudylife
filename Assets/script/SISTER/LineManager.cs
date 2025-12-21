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

    // --- LOGIKA MENGGAMBAR (START) ---
    public void StartDrawing(PinPoint startPin)
    {
        currentStartPin = startPin;
        currentLineObj = Instantiate(linePrefab, lineParent);
        
        currentLineRenderer = currentLineObj.GetComponent<LineRenderer>();
        
        if(currentLineRenderer != null) {
            currentLineRenderer.positionCount = 2;
            currentLineRenderer.useWorldSpace = true; 
            
            Vector3 startPos = currentStartPin.transform.position;
            startPos.z = -1f; 
            
            currentLineRenderer.SetPosition(0, startPos);
            currentLineRenderer.SetPosition(1, startPos);
        }
    }

    void Update()
    {
        // Fitur: Klik Kanan saat sedang MENARIK garis (Cancel proses tarik)
        if (Input.GetMouseButtonDown(1)) 
        {
            if (currentLineObj != null) 
            {
                Destroy(currentLineObj);
                currentStartPin = null;
                currentLineObj = null;
                currentLineRenderer = null;
            }
        }

        // Update posisi garis mengikuti mouse
        if (currentStartPin != null && currentLineObj != null && currentLineRenderer != null)
        {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = 10f; 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);
            mousePos.z = -1f; 
            currentLineRenderer.SetPosition(1, mousePos);
        }
    }

    // --- LOGIKA SELESAI GAMBAR (FINISH) ---
    public void FinishDrawing(PinPoint endPin)
    {
        if (currentStartPin != null && endPin != null && currentStartPin != endPin && !endPin.isConnected)
        {
            // 1. Simpan data koneksi ke List Global
            ConnectionPair newConnection = new ConnectionPair();
            newConnection.pinID_A = currentStartPin.pinID;
            newConnection.pinID_B = endPin.pinID;
            currentUserConnections.Add(newConnection);
            
            // 2. Kunci posisi visual garis
            Vector3 endPos = endPin.transform.position;
            endPos.z = -1f;
            currentLineRenderer.SetPosition(1, endPos);
            
            // 3. SET DATA KE PIN (PENTING AGAR BISA DIHAPUS NANTI)
            // Pin A menyimpan referensi Pin B dan Garisnya
            currentStartPin.isConnected = true;
            currentStartPin.connectedPin = endPin;       // Simpan pasangan
            currentStartPin.lineObject = currentLineObj; // Simpan objek garis

            // Pin B menyimpan referensi Pin A dan Garisnya (Garisnya sama/shared)
            endPin.isConnected = true;
            endPin.connectedPin = currentStartPin;       // Simpan pasangan
            endPin.lineObject = currentLineObj;          // Simpan objek garis
        }
        else
        {
            Destroy(currentLineObj); 
        }
        
        // Reset Temporary Variables
        currentStartPin = null;
        currentLineObj = null;
        currentLineRenderer = null;
    }

    // --- FITUR BARU: HAPUS KONEKSI (DIPANGGIL DARI PINPOINT) ---
    public void DeleteConnection(PinPoint pinRequestingDelete)
    {
        // Ambil referensi garis dan pasangan dari pin yang diklik
        GameObject lineToRemove = pinRequestingDelete.lineObject;
        PinPoint partnerPin = pinRequestingDelete.connectedPin;

        // 1. Hapus Objek Garis Visual
        if (lineToRemove != null) Destroy(lineToRemove);

        // 2. Hapus Data dari List currentUserConnections
        if (partnerPin != null)
        {
            currentUserConnections.RemoveAll(x => 
                (x.pinID_A == pinRequestingDelete.pinID && x.pinID_B == partnerPin.pinID) ||
                (x.pinID_A == partnerPin.pinID && x.pinID_B == pinRequestingDelete.pinID)
            );

            // 3. Reset Status Pin Pasangannya
            partnerPin.isConnected = false;
            partnerPin.connectedPin = null;
            partnerPin.lineObject = null;
        }

        // 4. Reset Status Pin yang diklik
        pinRequestingDelete.isConnected = false;
        pinRequestingDelete.connectedPin = null;
        pinRequestingDelete.lineObject = null;
        
        Debug.Log("Koneksi dihapus via Klik Kanan");
    }

    public void ClearLines()
    {
        foreach (Transform child in lineParent) Destroy(child.gameObject);
        currentUserConnections.Clear();
        PinPoint[] allPins = FindObjectsByType<PinPoint>(FindObjectsSortMode.None);
        foreach(var pin in allPins) {
            pin.isConnected = false;
            pin.connectedPin = null;
            pin.lineObject = null;
        }
    }
}