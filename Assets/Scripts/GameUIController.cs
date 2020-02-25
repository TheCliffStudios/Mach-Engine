using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public float Timer = 0;
    public float RingCount = 0;
    public float Speed = 0;
    public float Lives = 3;
    public float MaxSpeed = 100;

    public Image SpeedBarFill;
    public TMPro.TextMeshProUGUI TimerText;
    public TMPro.TextMeshProUGUI RingText;
    public TMPro.TextMeshProUGUI LivesText;

    // Update is called once per frame
    void Update()
    {
        RingText.text = RingCount.ToString("000");
        LivesText.text = Lives.ToString("000");

        int M = Mathf.FloorToInt((Timer / 60) / 60);
        int S = Mathf.FloorToInt((Timer / 60) - (M*60));
        int MS = Mathf.FloorToInt(Timer - (S * 60) - (M*60*60));

        TimerText.text = (M.ToString("00") + ":" + S.ToString("00") + ":" + MS.ToString("00"));

        SpeedBarFill.fillAmount = (Speed / MaxSpeed);
    }
}
