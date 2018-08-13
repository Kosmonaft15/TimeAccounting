using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;

namespace Учет_Времени
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //счетчик запусков программы
            Properties.Settings.Default.total_runs++;
            Properties.Settings.Default.Save(); //сохранение переменных приложения
            label1.Text += Properties.Settings.Default.total_runs.ToString(); //вывод кол-ва запусков программы

            Process[] procs = Process.GetProcesses();
            foreach (Process p in procs) //получить список процессов и время
            {
                try
                {
                    ListViewItem row = new ListViewItem(p.ProcessName);
                    row.SubItems.Add(p.TotalProcessorTime.TotalMinutes.ToString());
                    row.SubItems.Add(p.StartTime.ToString(@"hh\:mm\:ss"));
                    row.SubItems.Add((DateTime.Now - p.StartTime).ToString(@"hh\:mm\:ss"));
                    listView1.Items.Add(row);                    
                }
                catch (Exception)
                {
                    continue;
                }
            }

            save_data(); ////сохранение в XML
        }

        private void button1_Click(object sender, EventArgs e) //кнопка Обновить
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            try //пересоздание listView1
            {
                Process[] procs = Process.GetProcesses();
                foreach (Process p in procs)
                {
                    try
                    {
                        ListViewItem row = new ListViewItem(p.ProcessName);
                        row.SubItems.Add(p.TotalProcessorTime.TotalMinutes.ToString());
                        row.SubItems.Add(p.StartTime.ToString(@"hh\:mm\:ss"));
                        row.SubItems.Add((DateTime.Now - p.StartTime).ToString(@"hh\:mm\:ss"));
                        listView1.Items.Add(row);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            finally
            {
                listView1.EndUpdate();
            }
        }

        private void save_data()//сохранение в XML
        {          
            XmlDocument xmlData = new XmlDocument();
            XmlNode rootNode = xmlData.CreateElement("process");
            xmlData.AppendChild(rootNode);

            XmlNode procesNode = xmlData.CreateElement("proces");
            XmlAttribute attributeProcName = xmlData.CreateAttribute("name");
            XmlAttribute attributeProcTotalProcessorTime = xmlData.CreateAttribute("total_time");
            XmlAttribute attributeProcStartTime = xmlData.CreateAttribute("start_time");
            XmlAttribute attributeProcWorkTime = xmlData.CreateAttribute("work_time");

            attributeProcName.Value = listView1.Items[0].SubItems[0].Text;
            attributeProcTotalProcessorTime.Value = listView1.Items[0].SubItems[1].Text;
            attributeProcStartTime.Value = listView1.Items[0].SubItems[2].Text;
            attributeProcWorkTime.Value = listView1.Items[0].SubItems[3].Text;

            procesNode.Attributes.Append(attributeProcName);
            procesNode.Attributes.Append(attributeProcTotalProcessorTime);
            procesNode.Attributes.Append(attributeProcStartTime);
            procesNode.Attributes.Append(attributeProcWorkTime);

            rootNode.AppendChild(procesNode);
            xmlData.Save("process_data.xml");
        }
    }
}
