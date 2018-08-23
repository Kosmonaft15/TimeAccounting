using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

namespace Учет_Времени
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();

            //счетчик запусков программы
            Properties.Settings.Default.total_runs++;
            Properties.Settings.Default.Save(); //сохранение переменных приложения
            label1.Text += Properties.Settings.Default.total_runs.ToString(); //вывод кол-ва запусков программы            

            ProcessList process = new ProcessList();
            process.GetProcessList(ref listView1); //получить список процессов

            process.SaveData(ref listView1); //сохранение в XML
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
    }

    public class ProcessList
    {        
        protected Process[] procs = Process.GetProcesses();

        public void GetProcessList(ref ListView listView)
        {
            foreach (Process p in procs) //получить список процессов и время
            {
                try
                {
                    ListViewItem row = new ListViewItem(p.ProcessName);
                    row.SubItems.Add(p.TotalProcessorTime.TotalMinutes.ToString());
                    row.SubItems.Add(p.StartTime.ToString(@"hh\:mm\:ss"));
                    row.SubItems.Add((DateTime.Now - p.StartTime).ToString(@"hh\:mm\:ss"));
                    listView.Items.Add(row);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void SaveData(ref ListView listView) //сохранение в XML
        {
            XmlDocument xmlData = new XmlDocument();
            XmlNode rootNode = xmlData.CreateElement("process");
            xmlData.AppendChild(rootNode);

            XmlNode procesNode = xmlData.CreateElement("proces");
            XmlAttribute attributeProcName = xmlData.CreateAttribute("name");
            XmlAttribute attributeProcTotalProcessorTime = xmlData.CreateAttribute("total_time");
            XmlAttribute attributeProcStartTime = xmlData.CreateAttribute("start_time");
            XmlAttribute attributeProcWorkTime = xmlData.CreateAttribute("work_time");

            attributeProcName.Value = listView.Items[0].SubItems[0].Text;
            attributeProcTotalProcessorTime.Value = listView.Items[0].SubItems[1].Text;
            attributeProcStartTime.Value = listView.Items[0].SubItems[2].Text;
            attributeProcWorkTime.Value = listView.Items[0].SubItems[3].Text;

            procesNode.Attributes.Append(attributeProcName);
            procesNode.Attributes.Append(attributeProcTotalProcessorTime);
            procesNode.Attributes.Append(attributeProcStartTime);
            procesNode.Attributes.Append(attributeProcWorkTime);

            rootNode.AppendChild(procesNode);
            xmlData.Save("process_data.xml");
        }
    }
}
