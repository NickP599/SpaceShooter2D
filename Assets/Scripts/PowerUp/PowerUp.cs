using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public abstract class PowerUp : MonoBehaviour
{
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.root.TryGetComponent<SpaceShip>(out SpaceShip spaceShip))
        {
            if(Player.Instance.ActiveShip == spaceShip)
            {
                OnPickedUp(spaceShip);

                Destroy(gameObject);
            }
           
        }
    }

    protected virtual void OnPickedUp(SpaceShip spaceShip) { }

   
}
