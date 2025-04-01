﻿using UnityEngine;

public class MovementControl : MonoBehaviour
{
    public enum ControlMode
    {
        Mobile,
        Keyboard
    }

    [SerializeField] private SpaceShip m_TargetSpaceShip;
    [SerializeField] private VirtualJoyStick m_VirtualJoyStick;
    [SerializeField] private ControlMode m_ControlMode;

    [SerializeField] PointerClickHold m_MobileFirePrimary;
    [SerializeField] PointerClickHold m_MobileFireSecondary;

    
    public void SetTargetShip(SpaceShip ship) => m_TargetSpaceShip = ship;
 
    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            m_ControlMode = ControlMode.Mobile;
            m_VirtualJoyStick.gameObject.SetActive(true);
            m_MobileFirePrimary.gameObject.SetActive(true);
            m_MobileFireSecondary.gameObject.SetActive(true);
        }
        else
        {
            m_ControlMode = ControlMode.Keyboard;
            m_VirtualJoyStick.gameObject.SetActive(false);
            m_MobileFirePrimary.gameObject.SetActive(false);
            m_MobileFireSecondary.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (m_TargetSpaceShip == null) return;

        if (m_ControlMode == ControlMode.Mobile)
        {
            ControlMobile();
            
        }

        if (m_ControlMode == ControlMode.Keyboard)
        {
            ControlKeyboard();
        }

       m_VirtualJoyStick.gameObject.SetActive(m_ControlMode == ControlMode.Mobile);
       m_MobileFirePrimary.gameObject.SetActive(m_ControlMode == ControlMode.Mobile);
       m_MobileFireSecondary.gameObject.SetActive(m_ControlMode == ControlMode.Mobile);

        
    }

    private void ControlMobile()
    {
        var dir = m_VirtualJoyStick.Value;

        m_TargetSpaceShip.ThrustControl = dir.y;
        m_TargetSpaceShip.TorqueControl = -dir.x;

        if (m_MobileFirePrimary.IsHold)
        {
            m_TargetSpaceShip.Fire(TurretMode.Primary);
        }

        if (m_MobileFireSecondary.IsHold)
        {
            m_TargetSpaceShip.Fire(TurretMode.Secondary);
        }


        /*
        Vector2 dir = m_VirtualJoyStick.Value;

        var dot = Vector2.Dot(dir, m_TargetSpaceShip.transform.up);
        var dot2 = Vector2.Dot(dir, m_TargetSpaceShip.transform.right);

        m_TargetSpaceShip.ThrustControl = Mathf.Max(0, dot);
        m_TargetSpaceShip.TorqueControl = -dot2;
        */
    }

    private void ControlKeyboard()
    {
        float thrust = 0;
        float torque = 0;

        if (Input.GetKey(KeyCode.UpArrow))
            thrust = 1.0f;

        if (Input.GetKey(KeyCode.DownArrow))
            thrust = -1.0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            torque = 1.0f;

        if (Input.GetKey(KeyCode.RightArrow))
            torque = -1.0f;

        m_TargetSpaceShip.ThrustControl = thrust;
        m_TargetSpaceShip.TorqueControl = torque;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_TargetSpaceShip.Fire(TurretMode.Primary);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_TargetSpaceShip.Fire(TurretMode.Secondary);
        }
    
    }
}
