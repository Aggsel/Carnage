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
        if(this.reference == null){
            return;
        }
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
    internal void Stop(){
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}

public class AudioManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] public EventContainer playerDeath;
    [SerializeField] public EventContainer playerHurt;
    [SerializeField] public EventContainer playerMelee;
    [SerializeField] public EventContainer playerShooting;
    [SerializeField] public EventContainer playerOverheat;
    [SerializeField] public EventContainer playerExplosion;
    [SerializeField] public EventContainer playerDash;
    [SerializeField] public EventContainer playerFootsteps;
    [SerializeField] public EventContainer playerJump;
    [SerializeField] public EventContainer playerLand;

    [Header("Enemies - Constraint")]
    [SerializeField] public EventContainer patientDeath;
    [SerializeField] public EventContainer patientHurt;
    [SerializeField] public EventContainer patientMelee;
    [SerializeField] public EventContainer patientFootsteps;
    [SerializeField] public EventContainer patientSpawn;
    [SerializeField] public EventContainer patientProjectile;

    [Header("Enemies - Rage")]
    [SerializeField] public EventContainer rageCharge;
    [SerializeField] public EventContainer rageDeath;
    [SerializeField] public EventContainer rageHurt;
    [SerializeField] public EventContainer rageMelee;
    [SerializeField] public EventContainer rageSpawn;
    [SerializeField] public EventContainer rageFootsteps;

    [Header("Items")]
    [SerializeField] public EventContainer itemsActivate;
    [SerializeField] public EventContainer itemsHealing;
    [SerializeField] public EventContainer itemsPickup;

    [Header("Music")]
    [SerializeField] public EventContainer ambManager;

    [Header("Misc")]
    [SerializeField] public EventContainer endOfLevelBell;

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

    public void PlaySound(EventContainer eventContainer, GameObject sourceObject)
    {
        eventContainer.Play(sourceObject);
    }

    public void StopSound(ref EventContainer eventContainer){
        eventContainer.Stop();
    }

    public void StopSound(EventContainer eventContainer)
    {
        eventContainer.Stop();
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
                    Debug.LogError("Audio Manager not properly configured in this scene. Please add the AudioManager prefab to the scene if you want audio.");
                }
            }
            return _managerInstance;
        }
    }
}
