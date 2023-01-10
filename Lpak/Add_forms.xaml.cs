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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Markup;
using Connect;



namespace Lpak
{

    /// <summary>
    /// Логика взаимодействия для Add_forms.xaml
    /// </summary>
    
    public partial class Add_forms : Window
    {
        private SQLiteConnection conn = new SQLiteConnection("Data Source=" + Connect.Connect_bd.Conf[0] +"; Version="+ Connect.Connect_bd.Conf[1] + ";");
        private List<string> inn = new List<string>();
        private int prov_inn;
        private int prov_name;
        private int prov_sfera;
        private int name_work;
        private int read_work;
        public Add_forms()
        {
            InitializeComponent();
            textBox1.IsEnabled = false;
            textBox1.MaxLength= 12;
            textBox3.MaxLength = 99;
            //textBox5.MaxLength = 10;
            textBox2.Visibility = Visibility.Hidden;
            textBox2.Visibility = Visibility.Hidden;
           // textBox5.Visibility = Visibility.Hidden;
            textBox6.Visibility = Visibility.Hidden;
            textBox7.Visibility = Visibility.Hidden;
            label.Visibility = Visibility.Hidden;
            label2.Visibility = Visibility.Hidden;
            textBox3.Visibility = Visibility.Hidden;
            textBox4.Visibility = Visibility.Hidden;
           // comboBox3.Visibility = Visibility.Hidden;
            datePicker.Visibility = Visibility.Hidden;
            comboBox2.IsEnabled = false;
            button1.IsEnabled = false;
        }
        
        
        // Tolko int
        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            prov_inn = 1;
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                
            }
        }
        // Proverka na vvod
        private void textBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            prov_name = 1;
        }

        private void textBox4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            prov_sfera = 1;
        }

    
        private void textBox6_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            name_work= 1;
        }

        private void textBox7_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            read_work= 1;
        }


        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            //MessageBox.Show("" + comboBox2.SelectedIndex);
            
            if (comboBox1.SelectedIndex == 0)
            {
                textBox1.IsEnabled = true;
                textBox2.Visibility = Visibility.Visible;
                textBox3.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Visible;
                label2.Visibility = Visibility.Hidden;
                textBox4.Visibility = Visibility.Visible;
               // textBox5.Visibility = Visibility.Hidden;
                textBox6.Visibility = Visibility.Hidden;
                textBox7.Visibility = Visibility.Hidden;
                comboBox2.IsEnabled = false;
                //  comboBox3.Visibility = Visibility.Hidden;
                datePicker.Visibility = Visibility.Hidden;
                button1.IsEnabled = true;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                textBox1.IsEnabled = false;
                textBox2.Visibility = Visibility.Hidden;
                textBox3.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Hidden;
                label2.Visibility = Visibility.Visible;
                textBox4.Visibility = Visibility.Hidden;
              //  textBox5.Visibility = Visibility.Visible;
                textBox6.Visibility = Visibility.Visible;
                textBox7.Visibility = Visibility.Visible;
                comboBox2.IsEnabled = true;
                //  comboBox3.Visibility = Visibility.Visible;
                datePicker.Visibility = Visibility.Visible;
                button1.IsEnabled = true;
                datePicker.Text = DateTime.Now.ToShortDateString();
            }
        }

        private void comboBox2_DropDownOpened(object sender, EventArgs e)
        {
           // SQLiteConnection conn = new SQLiteConnection("Data Source=Test.db; Version=3;");
            SQLiteCommand command = new SQLiteCommand(@"SELECT Наименование, ИНН
                FROM Клиенты  
                GROUP BY Наименование, ИНН", conn);
            comboBox2.Items.Clear();
            conn.Open();
            DbDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                
                comboBox2.Items.Add((string)reader["Наименование"]);
                //MessageBox.Show(reader["ИНН"].ToString());
                inn.Add(reader["ИНН"].ToString());
            }
            conn.Close();
            // MessageBox.Show("" + comboBox2.Items[2].ToString());
            
        }

        private void comboBox2_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {
                textBox1.Text = inn[comboBox2.SelectedIndex];
            }
            // textBox5.Text = DateTime.Now.ToString("yyyy-MM-dd");
            //datePicker.Text = DateTime.Now.ToShortDateString();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            if (comboBox1.SelectedIndex == 0)
            {
                if (prov_inn == 1 && prov_name == 1 && prov_sfera == 1)
                
                {
                   
                    if (textBox1.Text.Length < 10 || textBox1.Text.Length ==11)
                    {
                        MessageBox.Show("Некоректно введен ИНН");
                    }
                    else
                    {
                        string sql = @"SELECT ИНН
                        FROM Клиенты  
                        WHERE Клиенты.ИНН=" + textBox1.Text + ";";

                        DataSet dataSet = new DataSet();

                        conn.Open();
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, conn);
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        //MessageBox.Show("" + dataSet.Tables[0].Rows.Count);

                        //Prover na sovpodenie INN
                        if (dataSet.Tables[0].Rows.Count == 0)
                        {
                            
                            SQLiteCommand com = new SQLiteCommand();
                            conn.Open();
                            com.Connection = conn;
                            com.CommandText = @"INSERT INTO Клиенты (Наименование, ИНН, Сфера_деятельности, Примечание) VALUES('" + textBox2.Text + "'," + textBox1.Text + ",'" + textBox4.Text + "','" + textBox3.Text + "'); ";
                            com.ExecuteNonQuery();
                            conn.Close();
                            textBox1.Text = "Введите ИНН";
                            textBox2.Text = "Введите Наименование";
                            textBox4.Text = "Введите Сферу деятельности";
                            textBox3.Clear();
                            MessageBox.Show("Клиент добавлен");
                            // obnul
                            prov_inn = 0; prov_name = 0; prov_sfera = 0;
                        }
                        else
                        {
                            MessageBox.Show("Организация с таким ИНН уже существует");
                        }
                    }
                }
                else { MessageBox.Show("Заполните все поля");  }
            }
            if (comboBox1.SelectedIndex == 1)
            {
                if (comboBox2.SelectedIndex >= 0)
                {
                    if (name_work == 1 && read_work == 1)
                    {
                        
                        // pri dobavlrnii obnovit 
                        Lpak.MainWindow.INN_client = textBox1.Text;
                        //MessageBox.Show(datePicker.SelectedDate.Value.ToString("yyyy-MM-dd"));
                        SQLiteCommand com1 = new SQLiteCommand();
                        conn.Open();
                        com1.Connection = conn;
                        com1.CommandText = @"INSERT INTO Заявки ( ИНН, Дата_заявки, Наименование_работ, Описание_работ, Статус) VALUES(" + textBox1.Text + ",'" + datePicker.SelectedDate.Value.ToString("yyyy-MM-dd") + "','" + textBox6.Text + "','" + textBox7.Text + "','Новая'); ";
                        com1.ExecuteNonQuery();
                        conn.Close();
                        textBox6.Text = "Введите наименование работ";
                        textBox7.Text = "Введите описание работ";
                        MessageBox.Show("Заявка добавлена");
                        name_work = 0; read_work = 0;
                    }
                    else { MessageBox.Show("Заполните все поля"); }
                    
                }
                else { MessageBox.Show("Выберете название организации"); }
            
            
            }
            }

       
    }

    }

