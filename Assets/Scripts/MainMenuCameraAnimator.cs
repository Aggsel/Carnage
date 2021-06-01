using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Points{
    [SerializeField] internal Transform startPoint = null;
    [SerializeField] internal Transform endPoint = null;
    [SerializeField] internal float animationDuration = 4.0f;
    [SerializeField] internal float fadeDuration = 3.0f;
}

public class MainMenuCameraAnimator : MonoBehaviour
{
    [SerializeField] Image fadeImage = null;
    [SerializeField] Points[] animationKeys = new Points[0];
    Points currentPoint = null;
    float currentDuration = 0.0f;
    int index = 0;
    Color fadeColor;

    void Start(){
        index = Random.Range(0,animationKeys.Length);
        currentPoint = animationKeys[index];
        fadeColor = new Color(0,0,0,0);
    }

    void Update(){
        //Fade in and fade out alpha of fadeImage.
        fadeColor.a = Mathf.Max((1.0f - (currentDuration / currentPoint.fadeDuration)), (currentDuration - (currentPoint.animationDuration - currentPoint.fadeDuration))/currentPoint.fadeDuration);
        fadeImage.color = fadeColor;
        currentDuration += Time.deltaTime;
        transform.parent.position = Vector3.Lerp(currentPoint.startPoint.position, currentPoint.endPoint.position, currentDuration / currentPoint.animationDuration);
        transform.parent.transform.rotation = Quaternion.Slerp(currentPoint.startPoint.rotation, currentPoint.endPoint.rotation, currentDuration / currentPoint.animationDuration);
        
        if(currentDuration > currentPoint.animationDuration){
            currentDuration = 0.0f;
            index = (index + 1) % animationKeys.Length;
            currentPoint = animationKeys[index];
        }
    }
}
