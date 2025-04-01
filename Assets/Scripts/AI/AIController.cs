using System;
using UnityEngine;

public class AIController : MonoBehaviour
{
   public enum AIBehaviour
    {
        None,
        Patrol
    }

    [SerializeField] private AIBehaviour m_AIBehaviour;
    [SerializeField] private AIPointPatrol m_AIPointPatrol;    
    
    [Range(0.0f, 1f)]
    [SerializeField] private float m_NavigationLinear;

    [Range(0.0f, 1f)]
    [SerializeField] private float m_NavigationAngular;

    [SerializeField] private float m_RandomSelectMovePointTime;

    [SerializeField] private float m_FindNewTargetTime;

    [SerializeField] private float m_ShootDelay;

    [SerializeField] private float m_EvadeRayLength;

    private SpaceShip m_SpaceShip;

    private Vector3 m_MovePosition;

    private Destructible m_SelectedTarget;

    private Timer m_RandomizeDirectionTimer;
    private Timer m_FireTimer;
    private Timer m_FindNewTargetTimer;

    private void Start()
    {
       m_SpaceShip = GetComponent<SpaceShip>();
       InitTimers();

        float maxV = float.MaxValue;

        Debug.Log(maxV);
    }

    private void Update()
    {
        UpdateTimers();
        UpdateAI();
    }
  
    private void UpdateAI()
    {
        if(m_AIBehaviour == AIBehaviour.None)
        {

        }

        if(m_AIBehaviour == AIBehaviour.Patrol)
        {
            UpdateBehaviourPatrol();
        }
    }

    private void UpdateBehaviourPatrol()
    {
        ActionFindNewMovePosition();
        ActionControlShip();
        ActionFindNewAttackTarget();
        ActionFire();
        ActionEvadeCollision();
    }

    private Destructible FindNearestDestructible()
    {
        float maxDist = float.MaxValue;

        Destructible potentialTarget = null;

        foreach( var v in Destructible.AllDestructible)
        {
            if (v.GetComponent<SpaceShip>() == m_SpaceShip) continue;

            if(v.TeamId == Destructible.TEAM_ID_NEUTRAL) continue;

            if(v.TeamId == m_SpaceShip.TeamId) continue;

            float dist = Vector2.Distance(m_SpaceShip.transform.position, v.transform.position);

            if(dist < maxDist)
            {
                maxDist = dist;
                potentialTarget = v;
            } 
        } 

        return potentialTarget;
    }

    private void ActionFire()
    {
        if(m_SelectedTarget != null)
        {
            if (m_FireTimer.IsFinished)
            {
                 m_SpaceShip.Fire(TurretMode.Primary);

                m_FireTimer.RestartTimer();
            }
        }
    }

    private void ActionFindNewAttackTarget()
    {
        if (m_FindNewTargetTimer.IsFinished)
        {
            m_SelectedTarget = FindNearestDestructible();

            m_FindNewTargetTimer.RestartTimer();
        }
    }
   
    private void ActionEvadeCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, m_EvadeRayLength);

        if (hit)
        {
            m_MovePosition = transform.position + transform.right * 100.0f;

            Debug.DrawRay(transform.position,hit.point, Color.red);
        
            {
                /// добавить функционал ухода от столкновения с коллизией !!
            }
        }
    }

    private void ActionControlShip()
    {
        m_SpaceShip.ThrustControl = m_NavigationLinear;
        m_SpaceShip.TorqueControl = ComputeAlignTorqueNormalized(m_MovePosition, m_SpaceShip.transform) * m_NavigationAngular;
    }

    private const float MAX_ANGEL = 45.0f;

    private static float ComputeAlignTorqueNormalized(Vector3 targetPosition , Transform ship)
    {
        Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);

        float angle = Vector3.SignedAngle(localTargetPosition,Vector3.up,Vector3.forward);

        angle = Mathf.Clamp(angle,-MAX_ANGEL,MAX_ANGEL ) / MAX_ANGEL;


        return - angle;
    }

    private void ActionFindNewMovePosition() 
    {
        if (m_SelectedTarget != null)
        {
            m_MovePosition = m_SelectedTarget.transform.position;
        }
        else
        {
            if (m_AIPointPatrol != null)
            {
                bool isInsidePatrolZone = (m_AIPointPatrol.transform.position - transform.position).sqrMagnitude < m_AIPointPatrol.Radius * m_AIPointPatrol.Radius;

                if (isInsidePatrolZone)
                {
                    if (m_RandomizeDirectionTimer.IsFinished)
                    {
                        Vector2 newPoint = m_AIPointPatrol.transform.position + UnityEngine.Random.onUnitSphere * m_AIPointPatrol.Radius;

                        m_MovePosition = newPoint;

                        m_RandomizeDirectionTimer.SetStartTime(m_RandomSelectMovePointTime);
                    }
                }
                else
                {
                    m_MovePosition = m_AIPointPatrol.transform.position;
                }
            }
        }
    }

    public void SetPatrolBehaviour(AIPointPatrol patrol)
    {
        m_AIBehaviour = AIBehaviour.Patrol;
        m_AIPointPatrol = patrol;
    }

    #region Timers

    private void InitTimers()
    {
        m_RandomizeDirectionTimer = new Timer(m_RandomSelectMovePointTime);
        m_FireTimer = new Timer(m_ShootDelay);
        m_FindNewTargetTimer = new Timer(m_FindNewTargetTime);
    }

    private void UpdateTimers()
    {
        m_RandomizeDirectionTimer.RemoveTime(Time.deltaTime);
        m_FireTimer.RemoveTime(Time.deltaTime);
        m_FindNewTargetTimer.RemoveTime(Time.deltaTime);
    }


    #endregion
}
