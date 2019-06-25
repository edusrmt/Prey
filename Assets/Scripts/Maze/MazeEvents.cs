using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeGenerator))]
public class MazeEvents : MonoBehaviour
{
    public GameObject playerPrefab;

    MazeGenerator mazeGenerator;

    void Start() {
        mazeGenerator = GetComponent<MazeGenerator>();
    }

    public void GenerationComplete ()
    {
        // Spawn the player at a random position
        Transform randomCell = mazeGenerator.CompletedSet[Random.Range(0, mazeGenerator.CompletedSet.Count)];
        Vector3 playerPosition = new Vector3(randomCell.position.x, 1.5f, randomCell.position.z);
        Instantiate(playerPrefab, playerPosition, Quaternion.identity);        
    }
}
