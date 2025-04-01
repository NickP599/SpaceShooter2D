using System;
using UnityEngine;


public class LevelBoundary : SingletonBase<LevelBoundary>
{
    [SerializeField] private float m_Radius;
    [SerializeField] private Vector2 m_ScreenResolution;
    public float Radius => m_Radius;
    public Vector2 ScreenResolution => m_ScreenResolution;

    public enum Mode
    {
        Limit,
        Teleport
    }


    [SerializeField] private Mode m_LimitMode;
    public Mode LimitMode => m_LimitMode;

    public float LeftBorder
    {
        get { return Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x; }
    }

    public float RightBorder
    {
        get { return Camera.main.ScreenToWorldPoint(new Vector3(m_ScreenResolution.x, 0, 0)).x; }
    }

    protected override void Awake()
    {
        base.Awake();

        if (Application.isEditor == false && Application.isPlaying == true)
        {
            m_ScreenResolution.x = Screen.width;
            m_ScreenResolution.y = Screen.height;
        }
    }

   

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, m_Radius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(LeftBorder, 10, 0), new Vector3(LeftBorder, -10, 0));
        Gizmos.DrawLine(new Vector3(RightBorder, 10, 0), new Vector3(RightBorder, -10, 0));
    }

#endif
}
