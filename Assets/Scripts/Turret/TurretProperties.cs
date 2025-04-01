using UnityEngine;


public enum TurretMode
{
    Primary,
    Secondary,
}

[CreateAssetMenu(fileName = "TurretProperties", menuName = "Turret/TurretProperties")]
public class TurretProperties : ScriptableObject
{

    [SerializeField] TurretMode m_Mode;
    public TurretMode Mode => m_Mode;

    [SerializeField] private Projectile m_ProjectilePrefab;
    public Projectile ProjectilePrefab => m_ProjectilePrefab;

    [SerializeField] private float m_RateOffFire;
    public float RateOffFire => m_RateOffFire;

    [SerializeField] private int m_EnergyUsage;
    public int EnergyUsage => m_EnergyUsage;

    [SerializeField] private int m_AmmoUsage;
    public int AmmoUsage => m_AmmoUsage;

    [SerializeField] private AudioClip m_LaunchSFX;
    public AudioClip LaunchSFX => m_LaunchSFX;


}
