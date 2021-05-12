using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MapDrawer : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites = new Sprite[0];
    [SerializeField] private Color unvisitedColor = Color.black;
    [SerializeField] private Color visitedColor = Color.white;
    [SerializeField] private Color currentColor = Color.red;
    [SerializeField] private GameObject player = null;

    private MazeCell[,] grid = new MazeCell[0,0];
    private Image[,] imageGrid = new Image[0,0];
    private Sprite sprite = null;
    private RectTransform rectTransform = null;
    private Vector2Int currentRoom = new Vector2Int(0,0);

    //Called by level manager when level generation is complete.
    public void SetGrid(MazeCell[,] grid, Vector2Int startingPos){
        this.grid = grid;
        this.imageGrid = new Image[grid.GetLength(0), grid.GetLength(1)];
        this.currentRoom = startingPos;
        InitializeUIElements();
    }

    public void UpdateRotation(){
        float rotation = player.transform.localRotation.eulerAngles.y;
        rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    private void InitializeUIElements(){
        if(player == null){
            player = FindObjectOfType<MovementController>().gameObject;
            Debug.LogWarning("Player reference was not set in MapDrawer, trying to fetch during runtime intead.", this);
        }
        rectTransform = GetComponent<RectTransform>();
        GridLayoutGroup layout = GetComponent<GridLayoutGroup>();
        layout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        layout.constraintCount = this.grid.GetLength(1);


        for (int y = this.grid.GetLength(1)-1; y >= 0; y--){
            for (int x = 0; x < this.grid.GetLength(0); x++){
                if(grid[x,y].visited){
                    imageGrid[x,y] = CreateNewRoom(x,y);
                    imageGrid[x,y].sprite = this.sprites[grid[x,y].doorMask];
                    imageGrid[x,y].color = unvisitedColor;
                }
                else{
                    Image newImage = CreateNewRoom(x,y);
                    newImage.color = new Color(0, 0, 0, 0);
                }
            }
        }

        SetRoomAsVisited(currentRoom);
    }

    // void Update(){
    //     float rotation = player.transform.localRotation.eulerAngles.y;
    //     rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
    // }

    private Image CreateNewRoom(int x, int y){
        GameObject newObject = new GameObject(string.Format("Room Sprite {0}x{1}",x,y));
        newObject.transform.SetParent(transform);
        Image newImage = newObject.AddComponent<Image>();
        newImage.rectTransform.localScale = new Vector3(1,1,1);
        return newImage;
    }

    public void SetRoomAsVisited(Vector2Int coord){
        imageGrid[coord.x,coord.y].color = visitedColor;
        CurrentRoom(coord);
    }

    private void CurrentRoom(Vector2Int coord){
        imageGrid[currentRoom.x, currentRoom.y].color = visitedColor;
        currentRoom = coord;
        imageGrid[currentRoom.x, currentRoom.y].color = currentColor;
    }
}
