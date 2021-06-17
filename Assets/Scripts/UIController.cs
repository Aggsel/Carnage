using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class AlertMessage{
    [SerializeField] string message;
    [SerializeField] float timer;
    [SerializeField] AnimationCurve fadeCurve;
    [SerializeField] Color color;
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
    [SerializeField] private TextMeshProUGUI healthText = null;
    [SerializeField] private GameObject winText = null;
    [SerializeField] private Renderer targetRenderer = null;
    [SerializeField] private Image fadeImage = null;

    [SerializeField] private GameObject slidersAndActives = null;
    [SerializeField] private GameObject crosshair = null;

    [Header("Poem")]
    [SerializeField] private GameObject poemText = null;
    [SerializeField] private GameObject poemTextAuthor = null;
    [SerializeField] private GameObject poemTextSkip = null;
    [SerializeField] private AlertMessage[] poems = new AlertMessage[3];
    private LevelManager levelManager = null;


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

    private Queue<AlertMessage> alertQueue = new Queue<AlertMessage>();
    private bool alertActive = false;

    private void OnEnable()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        StartCoroutine(Counter());
        StartCoroutine(WhiteFade(false, 0.5f));
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthbar.maxValue = maxHealth;
        //healthText.text = hc.Health + "/" + maxHealth;
    }

    public void UpdateHealthbar()
    {
        float hp = hc.Health;

        healthbar.value = hc.Health;
        healthText.text = hp.ToString("F0") + "/" + hc.MaxHealth;

        if (hp <= 0.0f)
        {
            healthbar.value = hc.Health;
            healthText.text = "0" + "/" + hc.MaxHealth;
        }
    }

    public void SetMaxDashcharge(int maxCharges)
    {
        dashCharges.maxValue = maxCharges;
    }

    public void SetMaxHeat(float maxHeat)
    {
        overheatbar.maxValue = maxHeat;
    }

    public void DisablePoem()
    {
        StopCoroutine(DisplayPoem());

        TextMeshProUGUI textRef = poemText.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI authorTextRef = poemTextAuthor.GetComponent<TextMeshProUGUI>();

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        textRef.text = "";
        authorTextRef.text = "";

        textRef.color = new Color(textRef.color.r, textRef.color.g, textRef.color.b, 0);
        authorTextRef.color = new Color(authorTextRef.color.r, authorTextRef.color.g, authorTextRef.color.b, 0);
        fadeImage.gameObject.SetActive(false);
        poemTextAuthor.SetActive(false);
        poemText.SetActive(false);
        poemTextSkip.SetActive(false);
    }

    public float DisplayPoemText(){
        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        int currentLevel = levelManager.GetCurrentLevel();
        if(currentLevel < 0 || currentLevel >= poems.Length)
            return -1.0f;
        StartCoroutine(DisplayPoem());
        
        AnimationCurve transition = poems[currentLevel].GetCurve();
        float combinedTransitionDuration = transition.keys[transition.length - 1].time * 4.0f;
        return poems[currentLevel].GetTimer() + combinedTransitionDuration;
    }

    public IEnumerator DisplayPoem(){
        TextMeshProUGUI textRef = poemText.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI authorTextRef = poemTextAuthor.GetComponent<TextMeshProUGUI>();
        Color oldColor = fadeImage.color;
        Color backgroundColor = new Color(0,0,0,0);
        fadeImage.color = backgroundColor;

        if(levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
        int currentLevel = levelManager.GetCurrentLevel();

        AlertMessage currentAlert =  poems[currentLevel];
        AnimationCurve curve = currentAlert.GetCurve();

        fadeImage.gameObject.SetActive(true);
    
        float fadeDuration = curve.keys[curve.length - 1].time;

        //Fade in background.
        float time = 0.0f;
        float fade = curve.Evaluate(time);
        while (time < fadeDuration) {
            backgroundColor.a = fade;
            fadeImage.color = backgroundColor;
            yield return null;
            time += Time.deltaTime;
            fade = curve.Evaluate(time);
        }

        //Fade in text.
        textRef.text = currentAlert.GetMessage().Replace("\\n", "\n");
        Color fadeColor = currentAlert.GetColor();
        authorTextRef.color = fadeColor;
        poemTextAuthor.SetActive(true);
        poemText.SetActive(true);

        //now enable skipping of the poem
        poemTextSkip.SetActive(true);
        FindObjectOfType<NextLevelTrigger>().inPoem = true; //weird coupling

        time = 0.0f;
        fade = curve.Evaluate(time);
        while (time < fadeDuration) {
            fadeColor.a = fade;

            textRef.color = fadeColor;
            authorTextRef.color = fadeColor;

            yield return null;
            time += Time.deltaTime;
            fade = curve.Evaluate(time);
        }

        yield return new WaitForSeconds(currentAlert.GetTimer());

        //Fade out text.
        time = fadeDuration;
        fade = curve.Evaluate(time);
        while (time > 0.0f) {
            fadeColor.a = fade;

            textRef.color = fadeColor;
            authorTextRef.color = fadeColor;

            yield return null;
            time -= Time.deltaTime;
            fade = curve.Evaluate(time);
        }
        textRef.color = new Color(0,0,0,0);
        authorTextRef.color = new Color(0,0,0,0);

        //Fade out background.
        time = fadeDuration;
        fade = curve.Evaluate(time);
        while (time > 0.0f) {
            backgroundColor.a = fade;

            fadeImage.color = backgroundColor;

            yield return null;
            time -= Time.deltaTime;
            fade = curve.Evaluate(time);
        }

        textRef.text = "";
        fadeImage.color = oldColor;
        poemText.SetActive(false);
        poemTextAuthor.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        poemTextSkip.SetActive(false);
    }

    public void UIAlertText(string text, float duration = -1.0f, AnimationCurve curve = null, Color? color = null) {
        if (curve == null)
            curve = defaultAnimationCurve;
        if (duration < 0.0f)
            duration = Mathf.Max(minTextDisplayDuration, text.Split(' ').Length / wordsPerSecond);
        if (color == null)
            color = winText.GetComponent<TMPro.TextMeshProUGUI>().color;

        alertQueue.Enqueue(new AlertMessage(text, duration, color, curve));
        if (!alertActive)
            StartCoroutine(DisplayNextAlert());
    }

    private IEnumerator DisplayNextAlert() {
        alertActive = true;
        TextMeshProUGUI textRef = winText.GetComponent<TMPro.TextMeshProUGUI>();

        while (alertQueue.Count > 0) {
            AlertMessage currentAlert = alertQueue.Dequeue();
            AnimationCurve curve = currentAlert.GetCurve();
            float time = 0.0f;
            float fade = curve.Evaluate(time);

            winText.SetActive(true);
            textRef.text = currentAlert.GetMessage();
            Color fadeColor = currentAlert.GetColor();

            float fadeDuration = curve.keys[curve.length - 1].time;
            //Fade in.
            while (time < fadeDuration) {
                fadeColor.a = fade;
                textRef.color = fadeColor;
                yield return null;
                time += Time.deltaTime;
                fade = curve.Evaluate(time);
            }

            yield return new WaitForSeconds(currentAlert.GetTimer());

            //Fade out using same curve but in reverse.
            time = fadeDuration;
            while (time > 0.0f) {
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

        if(Input.GetKeyDown(KeyCode.F3)){
            if(fpsText != null)
                fpsText.gameObject.SetActive(!fpsText.gameObject.activeInHierarchy);
        }
        if(Input.GetKeyDown(KeyCode.F2)){
            if(slidersAndActives != null)
                slidersAndActives.SetActive(!slidersAndActives.activeInHierarchy);
            if(crosshair != null)
                crosshair.SetActive(!crosshair.activeInHierarchy);
        }
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
    private IEnumerator Counter()
    {
        for (; ; ) //while true
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(0.5f);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            float frameCount = Time.frameCount - lastFrameCount;

            string displayText = Mathf.Ceil((frameCount / timeSpan) + 40).ToString();
            fpsText.text = displayText;
        }
    }

    //white effect for fading in and out
    public void ResetWhiteFade()
    {
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator WhiteFade (bool fadeIn, float time) //fadeIn is if its out or in, time should be around 1
    {
        fadeImage.gameObject.SetActive(true);
        float fade = 0.0f;

        if (fadeIn)
        {
            fadeImage.color = new Color(1, 1, 1, 0);

            while (fadeImage.color.a < 1.0f)
            {
                fade = fadeImage.color.a + (Time.deltaTime * time);
                fadeImage.color = new Color(1, 1, 1, fade);
                //fadeImage.color = Color.Lerp(fadeImage.color, new Color(1, 1, 1, 1), Time.deltaTime * time);
                yield return null;
            }
        }
        else
        {
            fadeImage.color = new Color(1, 1, 1, 1);

            while (fadeImage.color.a > 0.0f)
            {
                fade = fadeImage.color.a - (Time.deltaTime * time);
                fadeImage.color = new Color(1, 1, 1, fade);
                //fadeImage.color = Color.Lerp(fadeImage.color, new Color(1, 1, 1, 0), Time.deltaTime * time);
                yield return null;
            }

            fadeImage.gameObject.SetActive(false);
        }
    }
}
