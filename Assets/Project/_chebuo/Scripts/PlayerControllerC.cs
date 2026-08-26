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
        
    }

    public void Jump(float jumpForce)
    {
        rb.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
    }
}
