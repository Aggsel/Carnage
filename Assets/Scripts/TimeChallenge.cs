using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeChallenge : MonoBehaviour
{
    private float timer = 0.0f;
    private bool isActive = true;
    private UIController uic;

    void Start()
    {
        uic = GameObject.Find("Game Controller Controller/Canvas").GetComponent<UIController>();
    }

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
        uic.SetWinText("You completed the timed challenge in: " + timer.ToString("F1") + " seconds! Congratulations!", true);
        StartCoroutine("GoToHub");
    }

    private IEnumerator GoToHub(){
        
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(1);
    }
}
