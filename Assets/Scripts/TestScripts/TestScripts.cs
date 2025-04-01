using UnityEditor.Purchasing;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    public float A_f;
    public float B_f;
    public float R_f;

    public Vector3 A_v;
    public Vector3 B_v;
    public Vector3 R_v;

    public Color A_c;
    public Color B_c;
    public Color R_c;

    [Range(0, 1.0f)]
    public float t;

    

    // Update is called once per frame
    void Update()
    {
        R_f = Mathf.Lerp(A_f, B_f, t);

        R_v = Vector3.Lerp(A_v, B_v, t);

        R_c = Color.Lerp(A_c, B_c, t);
        
    }

}
