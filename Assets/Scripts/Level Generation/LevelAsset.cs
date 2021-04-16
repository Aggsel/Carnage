using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Level", menuName = "Room Generation/Level", order = 1)]
public class LevelAsset : ScriptableObject
{    
    [Tooltip("What rooms this level can contain.")]
    [SerializeField] private RoomAsset[] rooms = new RoomAsset[1];
    [Tooltip("A random room from this list will be used as the initial/spawn room for this level.")]
    [SerializeField] private RoomAsset[] initialRoomPool = new RoomAsset[1];
    [Tooltip("A random room from this list will be used as the final room for this level.")]
    [SerializeField] private RoomAsset[] finalRoomPool = new RoomAsset[1];

    [SerializeField] private int[] weights = new int[1];
    [SerializeField] private float[] normalizedWeights = new float[1];
    [HideInInspector] [SerializeField] private int sumOfWeights;

    public RoomAsset GetRandomRoom(int doorMask = -1, RoomType type = RoomType.COMMON){
        List<RoomAsset> compatibleRooms = new List<RoomAsset>();

        switch(type){
            case RoomType.COMMON:
                for (int i = 0; i < rooms.Length; i++){
                    if(RoomAsset.CompatibleDoorMask(doorMask, rooms[i].GetDoorMask()))
                        compatibleRooms.Add(rooms[i]);
                }
                break;

            case RoomType.FINAL:
                for (int i = 0; i < finalRoomPool.Length; i++){
                    if(RoomAsset.CompatibleDoorMask(doorMask, finalRoomPool[i].GetDoorMask()))
                        compatibleRooms.Add(finalRoomPool[i]);
                }
                break;
            
            case RoomType.INITIAL:
                for (int i = 0; i < initialRoomPool.Length; i++){
                    if(RoomAsset.CompatibleDoorMask(doorMask, initialRoomPool[i].GetDoorMask()))
                        compatibleRooms.Add(initialRoomPool[i]);
                }
                break;
        }

        int randomIndex = Random.Range(0, compatibleRooms.Count);
        return compatibleRooms[randomIndex];
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
// [CustomEditor(typeof(LevelAsset))]
// [CanEditMultipleObjects]
// internal class LevelAssetEditor : Editor 
// {
//     SerializedProperty rooms = null;
//     SerializedProperty weights = null;
//     SerializedProperty normalizedWeights = null;
//     private bool defaultRoomExists = false;

//     void OnEnable(){
//         rooms = serializedObject.FindProperty("rooms");
//         weights = serializedObject.FindProperty("weights");
//         normalizedWeights = serializedObject.FindProperty("normalizedWeights");
//     }

//     public override void OnInspectorGUI(){
//         serializedObject.Update();

//         EditorGUILayout.BeginHorizontal();
//         if(GUILayout.Button("Add new room")){
//             rooms.InsertArrayElementAtIndex(rooms.arraySize - 1);
//             weights.InsertArrayElementAtIndex(weights.arraySize - 1);
//             normalizedWeights.InsertArrayElementAtIndex(normalizedWeights.arraySize - 1);
//         }

//         EditorGUILayout.EndHorizontal();

//         EditorGUILayout.Separator();

//         EditorGUILayout.BeginHorizontal();
//         EditorGUIUtility.labelWidth = 20.0f;
//         EditorGUILayout.LabelField("Room Asset");
//         EditorGUILayout.LabelField("Weight");
//         EditorGUILayout.LabelField("Calulated Chance");
//         EditorGUILayout.EndHorizontal();

//         if(weights.arraySize == rooms.arraySize){
//             for (int i = 0; i < weights.arraySize; i++){
                
//                 RoomAsset currentAsset = (RoomAsset)rooms.GetArrayElementAtIndex(i).objectReferenceValue; 
//                 if(currentAsset?.GetDoorMask() == 0b1111)
//                     defaultRoomExists = true;

//                 EditorGUILayout.BeginHorizontal();

//                 EditorGUILayout.ObjectField(rooms.GetArrayElementAtIndex(i), GUIContent.none);
//                 weights.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(weights.GetArrayElementAtIndex(i).intValue);
                
//                 float chance = Mathf.Round((normalizedWeights.GetArrayElementAtIndex(i).floatValue * 10000)) / 100;
//                 EditorGUILayout.LabelField(string.Format("{0} %", chance));
                
//                 //Dont draw delete button for first element.
//                 if(i > 0){
//                     if(GUILayout.Button("-", GUILayout.MaxWidth(30.0f))){
//                         if (rooms.GetArrayElementAtIndex(i).objectReferenceValue != null)
//                             rooms.GetArrayElementAtIndex(i).objectReferenceValue = null;
//                         rooms.DeleteArrayElementAtIndex(i);

//                         weights.DeleteArrayElementAtIndex(i);
//                     }
//                 }
//                 EditorGUILayout.EndHorizontal();
//             }
//         }
//         if(!defaultRoomExists){
//             GUIStyle textStyle = EditorStyles.label;
//             textStyle.wordWrap = true;
//             EditorGUILayout.LabelField("No room in this list can have doors on all sides.\nThis is not recommended! \nConsider adding at least one room that can have doors on all four sides.", textStyle);
//         }
//         serializedObject.ApplyModifiedProperties();
//     }
// }
#endif