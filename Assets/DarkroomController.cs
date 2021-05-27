using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkroomController : MonoBehaviour
{

    private UIController uic = null;

    // Start is called before the first frame update
    void Start()
    {
        uic = GameObject.Find("Game Controller Controller/Canvas").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
