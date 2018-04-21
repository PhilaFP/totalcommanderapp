using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace TotalCommanderApp
{
    class FileAction
    {
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private CultureInfo culture;

        public FileAction(ComboBox comboBox1, ComboBox comboBox2)
        {
            this.comboBox1 = comboBox1;
            this.comboBox2 = comboBox2;
        }

        public void SetCulture(CultureInfo culture)
        {
            this.culture = culture;
        }

        private ListView ShowDirectoriesFromPath(ListView listView, string currentDirectory)
        {
            string[] fileList = Directory.GetDirectories(currentDirectory);
            int i;
            for (i = 0; i < fileList.Length; i++)
            {
                DirectoryInfo directory = new DirectoryInfo(fileList[i]);
                string[] directoryInfo = new string[] {
                    directory.Name,
                    "DIR",
                    directory.LastWriteTime.ToString("d", culture) + "    " + directory.LastWriteTime.ToString("t", culture),
                    "---" };
                ListViewItem listItem = new ListViewItem(directoryInfo);
                listView.Items.Add(listItem);
            }
            return listView;
        }

        private string[] CreateFileInfo(FileInfo file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            string extension = Path.GetExtension(file.Name);
            string fileDate = file.LastWriteTime.ToString("d", culture) + "    " + file.LastWriteTime.ToString("t", culture);
            string fileSize = file.Length.ToString() + "b";

            return new string[] { fileName, extension, fileDate, fileSize };
        }

        private ListView ShowFilesFromPath(ListView listView, string currentDirectory)
        {
            string[] fileList = Directory.GetFiles(currentDirectory);
            int i;
            for (i = 0; i < fileList.Length; i++)
            {
                FileInfo file = new FileInfo(fileList[i]);
                ListViewItem listItem = new ListViewItem(CreateFileInfo(file));
                listView.Items.Add(listItem);
            }
            return listView;
        }

        public void ShowFiles(ListView listView, string currentDirectory)
        {
            listView.Items.Clear();

            if (comboBox1.Items.Contains(currentDirectory) || comboBox2.Items.Contains(currentDirectory))
            {
                listView.Items.Add(new ListViewItem("..."));
            }

            ShowDirectoriesFromPath(listView, currentDirectory);
            ShowFilesFromPath(listView, currentDirectory);


        }

        public void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }

        }

        public void FileCopy(string sourcePath, string destination)
        {
            FileInfo file = new FileInfo(sourcePath);
            file.CopyTo(destination, true);
        }

    }
}
