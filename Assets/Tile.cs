using UnityEngine;
using System.Collections;


/**
 * container class for the grid tiles
 * 
 */
public class Tile : MonoBehaviour{

	//positions in the grid
	public int row;
	public int column;
	public GameController parent;
	//object in the tile
	public Unit occupant;
	//is the tile occupied
	public bool occupied;
	//reference to tile prefab/gameobject
	public GameObject tile;

	public bool active = true;

	public bool visited = false;//used when trying to find a path
	public Tile previous;//Tile that discovered this tile in the path
	public bool discovered = false;//used when trying to find a clsoest path to a tile that is blocked

	public ArrayList neighbors;//inefficient neighbors storage

	public Sprite notSelected;
	public Sprite mouseOver;

	//for detecting if you moved the mouse after right clicking
	private Vector3 startClick = new Vector3(1,1,1);
	private Vector3 clickRelease;

	//neighbor references
	public Tile left;
	public Tile right;
	public Tile topLeft;
	public Tile topRight;
	public Tile bottomLeft;
	public Tile bottomRight;

	public Tile(int x, int y){
		this.row = x;
		this.column = y;
		neighbors = new ArrayList();
	}

	/**
	 * build the neighbor list
	 */
	public void addNeighbor(Tile t){
		if(neighbors == null){
			neighbors = new ArrayList();
		}
		if(t != null)
			neighbors.Add(t);

	}


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	/**
	 * print a tile's neighbors what else scrub
	 */
	public string printNeighbors(){
		string str = "";
		Tile t;

		for (int i = 0; i < neighbors.Count; i++){
			t = (Tile) neighbors[i];
			str += t.row.ToString() + " " + t.column.ToString() + " ";
		}

		return str;
	}

	/**
	 * innefficently checks for right clicks
	 * a right click is considered pushing the right mouse button down, then releasing the right mouse button, with no mouse movement inbetween
	 */
	private void OnMouseOver(){
//		if(Input.GetMouseButtonUp(1) && !parent.camController.cameraMoving){
//			if(Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
//				print ("clicked");
//			else parent.canClick = false;
//		}
//		else if(Input.GetMouseButtonUp(1) && parent.canClick == false)
//			parent.canClick = true;


		if(Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift)){
			startClick = Input.mousePosition;
		}
		if(Input.mousePosition == startClick && Input.GetMouseButtonUp(1) && !Input.GetKey(KeyCode.LeftShift)){
			print("click");
			parent.rightClick(tile);
		}
		
	}

	private void OnMouseEnter(){
		GetComponent<SpriteRenderer>().sprite = mouseOver;

	}

	private void OnMouseExit(){
		GetComponent<SpriteRenderer>().sprite = notSelected;
	}

	public string ToString(){
		return row.ToString() + " " + column.ToString();
	}

	/**
	 * "efficiently" checks for left clicks
	 */
	private void OnMouseDown(){
		print ("tile " + tile.name + " clcked");
		parent.leftClick(tile);
	}

	/*
	 * set a unit as the occupant of a tile
	 * use null if a unit is leaving a tile
	 */
	public void setOccupant(Unit u){
		if( u == null){
			occupant = null;
			occupied = false;
			return;
		}
		occupant = u;
		occupied = true;
	}



}
