using System;
using System.Data.SQLite;
using System.IO;
using SQLi.Helpers;

namespace SQLi
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbFilePath = "";
            if (args.Length > 0) dbFilePath = args.Join();
            else
            {
                Console.Write("Enter path of SQLite file (or the path to create one): ");
                dbFilePath = Console.ReadLine();
            }
            if (!File.Exists(dbFilePath))
            {
                Console.WriteLine("File doesn't exist. Create one?");
                Console.Write("y/n ");
                string yn = Console.ReadLine();
                if (yn.ToLower() == "y") SQLiteConnection.CreateFile(dbFilePath);
                else return;
            }
            SQLiteConnection conn = new SQLiteConnection($"Data Source={dbFilePath}; FailIfMissing=False");
            conn.Open();
            while (true)
            {
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (SQLiteCommand comm = new SQLiteCommand("", conn, trans))
                    {
                        Console.Write($"SQLi (\"{dbFilePath}\")>");
                        string input = Console.ReadLine();
                        if (input.ToLower() == "exit") break;
                        comm.CommandText = input;
                        comm.Parameters.Add(new SQLiteParameter("test", "test"));
                        try
                        {
                            using (var reader = comm.ExecuteReader())
                            {
                                string recordsAffected = reader.RecordsAffected == -1 ? "No" : reader.RecordsAffected.ToString();
                                Console.WriteLine($"{recordsAffected} records affected.");
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write($"{reader.GetName(i)} ({reader.GetFieldType(i).Name})");
                                    if (i < reader.FieldCount - 1) Console.Write(" - ");
                                }
                                if (reader.HasRows)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine();
                                }
                                while (reader.HasRows)
                                {
                                    if (!reader.Read()) break;
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        Console.Write(reader.GetValue(i));
                                        if (i < reader.FieldCount - 1) Console.Write(" - ");
                                    }
                                    Console.WriteLine();
                                }
                            }
                        }
                        catch (SQLiteException x)
                        {
                            Console.WriteLine(x.Message);
                        }
                    }
                    trans.Commit();
                    Console.WriteLine();
                }
            }
            conn.Close();
        }
    }
}
