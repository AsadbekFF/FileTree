using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeConsoleApp
{
    
    class FileSystemVisitor
    {
        public delegate void FileTreeEventHandler(FileTreeEventArgs e);

        public delegate void StartAndEndEventHandler();

        public event StartAndEndEventHandler Start;

        public event StartAndEndEventHandler Finish;

        public event FileTreeEventHandler FileFound;

        public event FileTreeEventHandler DirectoryFound;

        public event FileTreeEventHandler FilteredDirectoryFound;

        public event FileTreeEventHandler FilteredFileFound;

        public readonly SortOption SortOption;

        public FileSystemVisitor(SortOption sortOption = SortOption.None) =>
            SortOption = sortOption;

        public IEnumerable<string> GetAllFilesAndDirectories(string path)
        {
            Start.Invoke();

            var directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                yield return string.Join("", GetDirectoryInGivenPath(directory));
            }

            var filesPaths = Directory.GetFiles(path);
            foreach (string file in filesPaths)
            {
                yield return GetFileInGivenPath(file);
            }

            Finish.Invoke();
        }

        public IEnumerable<string> GetFilteredFilesAndDirectories(string path)
        {
            Start.Invoke();

            var content = Directory.GetFiles(path).Concat(Directory.GetDirectories(path));

            if (SortOption == SortOption.Date)
            {
                Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();
                foreach (var item in content)
                    dates.Add(item, File.GetCreationTime(item));

                var sortedDates = from date in dates
                                  orderby date.Value ascending
                                  select date;

                foreach (var date in sortedDates)
                {
                    if (File.Exists(date.Key))
                        yield return GetFilteredDirectoryInGivenPath(date.Key);
                    else
                        yield return GetFilteredFileInGivenPath(date.Key);
                }
            }
            else
            {
                Array.Sort(content.ToArray());

                foreach (var data in content)
                {
                    if (File.Exists(data))
                        yield return GetFilteredDirectoryInGivenPath(data);
                    else
                        yield return GetFilteredFileInGivenPath(data);
                }
            }

            Finish.Invoke();
        }

        private string GetFilteredDirectoryInGivenPath(string path)
        {
            OnRaisingFilteredDirectoryFoundEvent(path);

            return path;
        }

        private void OnRaisingFilteredDirectoryFoundEvent(string path)
        {
            FileTreeEventArgs fileTreeEventArgs = new FileTreeEventArgs
            {
                Path = path,
                TimeOfCreation = Directory.GetCreationTime(path)
            };

            FilteredDirectoryFound(fileTreeEventArgs);
        }

        private string GetFilteredFileInGivenPath(string path)
        {
            OnRaisingFilteredFileFoundEvent(path);

            return path;
        }

        private void OnRaisingFilteredFileFoundEvent(string path)
        {
            FileTreeEventArgs fileTreeEventArgs = new FileTreeEventArgs
            {
                Path = path,
                TimeOfCreation = Directory.GetCreationTime(path)
            };

            FilteredFileFound.Invoke(fileTreeEventArgs);
        }

        private string GetFileInGivenPath(string path)
        {
            OnRaisingFileFoundEvent(path);

            return path;
        }

        private IEnumerable<string> GetDirectoryInGivenPath(string path)
        {
            OnRaisingDirectoryFoundEvent(path);

            var directories = Directory.GetDirectories(path);
            foreach (var item in directories)
            {
                string[] arrays;
                while ((arrays = Directory.GetDirectories(item)).Length > 0)
                {
                    foreach (var item2 in arrays)
                    {
                        yield return item2;
                    }
                }
            }
        }

        private string GetSubDirectoryInGivenPath(string item)
        {
            OnRaisingDirectoryFoundEvent(item);

            return item;
        }

        private void OnRaisingDirectoryFoundEvent(string path) 
        {
            FileTreeEventArgs fileTreeEventArgs = new FileTreeEventArgs()
            {
                Path = path,
                TimeOfCreation = Directory.GetCreationTime(path)
            };

            DirectoryFound.Invoke(fileTreeEventArgs);
        }

        private void OnRaisingFileFoundEvent(string path)
        {
            FileTreeEventArgs fileTreeEventArgs = new FileTreeEventArgs()
            {
                Path = path,
                TimeOfCreation = Directory.GetCreationTime(path)
            };

            FileFound.Invoke(fileTreeEventArgs);
        }
    }
}
