using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertMessage{
    string message;
    float timer;
    AnimationCurve fadeCurve;
    Color color;
    public AlertMessage(string message, float timer, Color? color = null, AnimationCurve fadeCurve = null){
        this.message = message;
        this.timer = timer;
        this.fadeCurve = fadeCurve;
        this.color = color.GetValueOrDefault();
    }
    public string GetMessage(){
        return message;
    }
    public AnimationCurve GetCurve(){
        return fadeCurve;
    }
    public Color GetColor(){
        return color;
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

    [Header("Alerts")]
    [Tooltip("How many words per second a player is reading at. Used for dynamically scaling alert text duration on screen.")]
    [SerializeField] private float wordsPerSecond = 3.0f;
    [Tooltip("When very short texts using dynamic duration is used, what will be the shortest duration allowed?")]
    [SerializeField] private float minTextDisplayDuration = 1.0f;
    [Tooltip("When no other animation curve is supplied to the text, this is the backup one to use.")]
    [SerializeField] private AnimationCurve defaultAnimationCurve = new AnimationCurve();

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

    public void UIAlertText(string text, float duration = -1.0f, AnimationCurve curve = null, Color? color = null){
        if(curve == null)
            curve = defaultAnimationCurve;
        if(duration < 0.0f)
            duration = Mathf.Max(minTextDisplayDuration, text.Split(' ').Length / wordsPerSecond);
        if(color == null)
            color = winText.GetComponent<TMPro.TextMeshProUGUI>().color;

        alertQueue.Enqueue(new AlertMessage(text, duration, color, curve));
        if(!alertActive)
            StartCoroutine(DisplayNextAlert());
    }

    private IEnumerator DisplayNextAlert(){
        alertActive = true;
        TextMeshProUGUI textRef = winText.GetComponent<TMPro.TextMeshProUGUI>();

        while(alertQueue.Count > 0){
            AlertMessage currentAlert = alertQueue.Dequeue();
            AnimationCurve curve = currentAlert.GetCurve();
            float time = 0.0f;
            float fade = curve.Evaluate(time);

            winText.SetActive(true);
            textRef.text = currentAlert.GetMessage();
            Color fadeColor = currentAlert.GetColor();;

            float fadeDuration = curve.keys[curve.length-1].time;
            //Fade in.
            while(time < fadeDuration){
                fadeColor.a = fade;
                textRef.color = fadeColor;
                yield return null;
                time += Time.deltaTime;
                fade = curve.Evaluate(time);
            }

            yield return new WaitForSeconds(currentAlert.GetTimer());

            //Fade out using same curve but in reverse.
            time = fadeDuration;
            while(time > 0.0f){
                fadeColor.a = fade;
                textRef.color = fadeColor;
                yield return null;
                time -= Time.deltaTime;
                fade = curve.Evaluate(time);
            }

            textRef.text = "";
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
