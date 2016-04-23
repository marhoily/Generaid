namespace Generaid
{
    internal interface INodeOwner
    {
        int Level { get; }
        string ProjectDir { get; }
    }
}