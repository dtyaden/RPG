using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject tile;
	public GameObject parent;
	public float yShift;
	public float renderShift;
	public int gridRows;
	public int gridCols;
	private GameObject[,] grid;
	public float mapSize;

	//x and y of the bottom of the grid
	private float originX;
	private float originY;
	
	public GameObject[] characters;
	public GameObject unit;
	public float moveAnimationSpeed;
	public Tile unitTargetTile;
	public Unit currentUnit;
	private int x;
	private int y;
	public Pathfinding pathfinder;
	public ArrayList units;
	public Camera camera;
	public CameraController camController;
	public bool canClick = true;
	public UI ui;

	void Start () {

		ui = transform.FindChild("UI").gameObject.GetComponent<UI>();

		print ("Ui found? " + ui);

		camController = camera.GetComponent<CameraController>();

		units = new ArrayList();
		pathfinder = parent.AddComponent<Pathfinding>();
		pathfinder.parent = this;

		originX = getOriginX()+.5f;
		originY = getOriginY()+.5f;
		characters = new GameObject[1];
		grid  = new GameObject[gridRows,gridCols];

		buildGrid();

		//DEBUG
		//print ("spawn tile's center " + getTileCenter(grid[5,5]));
		//print (grid [5, 5].tile.GetComponent<BoxCollider2D> ().size);


		currentUnit = getUnit(instantiateUnit(getTile(grid[0,0])));
		currentUnit.currentPosition = getTile(grid[0,0]);

		unitTargetTile = getTile(grid [4, 3]);





		//print ("target tile: " + unitTargetTile.tile.transform.position);
		//print ("current unit transform - init " +currentUnit.transform.position);

		//print (unitTargetTile.tile.transform.position);

		//debug
		units.Add(spawn(grid[gridRows-1, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-2, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-3, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-4, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-5, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-6, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-7, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-8, gridCols-1], 0));
		units.Add(spawn(grid[gridRows-8, gridCols-2], 0));
		units.Add(spawn(grid[gridRows-7, gridCols-3], 0));
		units.Add(spawn(grid[1, gridCols-1], 0));

		parent.GetComponent<BoxCollider2D>().enabled = false;//temporary fix for detecting mouse input
	}

	/**
	 * build and instantiate the hex grid
	 * build tile neighbor references*/
	public void buildGrid(){
		for (int i = 0; i < gridRows; i++)
			for (int j = 0; j < gridCols; j++)
				grid[i,j] = instantiateTile(i,j);

		//builds neighbors and prints them
		for(int i = 0; i < gridRows; i++)
			for(int j = 0; j < gridCols; j++){
				Tile tile = getTile(grid[i,j]);
				buildNeighboringTiles(tile);
				tile.occupied = false;
		}
	}

	//return center of tile
	public Vector3 getTileCenter(Tile tile){
		//print ("get center of tile at position: " + tile.tile.transform.position);
		return tile.GetComponent<PolygonCollider2D>().bounds.center;//reference the tile inside of the tile container
	}

	//get x origin of background/map
	private float getOriginX(){
		BoxCollider2D bounds = parent.GetComponent<BoxCollider2D> ();
		return bounds.bounds.min.x;
	}

	//get y origin of background/map
	private float getOriginY(){
		BoxCollider2D bounds = parent.GetComponent<BoxCollider2D> ();
		
		return bounds.bounds.min.y;
	}

	/**
	 * returns a unit prefab spawned at a tile "t"
	 */
	public GameObject instantiateUnit(Tile t){
		Unit u;
		GameObject unit =(GameObject) Instantiate(this.unit, getTileCenter(t),Quaternion.identity);
		u = getUnit(unit);
		u.parent = this;
		u.unit = unit;
		u.currentPosition = t;
		t.setOccupant(u);
		return unit;
	}

	/**
	 * IMPORTANT: returns a reference to the script of a unit prefab/gameobject which actually does stuff
	 * We need to reference scripts this way because we have to access scripts from their container GameObject
	 */ 
	public Unit getUnit(GameObject unit){
		return unit.GetComponent<Unit>();
	}

	//instantiate a tile and returns it, use this instead of new keyword, same as instantiate tile
	public GameObject instantiateTile(int row, int column){
		GameObject tile;
		if (row == 0)
			tile = (GameObject) Instantiate (this.tile, new Vector3 (originX+column, originY+row, 0f), Quaternion.identity);
		else if (row % 2 == 0)
			tile = (GameObject) Instantiate ( this.tile, new Vector3 (originX + column, originY + row-yShift*row, 0f), Quaternion.identity);
		else 
			tile = (GameObject) Instantiate (this.tile, new Vector3 (originX + column+.5f, originY + row-yShift*row,0f), Quaternion.identity);
		tile.name = row.ToString()+" " + column.ToString();
		Tile t = getTile (tile);
		t.row = row;
		t.column = column;
		t.tile = tile;
		t.parent = this;
		return tile;
	}

	/**
	 * IMPORTANT: returns a reference to the script of a tile prefab/gameobject which actually does stuff
	 * We need to reference scripts this way because we have to access scripts from their container GameObject
	 */ 
	public Tile getTile(GameObject tile){
		return tile.GetComponent<Tile>();
	}

	public Tile getTopRightNeighbor(Tile t){
		if(t.row < gridRows-1 && t.row % 2 == 0){
			t.topRight = getTile(grid[t.row+1, t.column]);
			return getTile(grid[t.row+1, t.column]);
		}
		if(t.row % 2 == 1 && t.row < gridRows-1 && t.column < gridCols-1){
			t.topRight = getTile(grid[t.row+1, t.column+1]);
			return getTile(grid[t.row+1, t.column+1]);
		}
		return null;
	}

	public Tile getTopLeftNeighbor(Tile t){
		if(t.row % 2 == 1 && t.row < gridRows-1){
			t.topLeft = getTile(grid[t.row+1, t.column]);
			return getTile(grid[t.row+1, t.column]);
		}
		if(t.row % 2 == 0 && t.row < gridRows-1 && t.column > 0){
			t.topLeft = getTile(grid[t.row+1, t.column-1]);
			return getTile(grid[t.row+1, t.column-1]);
		}
		return null;
	}
	
	public Tile getLeftNeighbor(Tile t){
		if(t.column > 0){
			t.left = getTile(grid[t.row,t.column-1]);
			return getTile(grid[t.row,t.column-1]);
		}
		return null;
	}

	public Tile getRightNeighbor(Tile t){
		if(t.column < gridCols-1){
			t.right = getTile(grid[t.row,t.column+1]);
			return getTile(grid[t.row,t.column+1]);
		}
		return null;
	}

	public Tile getBottomLeftNeighbor(Tile t){
		if(t.row % 2 == 1){
			t.bottomLeft = getTile(grid[t.row-1, t.column]);
			return getTile(grid[t.row-1, t.column]);
		}
		if(t.row > 0 && t.column > 0){
			t.bottomLeft = getTile(grid[t.row-1,t.column-1]);
			return getTile(grid[t.row-1,t.column-1]);
		}
		return null;
	}

	public Tile getBottomRightNeighbor(Tile t){
		if(t.column < gridCols-1 && t.row % 2 == 1){
			t.bottomRight = getTile(grid[t.row-1, t.column+1]);
			return getTile(grid[t.row-1, t.column+1]);
		}
		if(t.row > 0 && t.row % 2 == 0){
			t.bottomRight = getTile(grid[t.row-1, t.column]);
			return getTile(grid[t.row-1, t.column]);
		}
		
		return null;
	}

	//build list of neighboring tiles
	public void buildNeighboringTiles(Tile t){
		Tile temp;

		temp = getTopLeftNeighbor(t);
		t.addNeighbor(temp);
		temp = getTopRightNeighbor(t);
		t.addNeighbor(temp);
		temp = getLeftNeighbor(t);
		t.addNeighbor(temp);
		temp = getRightNeighbor(t);
		t.addNeighbor(temp);
		temp = getBottomLeftNeighbor(t);
		t.addNeighbor(temp);
		temp = getBottomRightNeighbor(t);
		t.addNeighbor(temp);
	}

	/**
	 * spawn a unit at Tile t, of a certain type
	 */
	public GameObject spawn(GameObject tile, int type){
		
		Tile t = getTile (tile);
		GameObject newUnit = instantiateUnit(t);
		t.occupied = true;
		t.occupant = getUnit(newUnit);
		return newUnit;
	}

	public void rightClick(GameObject target){
		string type = target.tag;

		switch(type){
		case "Tile":
			ui.displayPopup(target);
			break;
		}
	}

	/**
	 * called when some gameobject is clicked
	 */
	public void leftClick(GameObject target){
		string type = target.tag;

		switch(type){
		case "Tile":
			Tile t = getTile (target);
			if(t.occupied){
				if(t.occupant == currentUnit)
					break;
			}

			currentUnit.moveTo(t);
			break;

		}
	}

	public float getDistance(Tile start, Tile end){
		return 0.0f;
	}

	// Update is called once per frame
	void FixedUpdate () {
		//print (currentUnit.transform.position);
		//print ("target tile position: " + unitTargetTile.transform.position);


		if(currentUnit.isMoving())
			currentUnit.updateAnimation();


	}
}