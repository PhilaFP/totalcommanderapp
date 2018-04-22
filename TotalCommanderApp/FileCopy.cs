using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalCommanderApp
{
    class FileCopy
    {
        string sourcePath;
        string destinationPath;
        Boolean isDirectory;
        Boolean shouldDelete;

        public FileCopy(string sourcePath, string destinationPath, Boolean isDirectory, Boolean shouldDelete)
        {
            this.sourcePath = sourcePath;
            this.destinationPath = destinationPath;
            this.isDirectory = isDirectory;
            this.shouldDelete = shouldDelete;
        }

        public string getSourcePath()
        {
            return this.sourcePath;
        }

        public string getDestinationPath()
        {
            return this.destinationPath;
        }

        public Boolean getIsDirectory()
        {
            return this.isDirectory;
        }

        public Boolean getShouldDelete()
        {
            return this.shouldDelete;
        }

    }
}
