using UnityEngine;

public class PlayerControllerC : MonoBehaviour
{
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb=this.GetComponent<Rigidbody>();
    }

    public void Move(float speed)
    {
        rb.linearVelocity=new Vector3(speed,rb.linearVelocity.y,rb.linearVelocity.z);
    }

    public void Jump(float jumpForce)
    {
        rb.linearVelocity=new Vector3(rb.linearVelocity.x,0,rb.linearVelocity.z);
        rb.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
    }
}
