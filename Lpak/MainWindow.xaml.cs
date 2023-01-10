using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.SQLite;
using Connect;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Windows.Markup;
using System.Windows.Forms;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Common;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Collections.Generic;
using System.IO;

namespace Lpak
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static int id_klient;
        public static int id_zayvki;
        public static int flag_zayv_and_klient;
        public static string INN_client;
        static string INN_obn;
        public MainWindow()
        {
            InitializeComponent();
            textBox1.Text = "Выберете Клиента";
            comboBox1.Visibility = Visibility.Hidden;
            Connect_bd.fail_conf();
            Connect.Connect_bd.conect();
        }
        
        private void Obnovit_dt2(string INN)
        {
            
            DataSet dataSet1 = new DataSet();
            string sql1 = @"SELECT id, Дата_заявки, Наименование_работ, Описание_работ, Статус  FROM Заявки WHERE ИНН=" + INN + ";";
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
            conn.Open();
            SQLiteDataAdapter dataAdapter1 = new SQLiteDataAdapter(sql1, conn);
            dataAdapter1.Fill(dataSet1);
            data2.ItemsSource = dataSet1.Tables[0].DefaultView;
            conn.Close();
            data2.Columns[0].Visibility = Visibility.Hidden;
            data2.Columns[1].Header = "Дата заявки";
            data2.Columns[2].Header = "Наименование работ";
            data2.Columns[3].Header = "Описание работ";
            data2.Columns[4].Header = "Статус";
            data2.Columns[1].ClipboardContentBinding.StringFormat = "dd-MM-yyyy";
            
        }

        private void data1_Loaded(object sender, RoutedEventArgs e)
        {
            // При первой загрузки формы для отбражения комбо боксе организаций
            comboBox1.Items.Add("Выберете организацию");
            comboBox1.SelectedIndex=0;
        }

        private void Data2_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void data1_MouseUp(object sender, MouseButtonEventArgs e)
        {


            // Poluchenie indexa
            if (data1.SelectedIndex >= 0)
            {

                Connect.Connect_bd.SQLZAPROSY(@"UPDATE Клиенты 
                SET Количество_заявок =(
                SELECT   count(ИНН) AS Количество
                FROM Заявки
                WHERE Клиенты.ИНН=Заявки.ИНН
                GROUP BY ИНН);");

                // #1 sql zap
                DataRow dtr = ((DataRowView)(data1.SelectedValue)).Row;
                DataSet dataSet = new DataSet();
                INN_obn = dtr["ИНН"].ToString(); 
                string sql = @"SELECT id , Примечание FROM Клиенты WHERE ИНН=" + dtr["ИНН"] + ";";
                // SQLiteConnection conn = new SQLiteConnection("Data Source=Test.db; Version=3;");
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
                conn.Open();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                dataAdapter.Fill(dataSet);
                id_klient = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
                textBox1.Text = dataSet.Tables[0].Rows[0][1].ToString();
                conn.Close();

                // #2 sql zap
                DataSet dataSet1 = new DataSet();
                string sql1 = @"SELECT id, Дата_заявки, Наименование_работ, Описание_работ, Статус  FROM Заявки WHERE ИНН=" + dtr["ИНН"] + ";";
                conn.Open();
                SQLiteDataAdapter dataAdapter1 = new SQLiteDataAdapter(sql1, conn);
                dataAdapter1.Fill(dataSet1);
                data2.ItemsSource = dataSet1.Tables[0].DefaultView;
                conn.Close();
                data2.Columns[0].Visibility = Visibility.Hidden;
                data2.Columns[1].Header = "Дата заявки";
                data2.Columns[2].Header = "Наименование работ";
                data2.Columns[3].Header = "Описание работ";
                data2.Columns[4].Header = "Статус";
                data2.Columns[1].ClipboardContentBinding.StringFormat = "dd-MM-yyyy";


            }
            // DataRow tes= ((DataRowView)(data1.SelectedValue)).Row;
            // MessageBox.Show("" +dtr["id"]);
            else { System.Windows.Forms.MessageBox.Show("Выберете Клиента"); }


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Add_forms taskWindow = new Add_forms();
            //taskWindow.Show();
            Add_forms newForm = new Add_forms();
            newForm.ShowDialog();
            
        }

        private void GL_Windows_Activated(object sender, EventArgs e)
        {
           
            //Read data 
            Connect.Connect_bd.SQLZAPROSY(@"UPDATE Клиенты 
                SET Дата_последней_заявки =(
                SELECT  Дата_заявки
                FROM Заявки
                WHERE Заявки.ИНН=Клиенты.ИНН
                ORDER BY Дата_заявки DESC LIMIT 1
                )
                WHERE Клиенты.ИНН=Клиенты.ИНН");


            // Read count zayvki
            Connect.Connect_bd.SQLZAPROSY(@"UPDATE Клиенты 
                SET Количество_заявок =(
                SELECT   count(ИНН) AS Количество
                FROM Заявки
                WHERE Клиенты.ИНН=Заявки.ИНН
                GROUP BY ИНН);");
            //Vivod
            DataSet dataSet = new DataSet();
            string sql = @"SELECT Наименование, ИНН, Сфера_деятельности, Количество_заявок, Дата_последней_заявки FROM Клиенты ORDER BY Наименование";
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
            conn.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
            dataAdapter.Fill(dataSet);

            data1.ItemsSource = dataSet.Tables[0].DefaultView;
            conn.Close();
            data1.Columns[2].Header = "Сфера деятельности";
            data1.Columns[3].Header = "Количество заявок";
            data1.Columns[4].Header = "Дата последней заявки";
            data1.Columns[4].ClipboardContentBinding.StringFormat = "dd-MM-yyyy";
            if (INN_client!= null)
            { Obnovit_dt2(INN_client); }
        }

        private void data2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Dly izmeneniy zayvki
            if (data2.SelectedIndex >= 0)
            {
                DataRow dtr = ((DataRowView)(data2.SelectedValue)).Row;
                id_zayvki = int.Parse(dtr["id"].ToString());
                // MessageBox.Show(dtr["id"].ToString());
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            if (data1.SelectedIndex < 0 && data2.SelectedIndex <0 ) { MessageBox.Show("Выберете клиента или заявку"); }
            if (data1.SelectedIndex >= 0 && data2.SelectedIndex < 0)
            {
                flag_zayv_and_klient = 0;
                Upload Form = new Upload();
                Form.ShowDialog();
                DataSet dataSet = new DataSet();
                string sql = @"SELECT id , Примечание FROM Клиенты WHERE id=" + id_klient + ";";
                // SQLiteConnection conn = new SQLiteConnection("Data Source=Test.db; Version=3;");
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
                conn.Open();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                dataAdapter.Fill(dataSet);
                id_klient = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
                textBox1.Text = dataSet.Tables[0].Rows[0][1].ToString();
                conn.Close();

            }
            
            if (data2.SelectedIndex >= 0)
            {
               // string inn;
                //DataRow INN = ((DataRowView)(data1.SelectedValue)).Row;
               // inn = INN["ИНН"].ToString();
                flag_zayv_and_klient = 1;
                Upload Form = new Upload();
                Form.ShowDialog();
                // data2.ItemsSource = null;
                if (INN_client != null) { Obnovit_dt2(INN_client); }
                
            }
            

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            
            if (data1.SelectedIndex < 0 && data2.SelectedIndex < 0  ) { MessageBox.Show("Выберете клиента или заявку"); }
            
            if (data1.SelectedIndex >= 0 && data2.SelectedIndex < 0)
            {
                // Dly clienta
                string INN_klient;
                DataRow dtr = ((DataRowView)(data1.SelectedValue)).Row;
                INN_klient = dtr["ИНН"].ToString();
                DialogResult result = MessageBox.Show(
               "Удалить клиента?",
               "Сообщение",
               MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                   // DataRow dtr = ((DataRowView)(data1.SelectedValue)).Row;
                    INN_klient=dtr["ИНН"].ToString();
                    Connect.Connect_bd.SQLZAPROSY(@"DELETE FROM Заявки WHERE Заявки.ИНН="+INN_klient+";");
                    Connect.Connect_bd.SQLZAPROSY(@"DELETE FROM Клиенты WHERE Клиенты.ИНН=" + INN_klient + ";");
                    MessageBox.Show("Удалено");
                    
                }
            }

            // Dly zayvki
            if (data2.SelectedIndex >= 0)
            {
               // string INN_klient;
                int id_zayv;
               // DataRow INN = ((DataRowView)(data1.SelectedValue)).Row;
                DataRow dtr = ((DataRowView)(data2.SelectedValue)).Row;
                //INN_klient = INN["ИНН"].ToString();
                id_zayv = int.Parse(dtr["id"].ToString());
                DialogResult result = MessageBox.Show(
                "Удалить заявку?",
                "Сообщение",
                MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    Connect.Connect_bd.SQLZAPROSY(@"DELETE 
                    FROM Заявки
                    WHERE Заявки.id=" + id_zayv + ";");
                    System.Windows.Forms.MessageBox.Show("Удалено");
                    if (INN_obn != null) { Obnovit_dt2(INN_obn); }
                    
                }


            }

            
        }
        private void tabltem1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            button1.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
            button3.Visibility = Visibility.Visible;
            textBox1.Visibility = Visibility.Visible;
            comboBox1.Visibility = Visibility.Hidden;
            label1.Visibility = Visibility.Visible;
        }
        private void tabltem2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            button1.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            button3.Visibility = Visibility.Hidden;
            textBox1.Visibility = Visibility.Hidden;
            comboBox1.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Hidden;

            SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
            DataSet dataSet1 = new DataSet();
            string sql1 = @"SELECT ИНН, Дата_заявки, Наименование_работ, Описание_работ, Статус  FROM Заявки ORDER BY Дата_заявки DESC ;";
            conn.Open();
            SQLiteDataAdapter dataAdapter1 = new SQLiteDataAdapter(sql1, conn);
            dataAdapter1.Fill(dataSet1);
            data3.ItemsSource = dataSet1.Tables[0].DefaultView;
            conn.Close();
            //data2.Columns[0].Visibility = Visibility.Hidden;
            data3.Columns[0].Header = "ИНН";
            data3.Columns[1].Header = "Дата заявки";
            data3.Columns[2].Header = "Наименование работ";
            data3.Columns[3].Header = "Описание работ";
            data3.Columns[4].Header = "Статус";
            data3.Columns[1].ClipboardContentBinding.StringFormat = "dd-MM-yyyy";

        }

        private void comboBox1_DropDownOpened(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
            SQLiteCommand command = new SQLiteCommand(@"SELECT Наименование, ИНН
                FROM Клиенты  
                GROUP BY Наименование, ИНН", conn);
            comboBox1.Items.Clear();
            conn.Open();
            DbDataReader reader = command.ExecuteReader();
            comboBox1.Items.Clear();
            while (reader.Read())
            {
                comboBox1.Items.Add((string)reader["Наименование"]);
                //inn.Add(reader["ИНН"].ToString());
            }
            conn.Close();
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect_bd.Conf[0] + "; Version=" + Connect_bd.Conf[1] + ";");
                DataSet dataSet1 = new DataSet();
                string sql1 = @"SELECT К.Наименование, З.ИНН , З.Дата_заявки, З.Наименование_работ, З.Описание_работ ,З.Статус
                FROM Клиенты К INNER JOIN Заявки З
	            ON К.ИНН = З.ИНН
		        WHERE К.Наименование= '"+comboBox1.Text+"' ;";
                conn.Open();
                SQLiteDataAdapter dataAdapter1 = new SQLiteDataAdapter(sql1, conn);
                dataAdapter1.Fill(dataSet1);
                data3.ItemsSource = dataSet1.Tables[0].DefaultView;
                conn.Close();
                //data2.Columns[0].Visibility = Visibility.Hidden;
                data3.Columns[0].Header = "Наименование";
                data3.Columns[1].Header = "ИНН";
                data3.Columns[2].Header = "Дата заявки";
                data3.Columns[3].Header = "Наименование работ";
                data3.Columns[4].Header = "Описание работ";
                data3.Columns[5].Header = "Статус";
                data3.Columns[2].ClipboardContentBinding.StringFormat = "dd-MM-yyyy";

            }
            else { MessageBox.Show("Выберете организацию"); }

        }
        
    }
    
}
