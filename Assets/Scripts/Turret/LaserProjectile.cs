using UnityEngine;

public class LaserProjectile : Projectile
{
    [SerializeField] private float m_defDistanceRat = 10f; // Максимальная длина луча

    private LineRenderer m_LineRenderer;
    private Transform m_Target;

    private void Awake()
    {
        m_LineRenderer = GetComponentInChildren<LineRenderer>();
    }


    private void Update()
    {
        // Выполняем Raycast с указанием максимальной дистанции
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, m_defDistanceRat);

        Vector2 endPos;
        if (hit.collider != null && hit.transform != m_Parent.transform)
        {
            // Если луч попал в объект, используем точку попадания
            endPos = hit.point;
            m_Target = hit.transform;
        
            // Наносим урон, если цель разрушима
            if (m_Target.root.TryGetComponent(out Destructible destructible))
            {
                destructible.ApplyDamage(m_Damage);
            }
        }
        else
        {
            // Если ничего не попало, рисуем луч на максимальную дистанцию
            endPos = (Vector2)transform.position + (Vector2)transform.up * m_defDistanceRat;
            m_Target = null;
        }

        // Рисуем лазер от начальной точки до конечной
        Draw2DRay(transform.position, endPos);

        // Отладка: визуализация луча
        Debug.DrawRay(transform.position, transform.up * m_defDistanceRat, Color.red, 0.1f);
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_LineRenderer.SetPosition(0, startPos);
        m_LineRenderer.SetPosition(1, endPos);
    }
}

