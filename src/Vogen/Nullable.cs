namespace Vogen;

public class Nullable
{
    public Nullable(bool nullableEnabled, bool wrapperIsAReference, bool underlyingIsAReference)
    {
        IsEnabled = nullableEnabled;
        if (!nullableEnabled)
        {
            return;
        }

        QuestionMarkForWrapper = wrapperIsAReference ? "?" : "";
        QuestionMarkForUnderlying = underlyingIsAReference ? "?" : "";
        
        QuestionMarkForOtherReferences = "?";
        
        BangForWrapper = wrapperIsAReference ? "!" : "";
        BangForUnderlying = underlyingIsAReference ? "!" : "";
    }

    public readonly string QuestionMarkForUnderlying = string.Empty;
    public readonly string QuestionMarkForWrapper = string.Empty;
    public readonly string QuestionMarkForOtherReferences = string.Empty;
    public readonly string BangForUnderlying = string.Empty;
    public readonly string BangForWrapper = string.Empty;

    public bool IsEnabled { get; }


    public string WrapBlock(string block) =>
        IsEnabled
            ? $"""
               #nullable enable
               {block}
               #nullable restore
               """
            : block;
}