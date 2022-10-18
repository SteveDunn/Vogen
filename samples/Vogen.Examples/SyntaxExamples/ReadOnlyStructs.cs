namespace Vogen.Examples
{
    [ValueObject]
    public readonly partial struct MyReadOnlyVo
    {
    }

    public class UseReadOnly
    {
        public UseReadOnly()
        {
            MyReadOnlyVo.From(123);
        }
    }
}
