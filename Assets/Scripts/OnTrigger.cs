using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    public UnityEvent Enter;

    public UnityEvent Exit;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Exit.Invoke();
    }
}
