using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Класс, представляющий разрушаемый объект.
/// </summary>
public class Destructible : Entity
{
    #region Fields


    [SerializeField] private bool m_InDestructible;
    public bool IsInDestructible => m_InDestructible;

   
    [SerializeField] private int m_HitPoints;

    private int m_CurrentHitPoints;
    public int HitPoints => m_CurrentHitPoints;

    
    [SerializeField] public int m_TeamId;
    public int TeamId => m_TeamId;

   
    [SerializeField] private UnityEvent m_EventOnDeath;
    public UnityEvent EventOnDeath => m_EventOnDeath;

    /// <summary>
    /// Набор всех разрушаемых объектов.
    /// </summary>
    private static HashSet<Destructible> m_AllDestructible;
    public static IReadOnlyCollection<Destructible> AllDestructible => m_AllDestructible;

    #endregion

    #region Constants

    public const int TEAM_ID_NEUTRAL = 0;

    #endregion

    #region Unity Methods

    protected virtual void Start()
    {
        m_CurrentHitPoints = m_HitPoints;
        
    }

    protected virtual void OnEnable()
    {
        if (m_AllDestructible == null)
            m_AllDestructible = new HashSet<Destructible>();

        m_AllDestructible?.Add(this);
    }

    private void OnDestroy()
    {
        m_AllDestructible?.Remove(this);
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

    /// <summary>
    /// Добавляет очки здоровья объекту.
    /// </summary>
    /// <param name="hitPoints">Количество очков здоровья.</param>
    public void AddHitPoints(int hitPoints)
    {
        if (hitPoints <= 0) return;

        m_CurrentHitPoints = Mathf.Clamp(m_CurrentHitPoints + hitPoints, 0, m_HitPoints);
    }

    /// <summary>
    /// Устанавливает неуязвимость объекта.
    /// </summary>
    /// <param name="isActive">Статус неуязвимости.</param>
    /// <returns>Текущий статус неуязвимости.</returns>
    public bool SetInvulnerable(bool isActive)
    {
        return m_InDestructible = isActive;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Метод, вызываемый при уничтожении объекта.
    /// </summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
        m_EventOnDeath?.Invoke();
    }

    #endregion
}
