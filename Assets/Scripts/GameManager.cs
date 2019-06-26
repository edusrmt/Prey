using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject playerPrefab;
    public GameObject roachPrefab;
    public GameObject crystalPrefab;

    GameObject player;
    GameObject roach;

    Text messenger;
    int crystalsCollected = 0;

    bool lastVisibility = false;
    bool isBusy = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        messenger = GameObject.Find("Messenger").GetComponent<Text>();
        messenger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnElements ()
    {
        // Spawn the player at a random position
        Transform playerCell = MazeGenerator.instance.GetCells()[Random.Range(0, MazeGenerator.instance.GetCells().Count)];
        Vector3 playerPosition = new Vector3(playerCell.position.x, 1.5f, playerCell.position.z);
        player = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        player.name = "Player";

        // Spawn seven star crystals at random positions
        List<Transform> crystals = new List<Transform>(7);
        while (crystals.Count < 7)
        {
            Transform crystalCell = MazeGenerator.instance.GetCells()[Random.Range(0, MazeGenerator.instance.GetCells().Count)];

            if (crystalCell != playerCell && !crystals.Contains(crystalCell))
                crystals.Add(crystalCell);
        }

        GameObject crystalsParent = new GameObject();
        crystalsParent.name = "Star Crystals";

        for (int i = 0; i < 7; i++)
        {
            Vector3 crystalPosition = new Vector3(crystals[i].position.x, 0.5f, crystals[i].position.z);
            GameObject crystal = Instantiate(crystalPrefab, crystalPosition, Quaternion.identity, crystalsParent.transform);
            crystal.name = "Crystal " + (i + 1);
        }
    }

    void SpawnRoach ()
    {
        // Update A* path to the current maze
        AstarPath.active.Scan();

        // Spawn roach
        Transform roachCell = MazeGenerator.instance.GetCells()[Random.Range(0, MazeGenerator.instance.GetCells().Count)];
        Vector3 roachPosition = new Vector3(roachCell.position.x, 1.5f, roachCell.position.z);

        // Make sure the roach won't spawn too close to the player
        while (Vector3.Distance(player.transform.position, roachPosition) < 70)
        {
            roachCell = MazeGenerator.instance.GetCells()[Random.Range(0, MazeGenerator.instance.GetCells().Count)];
            roachPosition = new Vector3(roachCell.position.x, 1.5f, roachCell.position.z);
        }

        roach = Instantiate(roachPrefab, roachPosition, Quaternion.identity);
        roach.name = "Roach";

        RoachAI ai = roach.GetComponent<RoachAI>();
        ai.target = player.transform;
    }

    public void VisibleCrystal (bool currentVisibility)
    {
        if (isBusy)
            return;

        if (lastVisibility != currentVisibility)
        {
            if (currentVisibility)
            {
                messenger.text = "Press E to Collect";
                messenger.enabled = true;
            } else
            {
                messenger.enabled = false;
            }
        }

        lastVisibility = currentVisibility;
    }

    public void CrystalCollected ()
    {
        crystalsCollected++;

        if (crystalsCollected == 1)
            SpawnRoach();

        StartCoroutine(ShowMessage("Collected " + crystalsCollected + "/7 Crystals", 2f));        
    }

    IEnumerator ShowMessage (string message, float delay)
    {
        isBusy = true;
        messenger.text = message;
        messenger.enabled = true;
        yield return new WaitForSeconds(delay);
        messenger.enabled = false;
        isBusy = false;
    }
}
