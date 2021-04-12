using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Room Generation/Level", order = 1)]
public class LevelAsset : ScriptableObject
{    
    [SerializeField] private RoomAsset[] rooms = null;
    [SerializeField] private float[] weights = null;
    private float sumOfWeights;

    //This assumes that are rooms have 4 doors.
    public GameObject GetRandomRoom(){
        sumOfWeights = 0;
        for (int i = 0; i < weights.Length; i++){
            sumOfWeights += weights[i];
        }
        int randomIndex = Random.Range(0, rooms.Length);
        return rooms[randomIndex].GetRoom();
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(LevelAsset))]
[CanEditMultipleObjects]
public class LevelAssetEditor : Editor 
{
    SerializedProperty rooms = null;
    SerializedProperty weights = null;

    void OnEnable(){
        rooms = serializedObject.FindProperty("rooms");
        weights = serializedObject.FindProperty("weights");
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 10.0f;
        EditorGUILayout.PropertyField(rooms);

        EditorGUIUtility.labelWidth = 10.0f;
        EditorGUILayout.PropertyField(weights);
        
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif