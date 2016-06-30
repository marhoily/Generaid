namespace Generaid
{
    internal interface INodeOwner
    {
        int Level { get; }
        string GeneratedDirName { get; }
    }
}