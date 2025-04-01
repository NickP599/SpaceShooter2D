
using UnityEngine;

/// <summary>
/// Класс, представляющий разрушаемый объект.
/// </summary>
public class Destructible : Entity
{
    #region Fields

    /// <summary>
    /// Определяет, является ли объект разрушаемым.
    /// </summary>
    [SerializeField] private bool m_InDestructible;
    public bool IsInDestructible => m_InDestructible;

    /// <summary>
    /// Количество очков здоровья объекта.
    /// </summary>
    [SerializeField] private int m_HitPoints;

    /// <summary>
    /// Текущее количество очков здоровья объекта.
    /// </summary>
    private int m_CurrentHitPoints;
    public int HitPoints => m_CurrentHitPoints;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Инициализация объекта.
    /// </summary>
    protected virtual void Start()
    {
        m_CurrentHitPoints = m_HitPoints;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Применяет урон к объекту.
    /// </summary>
    /// <param name="damage">Количество урона.</param>
    public void ApplyDamage(int damage)
    {
        if (m_InDestructible) return;

        m_CurrentHitPoints -= damage;

        if (m_CurrentHitPoints <= 0)
        {
            OnDeath();
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Метод, вызываемый при уничтожении объекта.
    /// </summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    #endregion
}
