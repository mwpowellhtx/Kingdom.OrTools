using System;
using System.IO;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    public class FileEventArgs : EventArgs
    {
        public FileInfo Info { get; }

        public string Text { get; }

        internal FileEventArgs(FileInfo info)
            : this(info, null)
        {
        }

        internal FileEventArgs(FileInfo info, string text)
        {
            Info = info;
            Text = text;
        }
    }
}
