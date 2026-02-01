using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] LocomotionVR player;
    [SerializeField] TextMeshPro speedtext;
    [SerializeField] Image gasGauge;
    [SerializeField] Image damagesImage;

    [SerializeField] Color fullGaugeColor;
    [SerializeField] Color emptyGaugeColor;

    private int lastSpeed = 0;

    private void Update()
    {
        gasGauge.rectTransform.localScale = new Vector3(1.0f, player.CurrentGas, 1.0f);
        gasGauge.color = Color.Lerp(emptyGaugeColor, fullGaugeColor, player.CurrentGas);

        speedtext.text = $"{player.CurrentSpeed} km/h";

        if (player.CurrentDifficulty == Difficulty.Hard) HandleFallDamages();

    }

    private IEnumerator FadeDamagesImage()
    {
        Color tempColor = damagesImage.color;

        while (tempColor.a > 0)
        {
            tempColor.a -= Time.deltaTime * 0.5f;
            damagesImage.color = tempColor;
            yield return null;
        }

        tempColor.a = 0;
        damagesImage.color = tempColor;
    }

    void HandleFallDamages()
    {
        float damages = Mathf.Abs(0.03f * (player.CurrentSpeed - lastSpeed)) - 1;

        if (damages > 0)
        {

            Color tempColor = damagesImage.color;
            tempColor.a = Mathf.Clamp01(damages);
            damagesImage.color = tempColor;

            StartCoroutine(FadeDamagesImage());
        }

        lastSpeed = player.CurrentSpeed;
    }
}
