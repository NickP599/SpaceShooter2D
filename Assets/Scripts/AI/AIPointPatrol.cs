using UnityEngine;

public class AIPointPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] m_TargetPoints;
    public Transform[] TargetPoints => m_TargetPoints;

    [SerializeField] private float m_Radius;
    public float Radius => m_Radius;

    private int currentPointIndex = 0;
    public int CurrentPointIndex
    {
        get { return currentPointIndex; }
        set
        {
            value = Mathf.Clamp(value, 0, m_TargetPoints.Length - 1);

            currentPointIndex = value;
        }
    }

    private static readonly Color GizmosColor = new Color(1, 0, 0, 0.3f);
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
