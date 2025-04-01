using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

public class PointerClickHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool m_Hold;
    public bool IsHold => m_Hold;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_Hold = true;
        Debug.Log("Fire");
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_Hold= false;

    }
    
}
