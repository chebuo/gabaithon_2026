using UnityEngine;

[CreateAssetMenu(fileName = "EscapeData", menuName = "ScriptableObject/EscapeData")]
public class EscapeData : ScriptableObject
{
    public int jumpForceLevel;
    public int gameTimeLevel;
    public int doubleJumpLevel;
}
