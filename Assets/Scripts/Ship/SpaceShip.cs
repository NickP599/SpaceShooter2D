using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public enum MechanicState
{
    None,
    Invulnerable,
    SpeedBoost
}


/// <summary>
/// Класс SpaceShip представляет космический корабль с физическими параметрами и управлением.
/// </summary>
/// 
[RequireComponent(typeof(Rigidbody2D))]
public class SpaceShip : Destructible
{
    #region Fields

    [Header("SpaceShip")]
    [SerializeField] private float m_Mass;

    [SerializeField] private float m_Thrust;

    [SerializeField] private float m_Mobility;
 
    [SerializeField] private float m_MaxLinearVelocity;

    [SerializeField] private GameObject invulnerable;

    private Rigidbody2D m_Rigidbody;

    private MechanicState currentState = MechanicState.None;

    #endregion

    #region Properties

    public float ThrustControl { get; set; }  
    public float ClampThrustControl { get; set; }
    public float TorqueControl { get; set; }

    public float m_MaxAngularVelocity;

    private float m_AccelerateValue;

    public float AccelerateValue
    {
        get => m_AccelerateValue;
        set => m_AccelerateValue = value < 0 ? 0 : value;
    }

    private float m_Timer;
    public float Timer
    {
        get => m_Timer;
        set => m_Timer = value < 0 ? 0 : value;
    }


    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (invulnerable == null) invulnerable.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();

        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Rigidbody.mass = m_Mass;

        m_Rigidbody.inertia = 1;

        InitOffensive();   
    }
    private void Update()
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer > 0)
        {
            switch (currentState)
            {
                case MechanicState.Invulnerable:
                    SetInvulnerable(true);
                    invulnerable.SetActive(true);
                    break;

                case MechanicState.SpeedBoost:
                    ActivateSpeedBoost();
                    break;
            }
        }
        else
        {
            ResetMechanics();
        }

    }

    private void FixedUpdate()
    {
        UpdateRigidBody();
        UpdateEnergyRegen();
    }

      private void UpdateRigidBody()
    {
        m_Rigidbody.AddForce((ThrustControl ) * m_Thrust * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);
        m_Rigidbody.AddForce(-m_Rigidbody.linearVelocity * ( m_Thrust / m_MaxLinearVelocity ) * Time.fixedDeltaTime, ForceMode2D.Force);

        m_Rigidbody.AddTorque(TorqueControl *  m_Mobility * Time.fixedDeltaTime,ForceMode2D.Force);
        m_Rigidbody.AddTorque(-m_Rigidbody.angularVelocity *  (m_Mobility / m_MaxAngularVelocity) * Time.fixedDeltaTime,ForceMode2D.Force);

        m_Rigidbody.AddForce(ClampThrustControl * m_Thrust * transform.right * Time.fixedDeltaTime, ForceMode2D.Force);
        m_Rigidbody.AddForce(-m_Rigidbody.linearVelocity * (m_Thrust / m_MaxLinearVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
        
       
    }

    #endregion

    [SerializeField] private Turret[] m_Turrets;


    public void Fire(TurretMode mode)
    {     
        foreach (Turret turret in m_Turrets)
        {
            if(turret.Mode == mode)
            {
                turret.Fire();
            }
        }
    }

    [Space(20)]
    [Header(" Ammo properties")]

    [SerializeField] private int m_MaxAmmo;
    [SerializeField] private float m_MaxEnergy;
    [SerializeField] private int m_EnergyRegenPerSecond;

    private float m_PrimaryEnergy;
    private int m_SecondaryAmmo;

    private void InitOffensive()
    {
        m_PrimaryEnergy = m_MaxEnergy;
        m_SecondaryAmmo = m_MaxAmmo;
    }

    public void AddEnergy(float energy)
    {
        m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy +  energy, 0, m_MaxEnergy);
    }

    public void AddAmmo(int ammo)
    {
        m_SecondaryAmmo = Mathf.Clamp(m_SecondaryAmmo + ammo, 0, m_MaxAmmo);
    }

    private void UpdateEnergyRegen()
    {
        m_PrimaryEnergy += m_EnergyRegenPerSecond * Time.fixedDeltaTime;

        m_PrimaryEnergy = Mathf.Clamp(m_PrimaryEnergy , 0, m_MaxEnergy);
    }

    public bool DrawEnergy(float count)
    {
        if(count == 0)
            return true;

        if(m_PrimaryEnergy >= count)
        {
            m_PrimaryEnergy -= count;
            return true;
        }
            
        return false;
    }

    public bool DrawAmmo(int count)
    {
        if (count == 0)
            return true;

        if (m_SecondaryAmmo >= count)
        {
            m_SecondaryAmmo -= count;
            return true;
        }

        return false;
    }

    public void AssignLoadout(TurretProperties props)
    {
        foreach(Turret turret in m_Turrets)
        {
            turret.AssignLoadout(props);
        }
    }

  
    public void ActivateSpeedBoost() 
    {
        ThrustControl = m_AccelerateValue; 
    
    }

    public void SetMechanicState(MechanicState state)
    {
        currentState = state;
    }
    private void ResetMechanics()
    {
        SetInvulnerable(false);
        invulnerable.SetActive(false);
        currentState = MechanicState.None;
    }
}




