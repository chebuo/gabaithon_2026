using UnityEngine;

public class SelectPlayerController : MonoBehaviour
{
    [SerializeField]private GameObject playerModel;
    Rigidbody rb;

    void Awake()
    {
        rb=this.GetComponent<Rigidbody>();
    }

    public void Move(Vector2 speed)
    {
        rb.linearVelocity=new Vector3(speed.x,rb.linearVelocity.y,speed.y);
    }

    public void ChangeDir(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero)
            return;

        Vector3 direction = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        playerModel.transform.forward = direction;
    }
}