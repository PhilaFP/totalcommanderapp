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
        private string path1;
        private string path2;

        public Form1()
        {
            InitializeComponent();
            InitializeComboboxes();
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

        private void DoCopy(string otherPath, Boolean shouldDelete)
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
                    fileAction.DirectoryCopy(source, destination);
                    if (shouldDelete) Directory.Delete(source, true);
                }
                else
                {
                    string filePath = currentListView.SelectedItems[i].Text + currentListView.SelectedItems[i].SubItems[1].Text;
                    source = currentPath + filePath;
                    destination = otherPath + filePath;
                    fileAction.FileCopy(source, destination);
                    if (shouldDelete) File.Delete(source);
                }
            }
        }

        private void copy_Click(object sender, EventArgs e)
        {
            String otherPath = currentListView.Equals(listView1) ? comboBox2.Text : comboBox1.Text;
            ListView otherListView = currentListView.Equals(listView1) ? listView2 : listView1;

            DoCopy(otherPath, false);

            fileAction = new FileAction(comboBox1, comboBox2);
            fileAction.ShowFiles(otherListView, otherPath);
        }


        private void move_Click(object sender, EventArgs e)
        {
            String otherPath = currentListView.Equals(listView1) ? comboBox2.Text : comboBox1.Text;
            ListView otherListView = currentListView.Equals(listView1) ? listView2 : listView1;

            DoCopy(otherPath, true);

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
                ChangeCulture(new CultureInfo("pl"));
            }
            else
            {
                ChangeCulture(new CultureInfo("en"));
            }
        }

    }
}
