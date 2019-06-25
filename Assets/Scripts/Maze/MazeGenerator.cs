using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour {
    public static MazeGenerator instance;

	public Transform CellPrefab;
    public Vector3 GridSize;
	public float Buffer = 1;

    [HideInInspector]
	public List<Transform> Set = new List<Transform>();
    [HideInInspector]
    public static List<Transform> CompletedSet = new List<Transform>();
    [HideInInspector]
    public List<Vector3> Directions = new List<Vector3>();

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Directions.Add (new Vector3 (0, 0, 1));
		Directions.Add (new Vector3 (1, 0, 0));
		Directions.Add (new Vector3 (0, 0, -1));
		Directions.Add (new Vector3 (-1, 0, 0));
	}

	void Start () {
		CreateGrid ();
		SetStart ((int)Random.Range (0, GridSize.x), (int)Random.Range (0, GridSize.z));
		InvokeRepeating("FindNext", 0, 0.001f);
	}

	Transform[,]GridArr;
	public void CreateGrid () {
		int x = (int)GridSize.x;
		int z = (int)GridSize.z;
		GridArr = new Transform[x, z];
		Transform newCell;

		for (int ix = 0; ix < x; ix++) {
			for (int iz = 0; iz < z; iz++) {
				newCell = Instantiate (CellPrefab, new Vector3 (ix, 0, iz) * Buffer, Quaternion.identity) as Transform;
				newCell.name = string.Format ("({0},0,{1})", ix, iz);
				newCell.parent = transform;
				newCell.GetComponent<Cell> ().Position = new Vector3 (ix, 0, iz);
				GridArr [ix, iz] = newCell;
			}
		}
	}

	void SetStart (int x, int z) {
		AddToSet (GridArr[x,z]);
	}

	// n is 'Next'
	void AddToSet (Transform n)	{
		Set.Insert (0, n);
		Cell nScript = n.GetComponent<Cell> ();
		nScript.IsOpened = true;
	}
	
	void FindNext () {
		if (Set.Count == 0) {
			CancelInvoke ("FindNext");
            GameManager.instance.SpawnElements();
			return;
		}
		
		Transform previous = Set [0];
		Cell pScript = Set [0].GetComponent<Cell> ();
		
		Transform next;
		Cell nScript;
		
		int prevX = (int)pScript.Position.x;
		int prevZ = (int)pScript.Position.z;
		
		int randSeed = Random.Range (0, 4);
		int counter = 0;
		
		int nextX;
		int nextZ;		
		
		do {
			do {
				Vector3 randDirection = Directions [randSeed];
				nextX = prevX + (int)randDirection.x;
				nextZ = prevZ + (int)randDirection.z;
				randSeed = (randSeed + 1) % 4;
				counter++;
				if (counter > 4) {
					AddToCompletedSet (previous);
					return;
				}
			} while (nextX < 0 || nextZ < 0 || nextX >= GridSize.x || nextZ >= GridSize.z);
			next = GridArr [nextX, nextZ];
			nScript = next.GetComponent <Cell> (); 
		} while (nScript.IsOpened);
		
		ClearWalls (previous, next);
		
		AddToSet (next);		
	}
	
	void AddToCompletedSet (Transform  toAdd) {
		Set.Remove (toAdd);
		CompletedSet.Add (toAdd);
	}

	// p is 'Previous'
	// n is 'Next'
	void ClearWalls (Transform p, Transform n) {
		RaycastHit[] hitInfo;
		hitInfo = Physics.RaycastAll (p.position + Vector3.up, n.position - p.position, Buffer);
		foreach (RaycastHit hit in hitInfo) {
			Destroy (hit.transform.gameObject);
		}
	}

	public void Generate (int sizeX, int sizeZ) {
		CompletedSet = null;
		GridSize.x = sizeX;
		GridSize.z = sizeZ;
		Start ();
	}

    public List<Transform> GetCells ()
    {
        return CompletedSet;
    }
}