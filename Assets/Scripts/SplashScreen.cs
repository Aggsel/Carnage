using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class SplashClass
{
    public Sprite sprite;
    public float duration;
    public float fadeDuration;
    public float scaleSpeed;
}

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private GameObject splashScreenPrefab = null;
    [SerializeField] private SplashClass[] splashScreens = null;

    private List<GameObject> newSplashScreens = new List<GameObject>();
    private int itteration = 0;

    private void Start ()
    {
        for (int i = 0; i < splashScreens.Length; i++)
        {
            GameObject newScreen = Instantiate(splashScreenPrefab) as GameObject;
            newScreen.transform.SetParent(transform, false);
            newScreen.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            newScreen.GetComponentInChildren<Image>().sprite = splashScreens[i].sprite;
            newScreen.SetActive(false);

            newSplashScreens.Add(newScreen);
        }

        StartCoroutine(CycleSplashScreens());
    }

    private IEnumerator CycleSplashScreens()
    {
        float passedTime = 0.0f;
        float spriteAlpha = 0.0f;
        newSplashScreens[itteration].SetActive(true);

        while (passedTime < (splashScreens[itteration].duration + (splashScreens[itteration].fadeDuration * 2)))
        {
            if(passedTime < splashScreens[itteration].fadeDuration)
            {
                spriteAlpha += Time.deltaTime * splashScreens[itteration].fadeDuration;
                newSplashScreens[itteration].GetComponentInChildren<Image>().color = new Color(1, 1, 1, spriteAlpha);
            }
            else if(passedTime > (splashScreens[itteration].duration + splashScreens[itteration].fadeDuration))
            {
                spriteAlpha -= Time.deltaTime * splashScreens[itteration].fadeDuration;
                newSplashScreens[itteration].GetComponentInChildren<Image>().color = new Color(1, 1, 1, spriteAlpha);
            }

            passedTime += Time.deltaTime;
            newSplashScreens[itteration].transform.localScale += (new Vector3(1, 1, 1) * splashScreens[itteration].scaleSpeed * Time.deltaTime) * 0.1f;
            yield return null;
        }

        newSplashScreens[itteration].SetActive(false);
        itteration++;

        if (itteration < newSplashScreens.Count)
        {
            StartCoroutine(CycleSplashScreens());
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
