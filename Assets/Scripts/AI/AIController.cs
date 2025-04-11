using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{    
   public enum AIBehaviour
    {
        None,
        PatrolToTarget,
        RandomPatrol
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
    private Vector3 m_LookAtPosition;

    private Destructible m_SelectedTarget;
    private NavMeshAgent m_NavAgent;

    private Timer m_RandomizeDirectionTimer;
    private Timer m_FireTimer;
    private Timer m_FindNewTargetTimer;

    private void Start()
    {
       m_SpaceShip = GetComponent<SpaceShip>();
       m_NavAgent = GetComponent<NavMeshAgent>();

       InitTimers();
       
    }

    private void Update()
    {
        UpdateTimers();
        UpdateAI();
    }
  
    private void UpdateAI()
    {
        if(m_AIBehaviour == AIBehaviour.PatrolToTarget)
        {
            UpdateBehaviourToTargetPatrol();
        }

        if(m_AIBehaviour == AIBehaviour.RandomPatrol)
        {
            UpdateBehaviourRandomPatrol();
        }
    }
    private void UpdateBehaviourToTargetPatrol()
    {
       ActionFindMoveToTargetPosition();
       ActionControlShip();
       ActionEvadeCollision();

    }

    private void UpdateBehaviourRandomPatrol()
    {
        ActionFindNewRandomMovePositionInPointZone();
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
        if (m_SelectedTarget != null)
        {
            Vector2 moveDirection = (m_SelectedTarget.transform.position - m_SpaceShip.transform.position).normalized;
            Collider2D tragetColl = m_SelectedTarget.GetComponentInChildren<Collider2D>();

            if (tragetColl != null)
            {

                RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, m_EvadeRayLength);

                if (hit.collider != null)
                {
                    if (hit.distance < 3)
                    {

                        if (hit.collider == tragetColl)
                        {
                            Vector2 avoidanceDirection = Vector2.Perpendicular(moveDirection).normalized;

                            m_SpaceShip.transform.position = (Vector2)transform.position + avoidanceDirection * m_NavigationAngular;
                          
                        }
                    }
                }
            }
        }
    }


    private void ActionControlShip()
    {
        m_SpaceShip.ThrustControl = m_NavigationLinear;
        m_SpaceShip.TorqueControl = ComputeAlignTorqueNormalized(m_LookAtPosition, m_SpaceShip.transform) * m_NavigationAngular;

    }

    private const float MAX_ANGEL = 45.0f;

    private static float ComputeAlignTorqueNormalized(Vector3 targetPosition , Transform ship)
    {
        Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);

        float angle = Vector3.SignedAngle(localTargetPosition,Vector3.up,Vector3.forward);

        angle = Mathf.Clamp(angle,-MAX_ANGEL,MAX_ANGEL ) / MAX_ANGEL;

        return - angle;
    }

    private void ActionFindMoveToTargetPosition()
    {    
        if (m_SelectedTarget != null)
        {         
            SetMoveAndLookTarget(m_SelectedTarget.transform, m_SpaceShip.ThrustControl);
        }
        else
        {
            if (m_AIPointPatrol.TargetPoints.Length == 0) return;

            Vector2 targetPoint = m_AIPointPatrol.TargetPoints[m_AIPointPatrol.CurrentPointIndex].position;

            float distance = Vector2.Distance(targetPoint, transform.position);

            bool isReachedTarget = distance < 0.2f;

            if (!isReachedTarget)
            {
                m_MovePosition = targetPoint;
            }
            else
            {
                m_AIPointPatrol.CurrentPointIndex = (m_AIPointPatrol.CurrentPointIndex + 1) % m_AIPointPatrol.TargetPoints.Length;
            }
        }

    }
   

    private void ActionFindNewRandomMovePositionInPointZone() 
    {

        if (m_SelectedTarget != null)
        {
            SetMoveAndLookTarget(m_SelectedTarget.transform, m_SpaceShip.ThrustControl);
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

                        m_RandomizeDirectionTimer.RestartTimer();
                    }
                }
                else
                {                 
                    m_MovePosition = m_AIPointPatrol.transform.position;
                }
            }
        }
    }

    private void SetMoveAndLookTarget(Transform target, float moveSpeed, bool lookAtTargetInsteadOfLead = true)
    {
        Vector2 shipPos = m_SpaceShip.transform.position;
        Vector2 targetPos = target.position;
        Vector2 targetVelocity = Vector2.zero;
        Vector2 offset = UnityEngine.Random.insideUnitCircle * 1.5f;

        if (target.TryGetComponent<Rigidbody2D>(out var rb))
            targetVelocity = rb.linearVelocity; 

        m_MovePosition = MakeLead(shipPos, targetPos, targetVelocity, moveSpeed) + offset;

        m_LookAtPosition = lookAtTargetInsteadOfLead ? targetPos : m_MovePosition;

        Debug.DrawLine(shipPos, m_MovePosition, Color.red, 0.1f);
        Debug.DrawLine(shipPos, m_LookAtPosition, Color.green, 0.1f);
    }

    public static Vector2 MakeLead(Vector2 shooterPos, Vector2 targetPos, Vector2 targetVelocity, float moveSpeed)
    {
        if (moveSpeed <= 0.01f)
            return targetPos;

        Vector2 dir = targetPos - shooterPos;

        float distance = dir.magnitude;

        float timeToReach = distance / moveSpeed;

        return targetPos + targetVelocity * timeToReach;
    }



    public void SetPatrolBehaviour(AIPointPatrol patrol)
    {
        m_AIBehaviour = AIBehaviour.RandomPatrol;
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
