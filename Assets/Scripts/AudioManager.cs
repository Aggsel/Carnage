using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[System.Serializable]
public struct EventContainer{
    [FMODUnity.EventRef] public string reference;
    public EventInstance instance;
    private bool initialized;
    public void Initialize(){
        instance = RuntimeManager.CreateInstance(this.reference);
        initialized = true;
    }
    internal void Play(){
        if(!initialized)
            Initialize();
        instance.start();
    }
    internal void Play(Vector3 source){
        if(!initialized)
            Initialize();
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(source));
        instance.start();
    }
    internal void Play(GameObject source){
        if(!initialized)
            Initialize();
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(source));
        instance.start();
    }
    internal void SetParameterByName(string parameter, float value){
        if(!initialized)
            Initialize();
        instance.setParameterByName(parameter, value);
    }
}

public class AudioManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] public EventContainer playerDeath;
    [SerializeField] public EventContainer playerHurt;
    [SerializeField] public EventContainer playerMelee;
    [SerializeField] public EventContainer playerShooting;
    [SerializeField] public EventContainer playerDash;
    [SerializeField] public EventContainer playerFootsteps;
    [SerializeField] public EventContainer playerJump;
    [SerializeField] public EventContainer playerLand;

    [Header("Enemies - Patient")]
    [SerializeField] public EventContainer patientDeath;
    [SerializeField] public EventContainer patientHurt;
    [SerializeField] public EventContainer patientMelee;
    [SerializeField] public EventContainer patientFootsteps;
    [SerializeField] public EventContainer patientSpawn;

    public void PlaySound(EventContainer eventContainer){
        eventContainer.Play();
    }

    public void PlaySound(ref EventContainer eventContainer){
        eventContainer.Play();
    }

    public void PlaySound(ref EventContainer eventContainer, Vector3 sourcePosition){
        eventContainer.Play(sourcePosition);
    }

    public void PlaySound(ref EventContainer eventContainer, GameObject sourceObject){
        eventContainer.Play(sourceObject);
    }

    public void SetParameterByName(ref EventContainer eventContainer, string parameter, float value){
        eventContainer.SetParameterByName(parameter, value);
    }

    public static AudioManager _managerInstance;
    public static AudioManager Instance{
        get {
            if (_managerInstance == null){
                _managerInstance = GameObject.FindObjectOfType<AudioManager>();
                if (_managerInstance == null){
                    GameObject AudioManager = new GameObject("AudioManager");
                    _managerInstance = AudioManager.AddComponent<AudioManager>();
                }
            }
            return _managerInstance;
        }
    }
}
