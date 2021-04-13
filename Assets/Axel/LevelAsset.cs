using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Room Generation/Level", order = 1)]
public class LevelAsset : ScriptableObject
{    
    [SerializeField] private RoomAsset[] rooms = null;
    [SerializeField] private int[] weights = null;
    [SerializeField] private float[] normalizedWeights = null;
    [HideInInspector] [SerializeField] private int sumOfWeights;
    [SerializeField] private List<string> keyList = new List<string>();

    public GameObject GetRandomRoom(int doorMask = -1){
        List<RoomAsset> compatibleRooms = new List<RoomAsset>();
        for (int i = 0; i < rooms.Length; i++){
            if(RoomAsset.CompatibleDoorMask(doorMask, rooms[i].GetDoorMask()))
                compatibleRooms.Add(rooms[i]);
        }
        int randomIndex = Random.Range(0, compatibleRooms.Count);
        return compatibleRooms[randomIndex].GetRoom();
    }

    private void OnValidate(){
        for (int i = 0; i < weights.Length; i++){
            weights[i] = Mathf.Max(weights[i], 0);
        }

        sumOfWeights = 0;
        for (int i = 0; i < weights.Length; i++){
            sumOfWeights += weights[i];
        }

        normalizedWeights = new float[this.weights.Length];
        for (int i = 0; i < this.weights.Length; i++){
            this.normalizedWeights[i] = this.weights[i]/(float)sumOfWeights;
        }
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(LevelAsset))]
[CanEditMultipleObjects]
public class LevelAssetEditor : Editor 
{
    SerializedProperty rooms = null;
    SerializedProperty weights = null;
    SerializedProperty normalizedWeights = null;

    void OnEnable(){
        rooms = serializedObject.FindProperty("rooms");
        weights = serializedObject.FindProperty("weights");
        normalizedWeights = serializedObject.FindProperty("normalizedWeights");
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Add new room")){
            rooms.InsertArrayElementAtIndex(rooms.arraySize - 1);
            weights.InsertArrayElementAtIndex(weights.arraySize - 1);
            normalizedWeights.InsertArrayElementAtIndex(normalizedWeights.arraySize - 1);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 20.0f;
        EditorGUILayout.LabelField("Room Asset");
        EditorGUILayout.LabelField("Weight");
        EditorGUILayout.LabelField("Calulated Chance");
        EditorGUILayout.EndHorizontal();

        if(weights.arraySize == rooms.arraySize){
            for (int i = 0; i < weights.arraySize; i++){
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(rooms.GetArrayElementAtIndex(i), GUIContent.none);
                weights.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(weights.GetArrayElementAtIndex(i).intValue);
                
                float chance = Mathf.Round((normalizedWeights.GetArrayElementAtIndex(i).floatValue * 10000)) / 100;
                EditorGUILayout.LabelField(string.Format("{0} %", chance));
                
                //Dont draw delete button for first element.
                if(i > 0){
                    if(GUILayout.Button("-", GUILayout.MaxWidth(30.0f))){
                        if (rooms.GetArrayElementAtIndex(i).objectReferenceValue != null)
                            rooms.GetArrayElementAtIndex(i).objectReferenceValue = null;
                        rooms.DeleteArrayElementAtIndex(i);

                        weights.DeleteArrayElementAtIndex(i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif