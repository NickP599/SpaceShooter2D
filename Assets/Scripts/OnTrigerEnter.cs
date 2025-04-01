using UnityEngine;


public  class OnTrigerEnter : MonoBehaviour
{

    private Projectile projectile;
    private void Start()
    {
        projectile = transform.root.GetComponent<Projectile>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.transform.root.TryGetComponent<Destructible>(out Destructible destructible))
        {
            destructible.ApplyDamage(projectile.Damage);
        }
        
    }


    
}
