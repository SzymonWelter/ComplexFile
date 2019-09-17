namespace ComplexFile.Core.Validation
{
    public interface IPathValidationMember
    {
        string Path { get;}
        bool Handle();
        void SetNext(IPathValidationMember next);
    }
}