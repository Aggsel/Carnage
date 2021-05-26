using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private GameEvent onCombatEnter;
    [SerializeField] private GameEvent onCombatComplete;
    [SerializeField] private Volume combatPostProcessingVolume;
    [SerializeField] private AnimationCurve transitionCurve;

    void OnEnable(){
        if(transitionCurve == null){
            Debug.LogWarning("Transition curve in the post processing manager was not set. Applying backup curve instead.", this.gameObject);
            transitionCurve = AnimationCurve.EaseInOut(0,0,0.5f,1.0f);
        }
        onCombatEnter?.Subscribe(OnCombatEnter);
        onCombatComplete?.Subscribe(OnCombatComplete);
    }
    
    void OnDisable(){
        onCombatEnter?.Unsubscribe(OnCombatEnter);
        onCombatComplete?.Unsubscribe(OnCombatComplete);
    }

    private void OnCombatEnter(){
        StartCoroutine(TransitionToCombat());
    }
    
    private void OnCombatComplete(){
        StartCoroutine(TransitionFromCombat());
    }

    private IEnumerator TransitionToCombat(){
        float timer = 0.0f;
        float animationDuration = transitionCurve.keys[transitionCurve.length-1].time;
        float fade = transitionCurve.Evaluate(timer);

        while(timer < animationDuration){
            combatPostProcessingVolume.weight = fade;
            yield return null;
            timer += Time.deltaTime;
            fade = transitionCurve.Evaluate(timer);
        }
    }

    private IEnumerator TransitionFromCombat(){
        float animationDuration = transitionCurve.keys[transitionCurve.length-1].time;
        float timer = animationDuration;
        float fade = transitionCurve.Evaluate(animationDuration);

        while(timer > 0.0f){
            combatPostProcessingVolume.weight = fade;
            yield return null;
            timer -= Time.deltaTime;
            fade = transitionCurve.Evaluate(timer);
        }
    }

}
