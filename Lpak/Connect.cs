using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;
using System.CodeDom;
using SQLitePCL;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Windows.Shapes;

namespace Connect
{
    
    public class Connect_bd
    {
        public static List<string> Conf = new List<string>();
        public static String dbFileName;
        public static void SQLZAPROSY(string zap)
        {
            SQLiteCommand com = new SQLiteCommand();
            SQLiteConnection conn= new SQLiteConnection("Data Source=" + Conf[0] + ";Version=" + Conf[1] +";");
            conn.Open();
            com.Connection= conn;
            com.CommandText= zap;
            com.ExecuteNonQuery();
            conn.Close();
        }
 
        public static void fail_conf()
        {
           
            if (!File.Exists("CONF.txt"))
            {
                StreamWriter f = new StreamWriter("CONF.txt", false);
                f.WriteLine("Test.db");
                f.WriteLine("3");
                f.Close();
            }
            if (File.Exists("CONF.txt"))
            {
                string[] lines = File.ReadAllLines("CONF.txt");
                foreach (string s in lines)
                {
                    Conf.Add(s);
                    //dbFileName = s;
                }
            }
            
        }

        public static void conect()
        {
            //String dbFileName;
            SQLiteConnection m_dbConn;
            SQLiteCommand m_sqlCmd;

            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();

            //dbFileName = "Test.db";

            if (!File.Exists(Conf[0]))
            {
                SQLiteConnection.CreateFile(Conf[0]);
                

                try
                {
                     m_dbConn = new SQLiteConnection("Data Source=" + Conf[0] + ";Version=" + Conf[1] +";");
                    // m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";");
                    //m_dbConn.SetPassword("password");
                    m_dbConn.Open();
                    m_sqlCmd.Connection = m_dbConn;

                   
                    m_sqlCmd.CommandText = @"CREATE TABLE [Клиенты] (
                    [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [Наименование] char(100) NOT NULL,
                    [ИНН] integer(12) NOT NULL UNIQUE,
                    [Сфера_деятельности] char(100) NOT NULL,
                    [Количество_заявок] int,
                    [Дата_последней_заявки] DATETIME (100),
                    [Примечание] char(100)
                    ); ";


                    m_sqlCmd.ExecuteNonQuery();
                    m_sqlCmd.CommandText = @"CREATE TABLE [Заявки] (
                     [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                     [ИНН] integer(12) NOT NULL,
                     [Дата_заявки] DATETIME NOT NULL,
                     [Наименование_работ] char(100) NOT NULL,
                     [Описание_работ] char(100) NOT NULL,
                     [Статус] char(100) NOT NULL);";
                     m_sqlCmd.ExecuteNonQuery();

                    MessageBox.Show("БД Создана");

                }
                catch (SQLiteException ex)
                {

                    MessageBox.Show("Error: " + ex.Message);
                }
                m_dbConn.Close();
            }
            SQLiteConnection shif = new SQLiteConnection("Data Source="+ Conf[0] + ";Version="+ Conf[1] + ";");
            var command = shif.CreateCommand();
            shif.Open();
            command.CommandText = "SELECT quote($newPassword);";
            command.Parameters.AddWithValue("$newPassword", "1q2w3e4r");
            var quotedNewPassword = (string)command.ExecuteScalar();

            command.CommandText = "PRAGMA rekey = " + quotedNewPassword;
            command.Parameters.Clear();
            command.ExecuteNonQuery();
            shif.Close();


        }

    }
   
}


