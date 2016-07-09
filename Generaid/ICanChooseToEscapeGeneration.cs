namespace Generaid
{
    /// <summary>Implement to escape generating file</summary>
    public interface ICanChooseToEscapeGeneration
    {
        /// <summary>Return to -true- if you wish the file to never be generated</summary>
        bool DoNotGenerate { get; }
    }
}