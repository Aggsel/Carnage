using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ChallangeType { NONE, TIME }

public class ChallangeController : MonoBehaviour
{
    [Header("DEBUG: ")]
    [SerializeField] private ChallangeType challangeType = ChallangeType.TIME;

    [Header("Assignables: ")]
    [SerializeField] private TextMeshProUGUI timerText = null;
    [SerializeField] private TextMeshProUGUI bestTimeText = null;
    [SerializeField] private GameObject timerPanel = null;
    [SerializeField] private TextMeshProUGUI challangeTypeText = null;

    [Header("Challange Results")]
    public GameObject challangeResultsObj = null;
    [SerializeField] private TextMeshProUGUI resultsBestTimeText = null;
    [SerializeField] private TextMeshProUGUI resultsTimerText = null;

    private float timer = 0.0f;
    private float bestTime = 0.0f;
    private bool countTimer = false;

    public bool GetCountBool ()
    {
        return countTimer;
    }

    public ChallangeType GetChallange ()
    {
        return challangeType;
    }

    private void Start ()
    {
        LoadChallange(challangeType);
    }

    private void LoadChallange(ChallangeType type)
    {
        switch (type)
        {
            case ChallangeType.NONE:
                timerPanel.SetActive(false);
                break;
            case ChallangeType.TIME:
                challangeTypeText.text = "Time Challange";
                bestTime = PlayerPrefs.GetFloat(type.ToString());

                if (bestTime >= 999999) //magic number
                {
                    bestTimeText.text = "None";
                    resultsBestTimeText.text = "None";
                }
                else
                {
                    bestTimeText.text = bestTime.ToString("F2");
                    resultsBestTimeText.text = bestTime.ToString("F2");
                }
                break;
            default:
                Debug.LogWarning("This should not happen");
                break;
        }

        Debug.Log("Loaded besttime: " + bestTime);
    }

    public void SaveChallange(ChallangeType type)
    {
        switch (type)
        {
            case ChallangeType.NONE:
                break;
            case ChallangeType.TIME:
                float bestTime = PlayerPrefs.GetFloat(type.ToString());

                timer = (float)System.Math.Round(timer, 2);
                bestTime = (float)System.Math.Round(bestTime, 2);

                if (timer < bestTime)
                {
                    bestTimeText.text = timer.ToString("F2");
                    resultsBestTimeText.text = timer.ToString("F2");

                    timerText.text = timer.ToString("F2");
                    resultsTimerText.text = timer.ToString("F2");

                    PlayerPrefs.SetFloat(type.ToString(), timer);
                    bestTime = timer;
                }

                break;
            default:
                Debug.LogWarning("This should not happen");
                break;
        }

        Debug.Log("Saved besttime: " + bestTime);
    }

    public void StopCurrentChallange ()
    {
        switch (challangeType)
        {
            case ChallangeType.NONE:
                break;
            case ChallangeType.TIME:
                //highscore
                SaveChallange(challangeType);

                //challange vars
                countTimer = false;
                break;
            default:
                Debug.LogWarning("This should not happen");
                break;
        }
    }

    public void StartChallange(ChallangeType type)
    {
        challangeType = type;

        switch (challangeType)
        {
            case ChallangeType.NONE:
                timerPanel.SetActive(false);
                break;
            case ChallangeType.TIME:
                //highscore
                LoadChallange(challangeType);

                //challage vars
                countTimer = true;
                timer = 0.0f;
                timerPanel.SetActive(true);
                break;
            default:
                Debug.LogWarning("This should not happen");
                break;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Home))
        {
            PlayerPrefs.SetFloat(challangeType.ToString(), 999999);
            LoadChallange(challangeType);
            Debug.Log("RESET!");
        }

        if (challangeType == ChallangeType.TIME && countTimer)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("F2");
            resultsTimerText.text = timer.ToString("F2");
        }
    }
}