using UnityEngine;

[CreateAssetMenu(fileName = "EscapeData", menuName = "ScriptableObject/EscapeData")]
public class EscapeData : ScriptableObject
{
    public float jumpForce;
    public int gameTime;
    public int doubleJump;
}
