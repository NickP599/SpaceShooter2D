using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;


public class Turret : MonoBehaviour
{
    [SerializeField] private TurretMode m_Mode;
    public TurretMode Mode => m_Mode;

    [SerializeField] private TurretProperties m_TurretProperties;

    private float m_ReFireTime;
    public bool CanFire => m_ReFireTime <= 0;

    [HideInInspector] public float ProjectileVelocity;

    private SpaceShip m_Ship;
    private AudioSource m_AudioSource;
  
    #region Unity Events
    private void Start()
    { 
        if (!transform.root.TryGetComponent(out m_Ship))
        {
            Debug.LogWarning("SpaceShip не найден на корневом объекте!", gameObject);
        }

        if (!TryGetComponent<AudioSource>(out m_AudioSource))
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
        }
       
    }

    private void Update()
    {
        if (m_ReFireTime > 0)
            m_ReFireTime -= Time.deltaTime;

        Debug.Log(ProjectileVelocity);
    }

    #endregion

    #region Public API
    public void Fire()
    {
        if (m_TurretProperties == null && m_ReFireTime > 0) return;

        if(m_Ship.DrawEnergy(m_TurretProperties.EnergyUsage) == false) 
            return;

        if (m_Ship.DrawAmmo(m_TurretProperties.AmmoUsage) == false)
            return;


        if (m_TurretProperties.LaunchSFX != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_TurretProperties.LaunchSFX);
        }

        Projectile projectile = Instantiate(m_TurretProperties.ProjectilePrefab);
        projectile.transform.position = transform.position;
        projectile.transform.up = transform.up;
        
        ProjectileVelocity = projectile.Velocity;

        projectile.SetParentDestructible(m_Ship);
 
        m_ReFireTime = m_TurretProperties.RateOffFire;

    }

    public void AssignLoadout(TurretProperties props)
    {
        if (m_Mode != props.Mode) return;

        m_ReFireTime = 0;

        m_TurretProperties = props;  
    }

    #endregion
}
