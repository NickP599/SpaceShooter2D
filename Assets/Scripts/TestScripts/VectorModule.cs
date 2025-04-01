using UnityEngine;

public class VectorModule : MonoBehaviour
{
    private void FixedUpdate()
    {
        Debug.Log($"Модуль вектора:  { transform.position.magnitude}, квадрат модуля {transform.position.sqrMagnitude}," +
            $" направление вектора {transform.position.normalized}" );
 
        Debug.DrawRay(Vector3.zero, transform.position, Color.black);
    }
}
