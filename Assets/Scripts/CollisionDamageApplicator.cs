using UnityEngine;

public class CollisionDamageApplicator : MonoBehaviour
{
    public string IgnoreTag = "WorldBoundary";

    [SerializeField] private float m_VelocityDamageModifier;
    [SerializeField] private float m_DamageConst;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag(IgnoreTag) || !TryGetComponent<Destructible>(out Destructible destructible)) return;

        if (destructible != null)
        {
            destructible.ApplyDamage((int)m_DamageConst + (int)(m_VelocityDamageModifier * collision.relativeVelocity.magnitude));
        }
    }
}
