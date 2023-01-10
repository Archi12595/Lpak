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
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using Connect;



namespace Lpak
{
    /// <summary>
    /// Логика взаимодействия для Upload.xaml
    /// </summary>
    public partial class Upload : Window
    {
        private SQLiteConnection conn = new SQLiteConnection("Data Source="+ Connect.Connect_bd.Conf[0] + "; Version="+ Connect.Connect_bd.Conf[1] + ";");
        private string INN;
        public Upload()
        {
            InitializeComponent();
            // Lpak.MainWindow.id_klient;
            if (Lpak.MainWindow.flag_zayv_and_klient == 0)
            {
                comboBox1.Visibility = Visibility.Hidden;
                textBox5.Visibility = Visibility.Hidden;
                DataSet dataSet = new DataSet();
                string sql = @"SELECT Наименование, ИНН, Сфера_деятельности, Примечание
                FROM Клиенты
                WHERE id=" + Lpak.MainWindow.id_klient + ";";

                conn.Open();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                dataAdapter.Fill(dataSet);
                textBox1.Text = dataSet.Tables[0].Rows[0][0].ToString();
                textBox2.Text = dataSet.Tables[0].Rows[0][1].ToString();
                INN = dataSet.Tables[0].Rows[0][1].ToString();
                textBox3.Text = dataSet.Tables[0].Rows[0][2].ToString();
                textBox4.Text = dataSet.Tables[0].Rows[0][3].ToString();
                conn.Close();
                textBox2.MaxLength = 12;
                textBox4.MaxLength = 99;
            }
            if(Lpak.MainWindow.flag_zayv_and_klient==1)
            {
                textBox4.Visibility = Visibility.Hidden;
                comboBox1.Visibility = Visibility.Visible;
                textBox5.Visibility = Visibility.Visible;
                DataSet dataSet = new DataSet();
                string sql = @"SELECT  Наименование_работ, ИНН, Описание_работ, Дата_заявки , Статус
                FROM Заявки
                WHERE id=" + Lpak.MainWindow.id_zayvki + ";";
                conn.Open();
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                dataAdapter.Fill(dataSet);
                textBox1.Text = dataSet.Tables[0].Rows[0][0].ToString();
                textBox2.Text = dataSet.Tables[0].Rows[0][1].ToString();
                textBox2.IsReadOnly= true;
                textBox3.Text = dataSet.Tables[0].Rows[0][2].ToString();
                textBox5.Text= Convert.ToDateTime(dataSet.Tables[0].Rows[0][3].ToString()).ToString("dd-MM-yyyy");
                comboBox1.Items.Add(dataSet.Tables[0].Rows[0][4].ToString());
                comboBox1.SelectedIndex= 0;
            }


        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
        private void textBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;

            }
        }

        private void comboBox1_DropDownOpened(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Новая");
            comboBox1.Items.Add("В работе");
            comboBox1.Items.Add("Выполнена");
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (Lpak.MainWindow.flag_zayv_and_klient == 0)
            {
                if (textBox1.Text == "" || textBox3.Text == "")
                { MessageBox.Show("Поля наименованеи и сфера деятельности должны быть заполнены"); }
                else
                {
                    if (textBox2.Text.Length < 10 || textBox2.Text.Length == 11)
                    {
                        MessageBox.Show("Некоректно введен ИНН");
                    }
                    else
                    {
                        DataSet dataSet = new DataSet();
                        string sql = @"SELECT id, ИНН
                        FROM Клиенты  
                        WHERE Клиенты.ИНН=" + textBox2.Text + ";";
                        conn.Open();
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        if (dataSet.Tables[0].Rows.Count == 0)
                        {
                            // Esli INN smenilsa
                            Connect.Connect_bd.SQLZAPROSY(@"UPDATE Клиенты 
                            SET Наименование='" + textBox1.Text + "',ИНН=" + textBox2.Text + ",Сфера_деятельности='" + textBox3.Text + "',Примечание='" + textBox4.Text + "' WHERE id=" + Lpak.MainWindow.id_klient + ";");
                            Connect.Connect_bd.SQLZAPROSY(@"UPDATE Заявки SET ИНН=" + textBox2.Text + " WHERE ИНН=" + INN + ";");
                            MessageBox.Show("ИНН клиента был изменен");
                        }
                        else
                        {
                            // Esli INN ostalsa takim 
                            if (int.Parse(dataSet.Tables[0].Rows[0][0].ToString()) == Lpak.MainWindow.id_klient)
                            {
                                Connect.Connect_bd.SQLZAPROSY(@"UPDATE Клиенты 
                             SET Наименование='" + textBox1.Text + "',ИНН=" + textBox2.Text + ",Сфера_деятельности='" + textBox3.Text + "',Примечание='" + textBox4.Text + "' WHERE id=" + Lpak.MainWindow.id_klient + ";");
                                // Connect.Connect_bd.SQLZAPROSY(@"UPDATE Заявки SET ИНН=" + textBox2.Text + " WHERE ИНН=" + dataSet.Tables[0].Rows[0][1].ToString() + ";");
                            }
                            MessageBox.Show("Изменения сохранены");
                        }

                    }
                }
            }
            if (Lpak.MainWindow.flag_zayv_and_klient == 1)
            {
                if (textBox1.Text == "" || textBox3.Text == "" || comboBox1.SelectedIndex < 0)
                { MessageBox.Show("Заполните все поля"); }
                else
                {
                 Lpak.MainWindow.INN_client = textBox2.Text;
                 Connect.Connect_bd.SQLZAPROSY(@"UPDATE Заявки 
                 SET Наименование_работ='" + textBox1.Text + "',ИНН=" + textBox2.Text + ",Описание_работ='" + textBox3.Text + "', Статус='" + comboBox1.Text + "' WHERE id=" + Lpak.MainWindow.id_zayvki + ";");

                 MessageBox.Show("Изменения сохранены");
                }

            }
        
        }

        
    }
}
