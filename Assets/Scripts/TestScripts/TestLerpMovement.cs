using UnityEngine;

public class TestLerpMovement : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, target.position, Time.deltaTime);
    }
}
