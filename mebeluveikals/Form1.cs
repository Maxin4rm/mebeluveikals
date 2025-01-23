using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace mebeluveikals
{
    public partial class Form1 : Form
    {
        private FurnitureManager furnitureManager;

        public Form1()
        {
            InitializeComponent();

            furnitureManager = new FurnitureManager("Data Source=furniture.db");

            var furniture = furnitureManager.ReadFurniture();
            var furnitureNames = new List<string>();

            foreach (var f in furniture)
            {
                furnitureNames.Add(f.Name);
            }

            selectProductComboBox.DataSource = furnitureNames;
        }

        private void selectBtn_Click(object sender, EventArgs e)
        {
            var furniture = furnitureManager.ReadFurnitureByName(selectProductComboBox.Text);

            nameTextBox.Text = furniture.Name;
            descTextBox.Text = furniture.Description;
            priceTextBox.Text = furniture.Price.ToString();
            hTextBox.Text = furniture.Height.ToString();
            wTextBox.Text = furniture.Width.ToString();
            lTextBox.Text = furniture.Length.ToString();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(nameTextBox.Text))
                {
                    MessageBox.Show("Nav norādīts nosaukums.");
                }
                else if (string.IsNullOrEmpty(descTextBox.Text))
                {
                    MessageBox.Show("Nav norādīts apraksts.");
                }
                else if (string.IsNullOrEmpty(priceTextBox.Text))
                {
                    MessageBox.Show("Nav norādīta cena.");
                }
                else if (string.IsNullOrEmpty(hTextBox.Text))
                {
                    MessageBox.Show("Nav norādīts augstums.");
                }
                else if (string.IsNullOrEmpty(wTextBox.Text))
                {
                    MessageBox.Show("Nav norādīts platums.");
                }
                else if (string.IsNullOrEmpty(lTextBox.Text))
                {
                    MessageBox.Show("Nav norādīts garums.");
                }

                furnitureManager.AddFurniture(new Furniture(
                    nameTextBox.Text, descTextBox.Text,
                    Convert.ToDouble(priceTextBox.Text),
                    Convert.ToInt32(hTextBox.Text),
                    Convert.ToInt32(wTextBox.Text),
                    Convert.ToInt32(lTextBox.Text))
                );

                List<string>? furnitureList = selectProductComboBox.DataSource as List<string>;
                furnitureList?.Add(nameTextBox.Text);

                selectProductComboBox.DataSource = null;
                selectProductComboBox.DataSource = furnitureList;

                MessageBox.Show("Ieraksts tika pievienots datubāzei");
            }
            catch (SqliteException)
            {
                MessageBox.Show("Notikusi SQL kļūda.");
            }
            catch (Exception)
            {
                MessageBox.Show("Notikusi kļūda.");
            }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            furnitureManager.DeleteFurnitureByName(selectProductComboBox.Text);

            List<string>? furnitureList = selectProductComboBox.DataSource as List<string>;
            furnitureList?.Remove(selectProductComboBox.Text);

            selectProductComboBox.DataSource = null;
            selectProductComboBox.DataSource = furnitureList;

            MessageBox.Show("Mēbele tika izdzēsta no datubāzes.");
        }

        private void importCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Select CSV file for import";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    ImportFromCsv(filePath);
                }
            }
        }

        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Select CSV file for export";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    ExportToCsv(filePath);
                }
            }
        }

        public void ExportToCsv(string filePath)
        {
            var furnitureList = furnitureManager.ReadFurniture();
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Name,Description,Price,Height,Width,Length");
                foreach (var furniture in furnitureList)
                {
                    writer.WriteLine(furniture.ToString());
                }
            }
        }

        public void ImportFromCsv(string filePath)
        {
            List<string> furnitureList = (List<string>)selectProductComboBox.DataSource!;

            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line!.Split(',');
                  
                    try
                    {
                        var value = new Furniture(
                            values[0],
                            values[1],
                            double.Parse(values[2]),
                            int.Parse(values[3]),
                            int.Parse(values[4]),
                            int.Parse(values[5])
                        );

                        if (!furnitureManager.ReadFurniture().Where(x => x.Name == values[0]).Any())
                        {
                            furnitureManager.AddFurniture(value);
                            furnitureList!.Add(value.Name);
                        }
                        else
                        {
                            furnitureManager.UpdateFurniture(value);
                        }
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Invalid file data");
                    }
                    catch (SqliteException)
                    {
                        MessageBox.Show("Error while importing data");
                    }
                }
            }
            selectProductComboBox.DataSource = null;
            selectProductComboBox.DataSource = furnitureList;
        }
    }
}
