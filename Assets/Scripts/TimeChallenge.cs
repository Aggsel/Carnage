using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeChallenge : MonoBehaviour
{
    private float timer = 0.0f;
    private bool isActive = true;

    void Update(){
        if(isActive)
            timer += Time.deltaTime;
    }

    // void OnGUI(){
    //     GUIStyle style = new GUIStyle(GUI.skin.label);
    //     style.fontSize = 40;
    //     GUI.Label(new Rect(50, 20, 200, 200), timer.ToString("F2"), style);
    // }

    void OnTriggerEnter(Collider other){
        isActive = false;
        StartCoroutine("GoToHub");
    }

    private IEnumerator GoToHub(){
        
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("Hub");
    }
}
