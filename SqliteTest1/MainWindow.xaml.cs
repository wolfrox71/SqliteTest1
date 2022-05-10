using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
namespace SqliteTest1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    class database
    {
        protected string _filename, _tablename;
        public database(string filename, string tablename)
        {
            string path = Environment.CurrentDirectory;
            _filename = $"{path}\\{filename}";
            _tablename = tablename;
        }
        public void setup(string[] tableheadings)
        {
            if (tableheadings.Length == 0)
            {
                throw new Exception("Cannot create a table with no table headings");
            }
            using (var connection = new SQLiteConnection($"Data Source={_filename}"))
            {
                string command_text = $"CREATE TABLE IF NOT EXISTS {_tablename} (";
                for (int heading_id = 0; heading_id < tableheadings.Length; heading_id++)
                {
                    var heading = tableheadings[heading_id];
                    
                    if (heading_id == 0)
                    {
                        command_text += $"{heading} int PRIMARY KEY";
                    }
                    else
                    {
                        command_text += $" {heading} characters(100)";
                    }
                    if (heading_id < tableheadings.Length - 1)
                    {
                        command_text += ",";
                    }
                }
                command_text = command_text.Substring(0,command_text.Length - 1);
                command_text += ")";
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = command_text;
                var returned = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void drop()
        {
            using (var connection = new SQLiteConnection($"Data Source={_filename}"))
            {
                string command_text = $"DROP TABLE {_tablename}";
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = command_text;
                var returned = command.ExecuteNonQuery();
            }
        }
        public void clear()
        {
            using (var connection = new SQLiteConnection($"Data Source={_filename}"))
            {
                string command_text = $"DELETE * FROM {_tablename}";
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = command_text;
                var returned = command.ExecuteNonQuery();
            }
        }
        public string[] read(int column_number)
        {
            List<string> usersnames = new List<string>();
            using (var connection = new SQLiteConnection($"Data Source={_filename}"))
            {
                string command_text = $"SELECT * FROM {_tablename}";
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = command_text;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usersnames.Add(reader.GetString(column_number));
                    }
                }
            }
            return usersnames.ToArray();
        }

        public void insert(string[] data)
        {
            using (var connection = new SQLiteConnection($"Data Source={_filename}"))
            {
                string command_text = $"INSERT INTO {_tablename} VALUES (";
                foreach (string dat in data)
                {
                    string to_write = dat.Replace('\"', '\'');
                    command_text += $"\"{to_write}\",";
                }
                command_text = command_text.Substring(0, command_text.Length - 1);
                command_text += ")";
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = command_text;
                var returned = command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            database db = new database("main.db.sqlite3", "users");
            db.drop();
            db.setup(new string[] { "userid", "username", "password" });
            db.insert(new string[] { "1", "jamie", "password1" });
            db.insert(new string[] { "2", "not jamie", "other password" });
            string to_output = "";
            foreach (string line in db.read(1))
            {
                to_output += $"{line}\n";
            }
            MessageBox.Show(to_output);
        }
    }
}
