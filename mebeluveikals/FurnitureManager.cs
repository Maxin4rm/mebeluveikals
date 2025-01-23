using System;
using Microsoft.Data.Sqlite;


namespace mebeluveikals
{
    public class FurnitureManager
    {
        private readonly string connectionString;

        public FurnitureManager(string connectionString)
        {
            this.connectionString = connectionString;

            CreateFurnitureTable();
        }


        public void DeleteFurnitureByName(string name)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = @"DELETE FROM Furniture WHERE Name = @name";
                deleteCmd.Parameters.AddWithValue("name", name);

                deleteCmd.ExecuteNonQuery();
            }
        }

        public Furniture ReadFurnitureByName(string name)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = @"SELECT * FROM Furniture WHERE Name = @name";
                selectCmd.Parameters.AddWithValue("name", name);

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Furniture(
                                reader["Name"].ToString()!,
                                reader["Description"].ToString()!,
                                Convert.ToDouble(reader["Price"]),
                                Convert.ToInt32(reader["Height"]),
                                Convert.ToInt32(reader["Width"]),
                                Convert.ToInt32(reader["Length"])
                            );
                    }
                }
            }
            throw new Exception("Furniture with such name not found");
        }

        public List<Furniture> ReadFurniture()
        {
            var furnitureList = new List<Furniture>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT * FROM Furniture";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var newFurniture = new Furniture(
                                reader["Name"].ToString()!,
                                reader["Description"].ToString()!,
                                Convert.ToDouble(reader["Price"]),
                                Convert.ToInt32(reader["Height"]),
                                Convert.ToInt32(reader["Width"]),
                                Convert.ToInt32(reader["Length"])
                            );
                        furnitureList.Add(newFurniture);
                    }
                }
            }
            return furnitureList;
        }

        public void AddFurniture(Furniture furniture)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var createRecordCommand = connection.CreateCommand();
                createRecordCommand.CommandText = @"INSERT INTO Furniture(Name, Description, Price, Height, Width, Length)
                VALUES (@name, @description, @price, @height, @width, @length)";

                createRecordCommand.Parameters.AddWithValue("name", furniture.Name);
                createRecordCommand.Parameters.AddWithValue("description", furniture.Description);
                createRecordCommand.Parameters.AddWithValue("price", furniture.Price);
                createRecordCommand.Parameters.AddWithValue("height", furniture.Height);
                createRecordCommand.Parameters.AddWithValue("width", furniture.Width);
                createRecordCommand.Parameters.AddWithValue("length", furniture.Length);

                createRecordCommand.ExecuteNonQuery();
            }
        }

        public void UpdateFurniture(Furniture furniture)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var updateRecordCommand = connection.CreateCommand();
                updateRecordCommand.CommandText = @"UPDATE Furniture 
                                                    SET Description = @description, 
                                                        Price = @price, 
                                                        Height = @height, 
                                                        Width = @width, 
                                                        Length = @length 
                                                    WHERE Name = @name";

                updateRecordCommand.Parameters.AddWithValue("name", furniture.Name);
                updateRecordCommand.Parameters.AddWithValue("description", furniture.Description);
                updateRecordCommand.Parameters.AddWithValue("price", furniture.Price);
                updateRecordCommand.Parameters.AddWithValue("height", furniture.Height);
                updateRecordCommand.Parameters.AddWithValue("width", furniture.Width);
                updateRecordCommand.Parameters.AddWithValue("length", furniture.Length);
             
                updateRecordCommand.ExecuteNonQuery();
            }
        }

        private void CreateFurnitureTable()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Furniture (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Description TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Height INTEGER NOT NULL,
                        Width INTEGER NOT NULL,
                        Length INTEGER NOT NULL   
                    ); 
                ";
                createTableCommand.ExecuteNonQuery();
            }
        }
    }
}
