namespace Generaid
{
    /// <summary>Derive your T4 templates form this</summary>
    public interface ITransformer
    {
        /// <summary>Gives a namge to the generated file; add ".cs"</summary>
        string Name { get; }
        /// <summary>T4 should generate this automatically for you</summary>
        string TransformText();
    }

}