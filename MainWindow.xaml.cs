using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Server;

namespace LSCodeGenerator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void getDatabases(string serverName) {
            try
            {
                Server myServer = new Server(serverName);
                DatabaseCollection myDatabaseCollection = myServer.Databases;
                if (myDatabaseCollection.Count == 0)
                {
                    throw new System.Exception("El servidor no contiene bases de datos");
                }
                cbxDatabases.Items.Clear();
                foreach (Database database in myDatabaseCollection)
                {
                    cbxDatabases.Items.Add(database.Name);
                }
                MessageBox.Show("Conexión exitosa, se encontraron " + myDatabaseCollection.Count.ToString() + " bases de datos");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string strServerName = txtServerName.Text;
            getDatabases(strServerName);
        }

        private ListBoxItem GetItem(string text, string imagePath)
        {
            ListBoxItem item = new ListBoxItem();

            //  Create StackPanel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // Create Image
            Image image = new Image();
            image.Source = new BitmapImage
                (new Uri("pack://application:,,/Images/" + imagePath));
            image.Width = 16;
            image.Height = 16;
            // Label
            Label lbl = new Label();
            lbl.Content = text;


            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            // assign stack to header
            //item.Background = stack;
            return item;
        }

        private void CbxDatabases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                string strServerName = txtServerName.Text;
                Server myServer = new Server(strServerName);
                DatabaseCollection myDatabaseCollection = myServer.Databases;
                lbTables.Items.Clear();
                foreach (Database database in myDatabaseCollection)
                {
                    if (database.Name == cbxDatabases.SelectedValue.ToString())
                    {
                        foreach (Table table in database.Tables)
                        {
                            lbTables.Items.Add(table.Name);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void LbTables_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (lbTables.SelectedItem != null)
                {
                    String strTableName = lbTables.SelectedItem.ToString();
                    FileGenerator objFileGenerator = new FileGenerator();

                    string strServerName = txtServerName.Text;
                    Server myServer = new Server(strServerName);
                    DatabaseCollection myDatabaseCollection = myServer.Databases;
                    int idTable = 0;
                    int idDatabase = 0;
                    foreach (Database database in myDatabaseCollection)
                    {
                        if (database.Name == cbxDatabases.SelectedValue.ToString())
                        {
                            idDatabase = database.ID;
                            foreach (Table table in database.Tables)
                            {
                                if (table.Name == strTableName)
                                    idTable = table.ID;
                            }
                        }
                    }
                    Database myDatabase = myDatabaseCollection.ItemById(idDatabase);
                    Table myTable = myDatabase.Tables.ItemById(idTable);
                    objFileGenerator.CreateEntitesFile(@"C:\CodeGenerator\" + myTable.Name.Remove(0, 2) + @"\" + myTable.Name.Remove(0, 2) + ".cs", myTable.Columns, myTable.Name);
                    objFileGenerator.CreateSPFile(@"C:\CodeGenerator\" + myTable.Name.Remove(0, 2) + @"\sp" + myTable.Name.Remove(0,2) + "_Delete.sql", myTable.Columns, "Delete", myTable.Name);
                    objFileGenerator.CreateSPFile(@"C:\CodeGenerator\" + myTable.Name.Remove(0, 2) + @"\sp" + myTable.Name.Remove(0, 2) + "_Insert.sql", myTable.Columns, "Insert", myTable.Name);
                    objFileGenerator.CreateSPFile(@"C:\CodeGenerator\" + myTable.Name.Remove(0, 2) + @"\sp" + myTable.Name.Remove(0, 2) + "_Update.sql", myTable.Columns, "Update", myTable.Name);
                    objFileGenerator.CreateSPFile(@"C:\CodeGenerator\" + myTable.Name.Remove(0, 2) + @"\sp" + myTable.Name.Remove(0, 2) + "_SelectAll.sql", myTable.Columns, "SelectAll", myTable.Name);
                    objFileGenerator.CreateSPFile(@"C:\CodeGenerator\" + myTable.Name.Remove(0, 2) + @"\sp" + myTable.Name.Remove(0, 2) + "_SelectById.sql", myTable.Columns, "SelectById", myTable.Name);

                    MessageBox.Show("Archivo generado con exito en C:/CodeGenerator/" + myTable.Name.Remove(0, 2));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
