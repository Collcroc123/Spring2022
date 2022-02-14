using Mirror.Discovery;
using UnityEngine;

[CreateAssetMenu(menuName = "Datas/ServerData")]
public class ServerData : ScriptableObject
{
    public ServerResponse info;
    public string name;
    public string password;
    public int maxPlayers;
}
