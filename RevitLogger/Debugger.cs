
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RevitLogger
{
    public partial class Debugger : Form
    {
        public Debugger()
        {
            InitializeComponent();

            // Закрытие окна
            button2.Click += (s, e) => Close();

            // Открытие лог-файла
            button1.Click += OpenLogFile;

            // Отображение пути к лог-файлу
            textBox2.ReadOnly = true;
            textBox2.Text = Logger.GetLogPath();
        }

        private void OpenLogFile(object sender, EventArgs e)
        {
            string logPath = Logger.GetLogPath();

            if (!File.Exists(logPath))
            {
                MessageBox.Show(
                    $"Лог-файл не найден:\n{logPath}",
                    "RevitLogger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = logPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось открыть лог-файл:\n{ex.Message}",
                    "RevitLogger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
