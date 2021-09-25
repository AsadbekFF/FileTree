using System;

namespace FileTreeConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemVisitor systemVisitor = new FileSystemVisitor(SortOption.Date);

            var content = systemVisitor.GetAllFilesAndDirectories("E:\\Checking_Work");

            systemVisitor.FileFound += FileFound;
            systemVisitor.DirectoryFound += DirectoryFound;
            systemVisitor.Start += Start;
            systemVisitor.Finish += Finish;
            systemVisitor.FilteredFileFound += FilteredFileFound;
            systemVisitor.FilteredDirectoryFound += FilteredDirectoryFound;

            foreach (var item in content)
            {
                Console.WriteLine(item);
            }

            Console.ReadKey();
        }

        private static void DirectoryFound(FileTreeEventArgs e)
        {
            Console.WriteLine($"Directory \"{e.Path}\" has been found.");
        }

        private static void Start()
        {
            Console.WriteLine("Starting to search... \n");
        }

        private static void Finish()
        {
            Console.WriteLine("\nSearching has been finished.");
        }

        private static void FilteredFileFound(FileTreeEventArgs e)
        {
            Console.WriteLine($"File \"{e.Path}\" has been found. Date of creation: \"{e.TimeOfCreation}\".");
        }

        private static void FilteredDirectoryFound(FileTreeEventArgs e)
        {
            Console.WriteLine($"Directory \"{e.Path}\" has been found. Date of creation is \"{e.TimeOfCreation}\".");
        }

        private static void FileFound(FileTreeEventArgs e)
        {
            Console.WriteLine($"File \"{e.Path}\" has been found.");
        }
    }
}
