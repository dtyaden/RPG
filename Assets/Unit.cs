using UnityEngine;
using System.Collections;

/**
 * unit container class
 */ 
public class Unit : MonoBehaviour{

	//reference to the gameobject/prefab
	public GameObject unit;
	public GameController parent;
	public Tile currentPosition;
	public Tile currentTarget;
	public Tile previousTarget;
	private Action action;
	public bool projectile = false;
	public ArrayList path;

	public Unit(GameController parent){
		this.parent = parent;
	}

	public void setPath(ArrayList p){
		path = p;
	}

	public Unit(){

	}
	//what action the unit is doing
	enum Action {nothing, moving, skill};


	/**
	 * attempts to move this unit to a target tile
	 * if a unit is not a projectile it will find a path of tiles
	 * if a unit is a projectile it will just path in a straight line to a tile
	 */
	public void moveTo (Tile target){
		//print("move to :" + target.tile.name);
		if(projectile){
			moveAnimation(target);
			return;
		}

		//ignore previously clicked tile
		if(previousTarget != null)
			if(target == previousTarget)
				return;

		previousTarget = target;

		if(path == null)
			path = new ArrayList();

		path = parent.pathfinder.pathFind(currentPosition,target);


		if(path == null){

			path = parent.pathfinder.findClosest(target);
			if(path == null)
				return;

		}
		path.RemoveAt(0);//trim off starting tile
		print ("path in unit: " + printPath(path));
		if(path.Count < 1){
			print ("no path found");
			return;
		}
		currentTarget = (Tile) path[0];
		action = Action.moving;

		//set the tile to move to
		
	}




/**
	 * will move a unit towards a target tile's center
	 */
	public void moveAnimation(Tile target){

		//print ("unit transform" + unit.transform.position);
		Vector3 move = Vector3.MoveTowards(unit.transform.position, parent.getTileCenter(target),  parent.moveAnimationSpeed);
		//print ("move" + move);
		unit.transform.position = move;
		action = Action.moving;
		return;
	}

	public bool isMoving(){
		return action == Action.moving;
	}

	/**
	 * update animations for the unit.
	 * determine if the unit has reached its current tile destination
	 * update pathfinding for unit if necessary
	 */
	public void updateAnimation(){
	

		if(unit.transform.position != parent.getTileCenter(currentTarget)){
			moveAnimation(currentTarget);
			return;
		}
		else{

			if(projectile){
				action = Action.nothing;
				currentTarget = null;
				return;
			}
			//set occupancy for tiles
			currentPosition.occupied = false;
			currentPosition.occupant = null;
			currentPosition = currentTarget;
			currentPosition.occupied = true;
			currentPosition.occupant = this;

			//if you need to update the path, get the next in queue
			if(path.Count > 0){
				currentTarget = (Tile) path[0];
				path.RemoveAt(0);
			}
			//if the path is empty set the target to null;
			else if(path.Count == 0){
				currentTarget = null;
				action = Action.nothing;
				path.Clear();
			}
		}
	}


	public string printPath(ArrayList path){
		string str = "Path:";
		Tile temp;
		for (int i = 0; i < path.Count;i++){
			temp = (Tile) path[i];
			str += " " + temp.ToString();
		}
		
		return str;
	}

	void FixedUpdate(){

	}

	void Start(){
		action = Action.nothing;
	}
}
