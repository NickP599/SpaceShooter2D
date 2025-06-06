using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class CircleArea : MonoBehaviour
{
    [SerializeField] private float m_Radius;
    public float Radius => m_Radius;

    public Vector2 GetRandomInsideZone
    {
        get
        {
            return  (Vector2) transform.position + (Vector2) Random.insideUnitSphere * m_Radius;
        }
    }

#if UNITY_EDITOR

    private static Color GizmosColor = new Color(0, 1, 0, 0.05f);


    private void OnDrawGizmosSelected()
    {
        Handles.color = GizmosColor;

        Handles.DrawSolidDisc(transform.position,transform.forward,m_Radius);
    }

#endif
}
