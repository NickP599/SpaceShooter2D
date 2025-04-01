using UnityEngine;

public class PowerUpStats : PowerUp
{
    public enum EffectTypeMode
    {
        Energy,
        Ammo,
        Health,
        Invulnerable,
        Accelerator
    }

    [SerializeField] private EffectTypeMode m_EffectType;
    public EffectTypeMode EffectType => m_EffectType;

    [SerializeField] private float m_Value;

    [Header("Timer for Invulnerable only")]
    [SerializeField] private float m_Time;

   

    protected override void OnPickedUp(SpaceShip spaceShip)
    {
        if(m_EffectType == EffectTypeMode.Energy)
        {
            spaceShip.AddEnergy(m_Value);
        }

        if (m_EffectType == EffectTypeMode.Ammo)
        {
            spaceShip.AddAmmo((int)m_Value);
        }

        if(m_EffectType == EffectTypeMode.Health)
        {
            spaceShip.AddHitPoints((int)m_Value);
        }

        if (m_EffectType == EffectTypeMode.Invulnerable)
        {
            spaceShip.Timer = m_Time;
            spaceShip.SetMechanicState(MechanicState.Invulnerable);
            spaceShip.SetInvulnerable(true);    
        }

        if(m_EffectType  == EffectTypeMode.Accelerator)
        {
            spaceShip.Timer = m_Time;
            spaceShip.AccelerateValue = m_Value;

            spaceShip.SetMechanicState(MechanicState.SpeedBoost);     
            spaceShip.ActivateSpeedBoost();
        }
    }
}
