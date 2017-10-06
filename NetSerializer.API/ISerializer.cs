namespace NetSerializer.API
{
    public interface ISerializer<I>
    {
        I Serialize<O>(O input);
        
        O Deserialize<O>(I input);
    }
}