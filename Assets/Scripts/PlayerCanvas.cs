using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] LocomotionVR player;
    [SerializeField] TextMeshPro speedtext;
    [SerializeField] TextMeshPro leftLengthText;
    [SerializeField] TextMeshPro rightLengthText;
    [SerializeField] Image gasGauge;
    [SerializeField] Color fullGaugeColor;
    [SerializeField] Color emptyGaugeColor;

    private void Update()
    {
        gasGauge.rectTransform.localScale = new Vector3(1.0f, player.CurrentGas, 1.0f);
    }

}
