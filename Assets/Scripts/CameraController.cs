using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        ForwardOnly,
        FreeMovement
    }

    [SerializeField] private CameraMode m_CameraMode;
    public CameraMode CameraMod => m_CameraMode;

    [SerializeField] private Camera m_Camera;
    [SerializeField] private Transform m_Target;

    public void SetTarget(Transform newTarget) => m_Target = newTarget;

    [SerializeField] private float m_InterpolationLiner;
    [SerializeField] private float m_InterpolationAngular;

    [SerializeField] private float m_CameraZOffSet;
    [SerializeField] private float m_ForwardOffSet;

    private void FixedUpdate()
    {
        if (m_CameraMode == CameraMode.FreeMovement)
        {
            if (m_Camera == null || m_Target == null)
            {
                Debug.LogWarning("Camera or Target is not assigned.");
                return;
            }

            Vector2 cameraPos = m_Camera.transform.position;
            Vector2 targetPos = m_Target.position + m_Target.transform.up * m_ForwardOffSet;

            Vector2 newCamPos = Vector3.Lerp(cameraPos, targetPos, m_InterpolationLiner * Time.deltaTime);

            m_Camera.transform.position = new Vector3(newCamPos.x, newCamPos.y, m_CameraZOffSet);


            if (m_InterpolationAngular > 0)
            {
                m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation, m_Target.rotation, m_InterpolationAngular * Time.deltaTime);
            }
        }

        if(m_CameraMode == CameraMode.ForwardOnly)
        {
            if (m_Camera == null || m_Target == null)
            {
                Debug.LogWarning("Camera or Target is not assigned.");
                return;
            }

            Vector2 cameraPos = m_Camera.transform.position;
            Vector2 targetPos = m_Target.position + m_Target.transform.up * m_ForwardOffSet;

            Vector2 newCamPOs = Vector3.Lerp(cameraPos, targetPos,m_InterpolationAngular * Time.deltaTime);

            m_Camera.transform.position = new Vector3(m_Camera.transform.position.x, newCamPOs.y , m_CameraZOffSet);

           
        }
        
    }
}
