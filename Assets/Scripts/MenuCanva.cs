using NUnit.Framework;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanva : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> buttons;

    [SerializeField] Color baseColor;
    [SerializeField] Color highlightColor;

    int currentSelection = 0;

    int numOption = 5;


    public void HandleUpdate()
    {
        if (OVRInput.GetDown(OVRInput.Button.Down)) currentSelection = Mathf.Clamp(currentSelection + 1, 0, numOption-1);
        if (OVRInput.GetDown(OVRInput.Button.Up)) currentSelection = Mathf.Clamp(currentSelection - 1, 0, numOption-1);

        for (int i = 0; i < numOption; i++)
        {
            if (i != currentSelection) buttons[i].color = baseColor;
            else buttons[i].color = highlightColor;
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            switch (currentSelection)
            {
                case 0:
                    GameController.i.SetDiff(Difficulty.Easy);
                    Resume();
                    break;
                case 1:
                    GameController.i.SetDiff(Difficulty.Normal);
                    Resume();
                    break;
                case 2:
                    GameController.i.SetDiff(Difficulty.Hard);
                    Resume();
                    break;
                case 3:
                    Resume();
                    break;
                case 4:
                    SceneManager.LoadScene(0);
                    break;
                default: break;
            }
        }
    }

    void Resume()
    {
        GameController.i.CurrentState = State.FreeRoam;
        this.gameObject.SetActive(false);
    }
}
