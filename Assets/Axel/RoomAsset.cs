using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Room", menuName = "Room Generation/Room", order = 1)]
public class RoomAsset : ScriptableObject
{
    [SerializeField] private GameObject roomPrefab = null;
    [SerializeField] private int doorMask;

    public GameObject GetRoom(){
        return roomPrefab;
    }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(RoomAsset))]
[CanEditMultipleObjects]
public class RoomAssetEditor : Editor
{
    SerializedProperty doorMask = null;
    enum roomType {OneByOne};
    private bool[] doors = new bool[4];

    void OnEnable(){
        doorMask = serializedObject.FindProperty("doorMask");
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();

        for (int i = 0; i < doors.Length; i++){
            doors[i] = EditorGUILayout.Toggle(doors[i]);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif