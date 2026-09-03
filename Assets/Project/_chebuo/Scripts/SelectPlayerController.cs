using UnityEngine;

public class SelectPlayerController : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb=this.GetComponent<Rigidbody>();
    }

    public void Move(Vector2 speed)
    {
        rb.linearVelocity=new Vector3(speed.x,rb.linearVelocity.y,speed.y);
    }

    public void CheckSelectObject()
    {
        
    }
}