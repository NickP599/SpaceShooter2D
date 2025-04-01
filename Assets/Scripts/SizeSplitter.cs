using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// При уничтожении объекта спавнит его с меньшим размером 
/// </summary>
[RequireComponent(typeof(Destructible))]
public class SizeSplitter : MonoBehaviour
{
    public enum Size
    {
        small,
        normal,
        large,
        huge
    }

    [SerializeField] private Size m_size;

    private Destructible m_destructible;

    private void Awake()
    {
        m_destructible = GetComponent<Destructible>();
        SetSize((Size)Random.Range(0, 4));

        m_destructible.EventOnDeath.AddListener(OnDestroyed);
    }

    private void OnDestroy()
    {
        m_destructible.EventOnDeath.RemoveListener(OnDestroyed);
    }

    private void OnDestroyed()
    {
        if (m_size != Size.small)
        {
            Spawn();
        }
        Destroy(gameObject);
    }

    public void SetSize(Size size)
    {
        if (size < 0) return;
        transform.localScale = GetSizeFromVector(size);
        m_size = size;
    }

    private void Spawn()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;

            GameObject newGameOb = Instantiate(this, transform.position + (Vector3)offset, Quaternion.identity).gameObject;
            newGameOb.GetComponentInChildren<Collider2D>().enabled = true;
            newGameOb.name = m_size.ToString();

            if (newGameOb.TryGetComponent<Destructible>(out Destructible destructible))
            {
                destructible.enabled = true;
                destructible.AddHitPoints(destructible.HitPoints / 2);
            }
            if (newGameOb.TryGetComponent<SizeSplitter>(out SizeSplitter sizeSplitter))
            {
                sizeSplitter.enabled = true;
                sizeSplitter.SetSize(m_size - 1);
            }
            
            if (newGameOb.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
              
                Vector2 randomDirection = Random.insideUnitCircle.normalized;

                float force = Random.Range(3f, 5f); 

                rb.AddForce(randomDirection * force, ForceMode2D.Impulse);
            }
        }
    }

    private Vector3 GetSizeFromVector(Size size)
    {
        if (size == Size.huge) return new Vector3(1, 1, 1);
        if (size == Size.large) return new Vector3(0.75f, 0.75f, 0.75f);
        if (size == Size.normal) return new Vector3(0.6f, 0.6f, 0.6f);
        if (size == Size.small) return new Vector3(0.4f, 0.4f, 0.4f);

        return Vector3.zero;
    }
}
