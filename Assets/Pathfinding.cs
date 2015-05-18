using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Pathfinding : MonoBehaviour {

	public GameController parent;
	public Tile start;
	public Tile end;

	private ArrayList visitedTiles;
	private ArrayList queue;
	public ArrayList path;

	// Use this for initialization
	void Start () {
		queue = new ArrayList();
		visitedTiles = new ArrayList();
		print ("pathfind starting");

	}

	/**
	 * initiate pathfinding
	 */ 
	public ArrayList pathFind(Tile start, Tile end){
		clearPath();

		if(queue == null)
			queue = new ArrayList();
		if(visitedTiles == null)
			visitedTiles = new ArrayList();

		visitedTiles.Add(start);

		this.start = start;
		this.end = end;
		start.previous = null;
		Tile tree = visit(null, start);

		if (tree == null)
			return null;


		return buildPath(tree);
	}

	/**
	 * mark the current tile as visited, check if it's the destination and return it
	 * otherwise queue up the current tile's neighbors and visit the next, or return null if there is no path
	 */
	private Tile visit(Tile previous, Tile current){
		if(current == end)
			return current;
		queueNeighbors(current);
		if(queue.Count == 0)
			return null;
		return visit(current, deQueue());
	}

	/**
	 * queue up all the unvisited neighbors of the current tile
	 */
	private void queueNeighbors(Tile t){
		for(int i = 0; i < t.neighbors.Count; i++){
			Tile j = (Tile) t.neighbors[i];
			if(!j.visited && !(j.occupied || j.active)){
				j.previous = t;
				j.visited = true;
				queue.Add(j);
				visitedTiles.Add(j);
			}
		}
	}

	/**
	 * remove first element from the queue
	 */
	private Tile deQueue(){
		Tile first = (Tile) queue[0];
		queue.RemoveAt(0);
		return first;
	}

	/**
	 * go down the tree of previous nodes to create a path from the start to ending node
	 */
	private ArrayList buildPath(Tile final){
		path = new ArrayList();
		path.Add(final);
		while(final != start){
			final = final.previous;
			path.Add(final);
		}
		path.Reverse();


		clearPath();
		return path;
	}

	/**
	 * removes visited flag and previous tile references from visited nodes
	 */
	public void clearPath(){
		Tile temp;
		for(int i = 0; i < visitedTiles.Count; i++){
			temp = (Tile) visitedTiles[i];
			temp.visited = false;
			temp.previous = null;
		}
		queue.Clear();
		visitedTiles.Clear();

	}
	// Update is called once per frame
	void Update () {
	
	}

	/**
	 * print the path for debugging purposes
	 */
	public string printPath(ArrayList path){
		string str = "Path:";
		Tile temp;
		for (int i = 0; i < path.Count;i++){
			temp = (Tile) path[i];
			str += " " + temp.ToString();
		}

		return str;
	}

	/**
	 * find the closest point to a tile that is not traversable
	 */
	public ArrayList findClosest(Tile target){

		//if there is no path, try to find a point within a reasonable distance that has a path to it
		//iterate through each neighbor to find a position that is close and pathable to
		this.clearPath();//clear path first
		Tile currentTile = target;
		target.discovered = true;
		ArrayList tiles = (ArrayList) target.neighbors.Clone();//add neighboring tiles to a queue to check them
		Tile testTile;
		ArrayList shortestPath = null;
		ArrayList checkedTiles = new ArrayList();
		checkedTiles.Add (target);
		ArrayList path = new ArrayList();

		
		while(checkedTiles.Count < parent.gridCols * parent.gridRows){//distance from target tile to check if pathable to
			for(int j = 0; j < tiles.Count; j++){//check each neighbor of a tile for a path
				testTile = (Tile) tiles[j];
				if(!testTile.discovered){//check if this is the first iteration
					testTile.discovered = true;
					checkedTiles.Add(testTile);
				}
				if(!testTile.occupied || testTile.occupant == parent.currentUnit){//if the tile has not already been checked, try to find a path, check if the current tile is containing the current unit
					this.clearPath();
					path = this.pathFind(parent.currentUnit.currentPosition, testTile);
					if (path == null){//there was not a path to this tile even though it isn't occupied
						//print ("the path from :" + currentPosition +" to: " + testTile + " is null");
					}
					if(path != null){//if there is a path, check if it is shorter than an already discovered one
						//print(testTile+" has a path? from "+ currentPosition);
						//print ("path count: " + path.Count);
						if(shortestPath != null){
							if(path.Count < shortestPath.Count){
								shortestPath = (ArrayList) path.Clone ();
								//print("path found: " + printPath(shortestPath));
							}
							//if(path.Count<shortestPath.Count || shortestPath == null){
							
						}
						else{
							//print ("shortestpath is null");
							shortestPath = path;
						}
					}
				}
			}
			//if you have found a path, you're done! clear discovered flags
			if(shortestPath != null){
				//print ("found path");
				clearDiscovered(checkedTiles);
				return shortestPath;
			}
			
			//if you haven't found a path after one level, add each neighbor of each checked tile to a queue
			ArrayList nextLevel = new ArrayList();
			for(int j = 0; j < tiles.Count; j++){
				Tile t = (Tile) tiles[j];
				for(int k = 0; k < t.neighbors.Count; k++){
					Tile confusing = (Tile) t.neighbors[k];
					if(!confusing.discovered){
						//make sure newly queued tiles are marked as discovered
						confusing.discovered = true;
						checkedTiles.Add(confusing);
						nextLevel.Add(confusing);
					}
				}
			}
			tiles = nextLevel;
		}
		//if there was not a path within the set distance just return
		if(path == null){
			clearDiscovered(checkedTiles);
			return null;
		}
		
		return null;
	}

	public void clearDiscovered(ArrayList checkedTiles){
		for(int j = 0; j < checkedTiles.Count; j++){
			Tile t = (Tile) checkedTiles[j];
			t.discovered = false;
		}
		
		checkedTiles = new ArrayList();
	}




}
