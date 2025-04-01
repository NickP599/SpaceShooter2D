using System;
using UnityEngine;

public class Player : SingletonBase<Player>
{
    [SerializeField] private int m_NumLives;
    [SerializeField] private float TimeToRespawn;
    [SerializeField] private SpaceShip m_Ship;
    [SerializeField] private GameObject m_PlayerShipPrefab;
    public SpaceShip ActiveShip => m_Ship;

    [SerializeField] private CameraController m_CameraController;
    [SerializeField] private MovementControl m_MovementControl;
    [SerializeField] private AxisLockMovementControl m_AxisMovementControl;


    private void Start()
    {
        m_Ship.EventOnDeath.AddListener(OnShipDeath);

        
    }

    private void OnShipDeath()
    {
        m_NumLives--;

        if (m_NumLives > 0)
        {          
            Invoke(nameof(Respawn), TimeToRespawn);
        }
    }

    private void Respawn()
    {
        var newPlayerShip = Instantiate(m_PlayerShipPrefab);
      
        m_Ship = newPlayerShip.GetComponent<SpaceShip>();
        m_Ship.EventOnDeath.AddListener(OnShipDeath);
        
        m_CameraController.SetTarget(m_Ship.transform);
        m_MovementControl.SetTargetShip(m_Ship);
        m_AxisMovementControl.SetTargetShip(m_Ship);
    }
}
