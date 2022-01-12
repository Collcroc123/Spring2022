public static class NetOP
{
    public const int None = 0;
    public const int CreatePlayer = 1;
}

[System.Serializable]
public abstract class NetMessage
{
    public byte OP { set; get; }

    public NetMessage()
    {
        OP = NetOP.None;
    }
}