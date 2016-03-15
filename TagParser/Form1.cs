#region Лицензия MIT
//Copyright (c) 2016 Павел Орестов

//Данная лицензия разрешает лицам, получившим копию данного программного обеспечения и сопутствующей документации 
//(в дальнейшем именуемыми «Программное Обеспечение»), безвозмездно использовать Программное Обеспечение без ограничений, 
//включая неограниченное право на использование, копирование, изменение, слияние, публикацию, распространение, сублицензирование 
//и/или продажу копий Программного Обеспечения, а также лицам, которым предоставляется данное Программное Обеспечение, при соблюдении следующих условий:
//Указанное выше уведомление об авторском праве и данные условия должны быть включены во все копии или значимые части данного Программного Обеспечения.

//ДАННОЕ ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ ПРЕДОСТАВЛЯЕТСЯ «КАК ЕСТЬ», БЕЗ КАКИХ-ЛИБО ГАРАНТИЙ, ЯВНО ВЫРАЖЕННЫХ ИЛИ ПОДРАЗУМЕВАЕМЫХ, ВКЛЮЧАЯ ГАРАНТИИ 
//ТОВАРНОЙ ПРИГОДНОСТИ, СООТВЕТСТВИЯ ПО ЕГО КОНКРЕТНОМУ НАЗНАЧЕНИЮ И ОТСУТСТВИЯ НАРУШЕНИЙ, НО НЕ ОГРАНИЧИВАЯСЬ ИМИ. НИ В КАКОМ СЛУЧАЕ АВТОРЫ 
//ИЛИ ПРАВООБЛАДАТЕЛИ НЕ НЕСУТ ОТВЕТСТВЕННОСТИ ПО КАКИМ-ЛИБО ИСКАМ, ЗА УЩЕРБ ИЛИ ПО ИНЫМ ТРЕБОВАНИЯМ, В ТОМ ЧИСЛЕ, ПРИ ДЕЙСТВИИ КОНТРАКТА,
//ДЕЛИКТЕ ИЛИ ИНОЙ СИТУАЦИИ, ВОЗНИКШИМ ИЗ-ЗА ИСПОЛЬЗОВАНИЯ ПРОГРАММНОГО ОБЕСПЕЧЕНИЯ ИЛИ ИНЫХ ДЕЙСТВИЙ С ПРОГРАММНЫМ ОБЕСПЕЧЕНИЕМ.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;

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
            StreamReader outputReader;
            List<string> tagList = new List<string>();
            List<string> tagDateList = new List<string>();
            const string TEMP_FOLDER_PATH = "repository";

            //клонируем во временную папку
            commandLineProcess.StartInfo.WorkingDirectory = Application.StartupPath;
            commandLineProcess.StartInfo.FileName = "git";
            commandLineProcess.StartInfo.Arguments = "clone " + textBoxAddress.Text + " " + TEMP_FOLDER_PATH;
            commandLineProcess.Start();
            commandLineProcess.WaitForExit();
            commandLineProcess.Close();

            //получаем список тэгов
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
            }

            //получаем информацию по каждому тэгу
            foreach (string tag in tagList)
            {
                commandLineProcess.StartInfo.FileName = "git";
                commandLineProcess.StartInfo.Arguments = "show " + tag;
                commandLineProcess.Start();
                outputReader = commandLineProcess.StandardOutput;
                commandLineProcess.WaitForExit(100);
                commandLineProcess.Close();
                outputReader.ReadLine();
                outputReader.ReadLine();
                tagDateList.Add(outputReader.ReadLine().Substring(8, 24));
                outputReader.ReadToEnd();
            }

            //конвертируем в XML
            //XmlDocument document = new XmlDocument();
            XDocument document = new XDocument();
            XElement root = new XElement("root");
            document.Add(root);
            for (int i = 0; i < tagList.Count; i++)
            {
                root.Add(new XElement("tag",
                    new XAttribute("name", tagList[i]),
                    new XAttribute("date", tagDateList[i])));
            }

            //сохранение
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml документ|*.xml|Все файлы|*.*";
            saveFileDialog.Title = "Сохранение XML файла";
            saveFileDialog.ShowDialog();
            if(saveFileDialog.FileName != "")
                document.Save(saveFileDialog.FileName);

            //временная папка больше не нужна
            foreach (string file in Directory.GetFiles(TEMP_FOLDER_PATH, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            try
            {
                Directory.Delete(TEMP_FOLDER_PATH, true);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
        }
    }
}