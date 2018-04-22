using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Collections;

namespace TotalCommanderApp
{
    public partial class Form1 : Form
    {
        private FileAction fileAction;
        private ListView currentListView;
        private string currentPath;
        private string otherPath;
        private string path1;
        private string path2;
        private FileCopy fileCopy;
        Form2 form2;
        private int currentCopiedSize = 0;
        private int allFilesSize = 0;
        private ListViewColumnSorter lvwColumnSorter;
        private CultureInfo cultureInfo = new CultureInfo("pl");


        public Form1()
        {
            InitializeComponent();
            InitializeComboboxes();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
            this.listView2.ListViewItemSorter = lvwColumnSorter;
        }

        private void InitializeComboboxes()
        {
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                comboBox1.Items.Add(drive);
                comboBox2.Items.Add(drive);
            }
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf("C:\\");
            comboBox2.SelectedIndex = comboBox2.Items.IndexOf("C:\\");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            path1 = comboBox1.Text;
            try
            {
                fileAction = new FileAction(comboBox1, comboBox2);
                fileAction.ShowFiles(listView1, path1);
            }
            catch
            {
                MessageBox.Show("Błąd przy próbie dostępu do napędu", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf("C:\\");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            path2 = comboBox2.Text;
            try
            {
                fileAction = new FileAction(comboBox1, comboBox2);
                fileAction.ShowFiles(listView2, path2);
            }
            catch
            {
                MessageBox.Show("Błąd przy próbie dostępu do napędu", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox2.SelectedIndex = comboBox2.Items.IndexOf("C:\\");
            }
        }

        private void listView_Enter(object sender, EventArgs e)
        {
            currentListView = sender.Equals(listView1) ? listView1 : listView2;
            currentPath = sender.Equals(listView1) ? path1 : path2;

        }

        private void SetValueOnCurrentCombobox(ListView listView)
        {
            ComboBox currentCombobox = listView.Equals(listView1) ? comboBox1 : comboBox2;
            currentCombobox.Items.Add(currentPath);
            currentCombobox.SelectedIndex = currentCombobox.Items.IndexOf(currentPath);

        }


        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            fileAction = new FileAction(comboBox1, comboBox2);
            if (e.Button == MouseButtons.Left)
            {
                if (currentListView.SelectedItems[0].Text == "...")
                {
                    currentPath = currentPath.TrimEnd(new char[] { '\\' }); // usuniecie ostatniego znaku '\'
                    int last = currentPath.LastIndexOf('\\');
                    currentPath = currentPath.Substring(0, last + 1);
                    SetValueOnCurrentCombobox(currentListView);
                    fileAction.ShowFiles(currentListView, currentPath);
                }
                else if (currentListView.SelectedItems[0].SubItems[1].Text == "DIR")
                {
                    currentPath += (currentListView.SelectedItems[0].Text + "\\");
                    SetValueOnCurrentCombobox(currentListView);
                    fileAction.ShowFiles(currentListView, currentPath);
                }
                else
                {
                    MessageBox.Show("Nie można otworzyć pliku", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
                if (currentListView == listView1)
                    path1 = currentPath;
                else path2 = currentPath;
            }
        }

        private void listView1_ColumnClick(object sender,
                   System.Windows.Forms.ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
                lvwColumnSorter.Culture = cultureInfo; 
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        private void listView2_ColumnClick(object sender,
           System.Windows.Forms.ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
                lvwColumnSorter.Culture = cultureInfo;
            }

            // Perform the sort with these new sort options.
            this.listView2.Sort();
        }

        private void DoCopy(Boolean shouldDelete)
        {
            string destination, source;
            if (currentListView == null)
            {
                MessageBox.Show("Nie można wczytać odpowiedniego okna", "Błąd",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };
            for (int i = 0; i < currentListView.SelectedItems.Count; i++)
            {
                if (currentListView.SelectedItems[i].Text == "...")
                {
                    MessageBox.Show("Nie można otworzyć katalogu wyżej", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                if (currentListView.SelectedItems[i].SubItems[1].Text == "DIR")
                {
                    string dirPath = currentListView.SelectedItems[i].Text + "\\";
                    source = currentPath + dirPath;
                    destination = otherPath + dirPath;
                    fileCopy = new FileCopy(source, destination, true, shouldDelete);
                    backgroundWorker1.RunWorkerAsync(form2);
                    //if (shouldDelete) Directory.Delete(source, true);
                }
                else
                {
                    string filePath = currentListView.SelectedItems[i].Text + currentListView.SelectedItems[i].SubItems[1].Text;
                    source = currentPath + filePath;
                    destination = otherPath + filePath;
                    fileCopy = new FileCopy(source, destination, false, shouldDelete);
                    backgroundWorker1.RunWorkerAsync(form2);
                    //if (shouldDelete) File.Delete(source);
                }
            }
        }

        private void copy_Click(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();

            otherPath = currentListView.Equals(listView1) ? comboBox2.Text : comboBox1.Text;
            ListView otherListView = currentListView.Equals(listView1) ? listView2 : listView1;

            DoCopy(false);

            fileAction = new FileAction(comboBox1, comboBox2);
            fileAction.ShowFiles(otherListView, otherPath);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            if (fileCopy.getIsDirectory().Equals(true))
            {
                CountAllFilesSizes(fileCopy.getSourcePath(), fileCopy.getDestinationPath());
                DoCopyDirByteByByte(fileCopy.getSourcePath(), fileCopy.getDestinationPath());
                if (fileCopy.getShouldDelete()) Directory.Delete(fileCopy.getSourcePath(), true);

            }
            else
            {
                DoCopyFileByteByByte();
                if (fileCopy.getShouldDelete()) File.Delete(fileCopy.getSourcePath());

            }
            currentCopiedSize = 0;
            allFilesSize = 0;
        }

        private void DoCopyFileByteByByte()
        {
            FileInfo finInfo = new FileInfo(fileCopy.getSourcePath());

            FileStream fin = new FileStream(fileCopy.getSourcePath(), FileMode.Open);
            FileStream fout = new FileStream(fileCopy.getDestinationPath(), FileMode.Create);
            int copiedByte;
            
            int numberOfBytesAlreadyCopied = 0;
            int percentageDone = 0;
            do
            {
                copiedByte = fin.ReadByte();
                if (copiedByte != -1)
                {
                    fout.WriteByte((byte)copiedByte);

                    numberOfBytesAlreadyCopied++;
                    percentageDone = 100 * numberOfBytesAlreadyCopied / (int)finInfo.Length;
                    backgroundWorker1.ReportProgress(percentageDone);
                    Thread.Sleep(20);
                }
            } while (copiedByte != -1);

            fin.Close();
            fout.Close();
        }

       private void CountAllFilesSizes(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                allFilesSize += (int)file.Length;
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CountAllFilesSizes(subdir.FullName, temppath);
            }

        }

        public void DoCopyDirByteByByte(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

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

                FileStream fin = new FileStream(file.FullName, FileMode.Open);
                FileStream fout = new FileStream(temppath, FileMode.Create);

                int copiedByte;

                int percentageDone = 0;
                do
                {
                    copiedByte = fin.ReadByte();
                    if (copiedByte != -1)
                    {
                        fout.WriteByte((byte)copiedByte);

                        currentCopiedSize++;
                        percentageDone = 100 * currentCopiedSize / allFilesSize;
                        backgroundWorker1.ReportProgress(percentageDone);
                        Thread.Sleep(20);
                    }
                } while (copiedByte != -1);

                fin.Close();
                fout.Close();
            }

            // If copying subdirectories, copy them and their contents to new location.

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DoCopyDirByteByByte(subdir.FullName, temppath);
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            form2.SetProgressBarValue(e.ProgressPercentage);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Kopiowanie zakończone", "Sukces",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            fileAction = new FileAction(comboBox1, comboBox2);
            fileAction.ShowFiles(listView1, path1);
            fileAction.ShowFiles(listView2, path2);
        }


        private void move_Click(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();

            otherPath = currentListView.Equals(listView1) ? comboBox2.Text : comboBox1.Text;
            ListView otherListView = currentListView.Equals(listView1) ? listView2 : listView1;

            DoCopy(true);

            fileAction = new FileAction(comboBox1, comboBox2);
            fileAction.ShowFiles(listView1, path1);
            fileAction.ShowFiles(listView2, path2);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            string deleteObject;

            if (currentListView == null)
            {
                MessageBox.Show("Nie można wczytać odpowiedniego okna", "Błąd",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };
            if (currentListView.SelectedItems[0].Text == "...")
            {
                MessageBox.Show("Nie można otworzyć katalogu wyżej", "Błąd",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult odp = MessageBox.Show("Czy jesteś pewien?", "Usuń", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (odp != DialogResult.Yes)
            {
                return;
            }

            for (int i = 0; i < currentListView.SelectedItems.Count; i++)
            {
                if (currentListView.SelectedItems[i].SubItems[1].Text == "DIR")
                {
                    deleteObject = currentPath + currentListView.SelectedItems[i].Text;
                    Directory.Delete(deleteObject, true);
                }
                else
                {
                    string filePath = currentListView.SelectedItems[i].Text + currentListView.SelectedItems[i].SubItems[1].Text;

                    deleteObject = currentPath + filePath;
                    File.Delete(deleteObject);
                }
            }
            fileAction = new FileAction(comboBox1, comboBox2);
            fileAction.ShowFiles(currentListView, currentPath);
        }


        private void UpdateToolStripItemsCulture(ToolStripItem item, ComponentResourceManager resourceProvider,
            CultureInfo culture)
        {
            resourceProvider.ApplyResources(item, item.Name, culture);

            if (item is ToolStripMenuItem)
            {
                foreach (ToolStripItem it in ((ToolStripMenuItem)item).DropDownItems)
                {
                    UpdateToolStripItemsCulture(it, resourceProvider, culture);
                }
            }
        }

        private void UpdateColumnNamesCulture(ComponentResourceManager resourceProvider, CultureInfo culture)
        {
            resourceProvider.ApplyResources(NameListView1, "NameListView1", culture);
            resourceProvider.ApplyResources(NameListView2, "NameListView2", culture);
            resourceProvider.ApplyResources(SizeListView1, "SizeListView1", culture);
            resourceProvider.ApplyResources(SizeListView2, "SizeListView2", culture);
            resourceProvider.ApplyResources(DateListView1, "DateListView1", culture);
            resourceProvider.ApplyResources(DateListView2, "DateListView2", culture);
            resourceProvider.ApplyResources(ExtensionListView1, "ExtensionListView1", culture);
            resourceProvider.ApplyResources(ExtensionListView2, "ExtensionListView2", culture);
        }


        private void UpdateControlsCulture(Control control, ComponentResourceManager resourceProvider,
            CultureInfo culture)
        {
            control.SuspendLayout();
            resourceProvider.ApplyResources(control, control.Name, culture);

            foreach (Control ctrl in control.Controls)
            {
                UpdateControlsCulture(ctrl, resourceProvider, culture);
            }

            PropertyInfo property = control.GetType().GetProperty("Items");
            if (property != null)
            {
                foreach (ToolStripItem item in menuStrip1.Items)
                {
                    UpdateToolStripItemsCulture(item, resourceProvider, culture);
                }
            }

            UpdateColumnNamesCulture(resourceProvider, culture);

            fileAction = new FileAction(comboBox1, comboBox2);
            fileAction.SetCulture(culture);
            fileAction.ShowFiles(listView1, path1);
            fileAction.ShowFiles(listView2, path2);

            control.ResumeLayout(false);
        }


        private void ChangeCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));

            UpdateControlsCulture(this, resources, culture);
            resources.ApplyResources(this, "$this", culture);


            if (culture.Name == "pl")
            {
                polishToolStripMenuItem.Checked = true;
                englishToolStripMenuItem.Checked = false;
            }
            else
            {
                englishToolStripMenuItem.Checked = true;
                polishToolStripMenuItem.Checked = false;
            }
        }

        private void changeLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == polishToolStripMenuItem)
            {
                cultureInfo = new CultureInfo("pl");
                ChangeCulture(cultureInfo);
            }
            else
            {
                cultureInfo = new CultureInfo("en");
                ChangeCulture(cultureInfo);
            }
        }

    }
}
