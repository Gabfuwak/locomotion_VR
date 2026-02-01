using UnityEngine;

public enum State
{
    FreeRoam,
    Pause
};

public class GameController : MonoBehaviour
{
    [SerializeField] MenuCanva menu;
    [SerializeField] LocomotionBase player;

    public State CurrentState = State.FreeRoam;


    public static GameController i;

    private void Awake()
    {
        i = this;
    }

    private void Update()
    {
        if (CurrentState == State.FreeRoam) player.HandleUpdate();
        else menu.HandleUpdate();

        if (OVRInput.GetDown(OVRInput.Button.Start) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (CurrentState == State.FreeRoam)
            {
                CurrentState = State.Pause;
                menu.gameObject.SetActive(true);
            }
            else CurrentState = State.FreeRoam;
        }
    }

    public void SetDiff(Difficulty diff)
    {
        player.CurrentDifficulty = diff;
    }
}
