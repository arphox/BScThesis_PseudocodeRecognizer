using System.IO;
namespace LexicalAnalysis
{
    internal static class FileHandler
    {
        internal static string ReadUTF8File(string path)
        {
            return File.ReadAllText(path);
        }
    }
}