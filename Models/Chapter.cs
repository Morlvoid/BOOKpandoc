namespace BOOKpandoc.Models
{
    public class Chapter
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public Chapter(string filePath)
        {
            FilePath = filePath;
            FileName = System.IO.Path.GetFileName(filePath);
        }
    }
}