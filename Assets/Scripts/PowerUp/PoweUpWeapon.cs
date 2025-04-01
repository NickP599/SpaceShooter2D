using UnityEngine;

public class PoweUpWeapon : PowerUp
{
    [SerializeField] TurretProperties m_TurretProperties;

    protected override void OnPickedUp(SpaceShip spaceShip)
    {
        spaceShip.AssignLoadout(m_TurretProperties);
    }
}
 