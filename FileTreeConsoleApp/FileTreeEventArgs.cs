using System;

namespace FileTreeConsoleApp
{
    public class FileTreeEventArgs : EventArgs
    {
        public string Path { get; set; }
        public DateTime TimeOfCreation { get; set; }

    }
}