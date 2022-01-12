[System.Serializable]
public class Net_CreatePlayer : NetMessage
{
    public Net_CreatePlayer()
    {
        OP = NetOP.CreatePlayer;
    }
    
    public string Username { set; get; }
    public string Password { set; get; }
    public string Email { set; get; }
}