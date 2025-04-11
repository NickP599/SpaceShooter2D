using System;
using UnityEngine;
using UnityEngine.AI;


public class AIController : MonoBehaviour
{
    #region Enums

    public enum AIBehaviour
    {
        None,
        PatrolToTarget,
        RandomPatrol
    }

    #endregion

    #region Serialized Fields

    [SerializeField] private AIBehaviour m_AIBehaviour;
    [SerializeField] private AIPointPatrol m_AIPointPatrol;

    [Range(0.0f, 1f)][SerializeField] private float m_NavigationLinear;
    [Range(0.0f, 1f)][SerializeField] private float m_NavigationAngular;
    [Range(0.0f, 0.1f)][SerializeField] private float m_LeadSpeed;

    [SerializeField] private float m_RandomSelectMovePointTime;
    [SerializeField] private float m_FindNewTargetTime;
    [SerializeField] private float m_ShootDelay;
    [SerializeField] private float m_EvadeRayLength;

    #endregion

    #region Private Fields

    private SpaceShip m_SpaceShip;
    private Destructible m_SelectedTarget;
    private Turret m_Turret;

    private Vector3 m_MovePosition;
    private Vector3 m_LookAtPosition;

    private Timer m_RandomizeDirectionTimer;
    private Timer m_FireTimer;
    private Timer m_FindNewTargetTimer;

    #endregion

    #region Unity Methods

    private void Start()
    {
        m_SpaceShip = GetComponent<SpaceShip>();
        m_Turret = GetComponentInChildren<Turret>();
        InitTimers();
    }

    private void Update()
    {
        UpdateTimers();
        UpdateAI();
    }

    #endregion

    #region AI Logic

    private void UpdateAI()
    {
        if (m_AIBehaviour == AIBehaviour.PatrolToTarget)
        {
            UpdateBehaviourToTargetPatrol();
        }

        if (m_AIBehaviour == AIBehaviour.RandomPatrol)
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

    #endregion

    #region Actions

    private void ActionFire()
    {
        if (m_SelectedTarget != null && m_FireTimer.IsFinished)
        {         
            m_SpaceShip.Fire(TurretMode.Primary);
            m_FireTimer.RestartTimer();       
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
        if (Physics2D.Raycast(transform.position, transform.up, m_EvadeRayLength))
        {
            m_MovePosition = transform.position + transform.right * 1000f;
        }
    }

    private void ActionControlShip()
    {
        m_SpaceShip.ThrustControl = m_NavigationLinear;
        m_SpaceShip.TorqueControl = ComputeAlignTorqueNormalized(m_MovePosition, m_SpaceShip.transform) * m_NavigationAngular;
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

    #endregion

    #region Utility Methods

    private Destructible FindNearestDestructible()
    {
        float maxDist = float.MaxValue;
        Destructible potentialTarget = null;

        foreach (var v in Destructible.AllDestructible)
        {
            if (v.GetComponent<SpaceShip>() == m_SpaceShip) continue;
            if (v.TeamId == Destructible.TEAM_ID_NEUTRAL) continue;
            if (v.TeamId == m_SpaceShip.TeamId) continue;

            float dist = Vector2.Distance(m_SpaceShip.transform.position, v.transform.position);

            if (dist < maxDist)
            {
                maxDist = dist;
                potentialTarget = v;
            }
        }

        return potentialTarget;
    }

    private static float ComputeAlignTorqueNormalized(Vector3 targetPosition, Transform ship)
    {
        Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);

        float angle = Vector3.SignedAngle(localTargetPosition, Vector3.up, Vector3.forward);

        angle = Mathf.Clamp(angle, -MAX_ANGEL, MAX_ANGEL) / MAX_ANGEL;

        return -angle;
    }

    private void SetMoveAndLookTarget(Transform target, float moveSpeed, bool lookAtTargetInsteadOfLead = true)
    {
        Vector2 shipPos = m_SpaceShip.transform.position;
        Vector2 targetPos = target.position;
        Vector2 targetVelocity = Vector2.zero;

        if (target.TryGetComponent<Rigidbody2D>(out var rb))
            targetVelocity = rb.linearVelocity;

        m_MovePosition = MakeLead(shipPos, targetPos, targetVelocity, moveSpeed, m_LeadSpeed);
        m_LookAtPosition = lookAtTargetInsteadOfLead ? targetPos : m_MovePosition;

        Debug.DrawLine(shipPos, m_MovePosition, Color.red, 0.1f);
        Debug.DrawLine(shipPos, m_LookAtPosition, Color.green, 0.1f);
    }

    public static Vector2 MakeLead(Vector2 shooterPos, Vector2 targetPos, Vector2 targetVelocity, float moveSpeed , float leadFactor)
    {
        if (moveSpeed <= 0.01f)
            return targetPos;

        Vector2 dir = targetPos - shooterPos;

        float distance = dir.magnitude;
        float timeToReach = distance / moveSpeed;

        timeToReach *= leadFactor;

        return targetPos + targetVelocity * timeToReach;
    }

    public void SetPatrolBehaviour(AIPointPatrol patrol)
    {
        m_AIBehaviour = AIBehaviour.RandomPatrol;
        m_AIPointPatrol = patrol;
    }

    #endregion

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

    #region Constants

    private const float MAX_ANGEL = 45.0f;

    #endregion
}
