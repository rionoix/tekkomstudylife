using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DotPoint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    [HideInInspector] public string pairID;
    [HideInInspector] public ConnectManager manager;
    [HideInInspector] public bool isConnected = false;
    
    public Image myImage; // Drag komponen Image ke sini di inspector prefab

    // Saat mouse ditekan di titik ini
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isConnected)
        {
            manager.StartDrawing(this);
        }
    }

    // Saat mouse memasuki area titik ini (saat sedang drag)
    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.OnEnterDot(this);
    }

    // Saat mouse dilepas
    public void OnPointerUp(PointerEventData eventData)
    {
        manager.StopDrawing();
    }
}