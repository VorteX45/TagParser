using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TagParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process commandLineProcess = new System.Diagnostics.Process();
            commandLineProcess.StartInfo.UseShellExecute = false;
            commandLineProcess.StartInfo.RedirectStandardOutput = true;
            commandLineProcess.StartInfo.WorkingDirectory = Application.StartupPath;
            StreamReader outputReader;
            List<string> tagList = new List<string>();
            const string TEMP_FOLDER_PATH = "repository";

            //клонируем во временную папку
            commandLineProcess.StartInfo.FileName = "git";
            commandLineProcess.StartInfo.Arguments = "clone " + textBoxAddress.Text + " " + TEMP_FOLDER_PATH;
            commandLineProcess.Start();
            commandLineProcess.WaitForExit();
            commandLineProcess.Close();

            //получаем список тегов
            commandLineProcess.StartInfo.WorkingDirectory = Application.StartupPath + "\\" + TEMP_FOLDER_PATH;

            commandLineProcess.StartInfo.FileName = "git";
            commandLineProcess.StartInfo.Arguments = "tag";
            commandLineProcess.Start();
            outputReader = commandLineProcess.StandardOutput;
            commandLineProcess.WaitForExit();
            commandLineProcess.Close();
            while (outputReader.Peek() != -1)
            {
                tagList.Add(outputReader.ReadLine());
                textBoxDebug.Text += (tagList.Last());
            }

            //получаем информацию по каждому тэгу
            //commandLineProcess.StartInfo.FileName = "git";
            //commandLineProcess.StartInfo.Arguments = "clone " + textBoxAddress.Text;
            //commandLineProcess.Start();
            //outputReader = commandLineProcess.StandardOutput;
            //commandLineProcess.WaitForExit();
            //commandLineProcess.Close();

            //временная папка больше не нужна
            foreach (string file in Directory.GetFiles(TEMP_FOLDER_PATH, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            Directory.Delete(TEMP_FOLDER_PATH, true);
        }
    }
}