using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    void Start(){
        AudioManager am = AudioManager.Instance;
        am.PlaySound(am.playerShooting);
    }
}
