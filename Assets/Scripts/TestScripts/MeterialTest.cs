using UnityEngine;

public class MeterialTest : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public Material material;

    public float t;

    private void Start()
    {
        material = MeshRenderer.material;
    }

    private void Update()
    {
        material.mainTextureOffset += new Vector2(t, t);
    }
}
