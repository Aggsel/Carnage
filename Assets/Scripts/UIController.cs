using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertMessage{
    string message;
    float timer;
    public AlertMessage(string message, float timer){
        this.message = message;
        this.timer = timer;
    }
    public string GetMessage(){
        return message;
    }
    public float GetTimer(){
        return timer;
    }
}

public class UIController : MonoBehaviour
{
    [Header("Assign: ")]
    [SerializeField] private TextMeshProUGUI fpsText = null;
    [SerializeField] private Slider healthbar = null;
    [SerializeField] private Slider dashCharges = null;
    [SerializeField] private Slider overheatbar = null;
    [SerializeField] private GameObject winText = null;
    [SerializeField] private Renderer targetRenderer = null;
    [SerializeField] private Image fadeImage = null;

    [Header("Assign scripts")]
    [SerializeField] private HealthController hc = null;
    [SerializeField] private MovementController mc = null;
    [SerializeField] private OverheatScript oc = null;

    public Color baseEmissiveColor;
    public float emissionIntensity;
    public float MaxIntensity;
    private MaterialPropertyBlock _propBlock;

    private void OnEnable()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    private Queue<AlertMessage> alertQueue = new Queue<AlertMessage>();
    private bool alertActive = false;

    private void Start ()
    {
        StartCoroutine(Counter());
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthbar.maxValue = maxHealth;
    }

    public void UpdateHealthbar()
    {
        healthbar.value = hc.Health;
    }

    public void SetMaxDashcharge(int maxCharges)
    {
        dashCharges.maxValue = maxCharges;
    }

    public void SetMaxHeat(float maxHeat)
    {
        overheatbar.maxValue = maxHeat;
    }

    public void SetWinText(string text, bool state)
    {
        winText.SetActive(state);
        winText.GetComponent<TMPro.TextMeshProUGUI>().text = text;
    }

    public void UIAlertText(string text, float duration){
        alertQueue.Enqueue(new AlertMessage(text, duration));
        if(!alertActive)
            StartCoroutine(DisplayNextAlert());
    }

    private IEnumerator DisplayNextAlert(){
        alertActive = true;
        while(alertQueue.Count > 0){
            AlertMessage currentAlert = alertQueue.Dequeue();
            SetWinText(currentAlert.GetMessage(), true);
            yield return new WaitForSeconds(currentAlert.GetTimer());
            SetWinText("", true);
        }
        alertActive = false;
    } 

    void Update()
    {
        dashCharges.value = Mathf.Floor(mc.Charge);
        overheatbar.value = oc.HeatValue;
        targetRenderer.GetPropertyBlock(_propBlock, 0);
        _propBlock.SetColor("_EmissiveColor", baseEmissiveColor * Mathf.Lerp(0.0f, MaxIntensity, oc.HeatPercentage));
        targetRenderer.SetPropertyBlock(_propBlock, 0);
    }

    public IEnumerator FadeImage(RawImage image, float fadeTime, bool fadeAway)
    {
        if (fadeAway)
        {
            // fade the overlay
            for (float i = fadeTime; i >= 0.0f; i -= Time.deltaTime)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, i);
                yield return null;
            }
        }
    }

    //Carls andra försök på fps counter :(
    private IEnumerator Counter ()
    {
        for(; ; ) //while true
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(0.5f);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            float frameCount = Time.frameCount - lastFrameCount;

            string displayText = Mathf.Ceil(frameCount / timeSpan).ToString();
            fpsText.text = displayText;
        }
    }

    //white effect for fading in and out
    public IEnumerator WhiteFade (bool fadeIn, float time) //fadeIn is if its out or in, time should be around 1
    {
        float elapsedTime = 0.0f;
        fadeImage.gameObject.SetActive(true);

        if (fadeIn)
        {
            fadeImage.color = new Color(1, 1, 1, 0);

            while (elapsedTime < time)
            {
                fadeImage.color = Color.Lerp(fadeImage.color, new Color(1, 1, 1, 1), Time.deltaTime * time);
                elapsedTime += Time.deltaTime * time;
                yield return null;
            }
        }
        else
        {
            fadeImage.color = new Color(1, 1, 1, 1);

            while (elapsedTime < time)
            {
                fadeImage.color = Color.Lerp(fadeImage.color, new Color(1, 1, 1, 0), Time.deltaTime * time);
                elapsedTime += Time.deltaTime * time;
                yield return null;
            }
        }
    }
}
