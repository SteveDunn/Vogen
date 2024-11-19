namespace Vogen.Types;

internal class Filename
{
    public Filename(string filename)
    {
        OriginalFilename = filename;
        Value = Util.SanitizeToALegalFilename(filename);
    }

    public string Value { get; set; }

    public string OriginalFilename { get; }
    
    public static implicit operator string(Filename filename) => filename.Value;
    public override string ToString() => Value;
}