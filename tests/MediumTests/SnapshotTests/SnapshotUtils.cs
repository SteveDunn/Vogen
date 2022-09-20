namespace MediumTests.SnapshotTests
{
    /// <summary>
    /// We have tests targeting different versions of the framework and different locales.
    /// This method uses the major and minor version of the runtime version.
    /// </summary>
    public static class SnapshotUtils
    {
        public static string GetSnapshotDirectoryName(string locale = "")
        {
            var s = $"snapshots-{System.Environment.Version.Major}-{System.Environment.Version.Minor}";
        
            if (locale.Length > 0)
            {
                s += $"-{locale}";
            }

            return s;
        }
    }
}