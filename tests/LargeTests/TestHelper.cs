using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LargeTests;

public static class TestHelper
{
    public static string ShortenForFilename(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

        //make sure the hash is only alpha numeric to prevent characters that may break the url
        return string.Concat(Convert.ToBase64String(hash).ToCharArray().Where(char.IsLetterOrDigit).Take(10));
    }
}