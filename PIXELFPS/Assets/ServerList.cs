using Mirror.Discovery;
using UnityEngine;
using TMPro;

public class ServerList : MonoBehaviour
{
    public GameObject serverPrefab;

    public void OnServerFound(ServerResponse resp)
    {
        GameObject serverEntry = Instantiate(serverPrefab);
        serverEntry.GetComponentInChildren<TextMeshPro>().text = resp.serverId.ToString();
    }
}
