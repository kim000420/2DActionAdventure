using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("[PlayerInputHandler] Awake!");
    }

    public float Horizontal
    {
        get
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                return 1f;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                return -1f;
            }
            return 0f;
        }
    }
    public bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
    public bool RollPressed => Input.GetKeyDown(KeyCode.DownArrow);
    public bool CrouchHeld => Input.GetKey(KeyCode.DownArrow);
    public bool GuardHeld => Input.GetKey(KeyCode.UpArrow);
    public bool AttackPressed => Input.GetKeyDown(KeyCode.A);
    public bool SkillPressed => Input.GetKeyDown(KeyCode.Q);
}