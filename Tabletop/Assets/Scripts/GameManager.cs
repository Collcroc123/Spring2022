using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public Vector2 nextSpawnPos;

    public void SpawnPlayer()
    {
        GameObject temp = Instantiate(playerPrefab, nextSpawnPos, Quaternion.identity);
    }
}
