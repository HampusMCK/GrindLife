using Pc;

namespace GrindLife;

public class Worlds
{
    public List<worldInfo> worldList = new();
}

public class worldInfo
{
    public string name = "";
    public Player character;
    public int seed;
    public string screen = "start";
}