using UnityEngine;
/// <summary>
/// Ограничитель позиции.Работает в связке со скриптом LevelBoundary если таковой имеется на сцене.
/// Кидается на объект который надо ограничить.
/// </summary>
public class LevelBoundaryLimiter : MonoBehaviour
{

    public enum Mode
    {
        Radius,
        Border
    }

    [SerializeField] private Mode m_Mode;
  private SpaceShip m_SpaceShip;
    private void Update()
    {
        if (LevelBoundary.Instance == null) return;

        var lb = LevelBoundary.Instance;
        var r = lb.Radius;

        if( m_Mode == Mode.Radius)
        {
            if (transform.position.magnitude > r)
            {
                if (lb.LimitMode == LevelBoundary.Mode.Limit)
                {
                    transform.position = transform.position.normalized * r;
                }

                if (lb.LimitMode == LevelBoundary.Mode.Teleport)
                {
                    transform.position = -transform.position.normalized * r;
                }
            }
        }

        if( m_Mode == Mode.Border)
        {
            transform.position = ClampMovementTarget(transform.position,lb);
        }
    }

    private Vector3 ClampMovementTarget(Vector3 target ,LevelBoundary instance)
    {
        float leftBoard = instance.LeftBorder;
        float rightBoard = instance.RightBorder;

        Vector3 movTarget = target;

        movTarget.z = transform.position.z;
        movTarget.y = transform.position.y;

        if (movTarget.x < leftBoard) movTarget.x = leftBoard;
        if (movTarget.x > rightBoard) movTarget.x = rightBoard;

        return movTarget;
    }
}
