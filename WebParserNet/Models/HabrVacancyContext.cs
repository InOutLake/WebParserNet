using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace WebParserNet.Models
{
    public class HabrVacancyContext : DbContext
    {
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Company> Companies { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\Maxim\\source\\repos\\WebParserNet\\WebParserNet\\HabrVacancyes.db");
        }

        public void CheckDatabaseConnection()
        {
            var databaseName = Database.GetDbConnection().Database;

            Console.WriteLine($"Connected to database: {databaseName}");
        }
        public void GetTableNames()
        {
            var tableNames = new List<string>();

            using (var connection = new SqliteConnection(Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }
            }

            foreach (var table in tableNames)
            {
                Console.WriteLine(table);
            }
        }
    }
}
