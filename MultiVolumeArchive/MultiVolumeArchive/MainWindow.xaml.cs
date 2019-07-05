using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FirstWpfApp
{
    public partial class MainWindow : Window
    {
        //Путь к каталогу в котором находится данный файл
        string filePath;
        //Полный путь к файлу
        string filename;
        string[] str = new string[] { "байт", "кБ", "мБ"};
        int MAX;
        bool fileIsChecked = false;
       


        public MainWindow()
        {
            InitializeComponent();
            cbx.ItemsSource = str;
            cbx.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Считывает полный путь к файлу
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                // Open document
                filename = dialog.FileName;
                //TextBlock textBlock = new TextBlock();
                filePath = System.IO.Path.GetDirectoryName(filename);
                textBlock.Text = filename;
            }

            fileIsChecked = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (fileIsChecked)
            {
                progressBar.Value = 0;
                int SIZE = Convert.ToInt32(textBlock2.Text);

                filename = textBlock.Text;

                // Определяет единицу информации 
                // По-умолчанию - 1Мб
                if (textBlock2.Text == null)
                {
                    SIZE = 1024;
                }
                if (cbx.SelectedItem.ToString() == "байт")
                {
                    SIZE *= 1;
                }
                else if (cbx.SelectedItem.ToString() == "кБ")
                {
                    SIZE *= 1024;
                }
                else if (cbx.SelectedItem.ToString() == "мБ")
                {
                    SIZE *= 1048576;
                }

                MAX = SIZE;

                byte[] bt = File.ReadAllBytes(filename);


                // Нахождения индекса элемента с которого начинается расширение
                int indxx = 0;
                for (int i = 0; i < filename.Length; i++)
                {
                    if (filename[i] == '.')
                    {
                        indxx = i;
                    }
                }

                string extension = "";
                for (int i = indxx; i < filename.Length; i++)
                {
                    extension += filename[i];
                }
                //string extension = filename.Substring(filename.Length - 5);

                // Определяет количество архивов
                int count = (bt.Length + SIZE - 1) / SIZE;

                int cnt = 0;

                // Имя файла
                string flname = "";
                for (int i = filePath.Length + 1; i < filename.Length - extension.Length; i++)
                {
                    flname += filename[i];
                }

                // Создание архивов
                for (int i = 0; i < count; i++)
                {
                    using (FileStream zipToOpen = new FileStream(filePath + "\\" + flname + "@" + i + ".zip", FileMode.OpenOrCreate))
                    {
                        using (ZipArchive zip = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry readmeEntry = zip.CreateEntry(i + extension);
                            using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                            {

                                long _byte_counter = SIZE;
                                while (_byte_counter > 0 && cnt < bt.Length)
                                {
                                    _byte_counter--;
                                    writer.BaseStream.WriteByte(bt[cnt]);
                                    progressBar.Value++;
                                    cnt++;
                                }
                            }


                        }
                    }
                }

                System.IO.File.WriteAllText(filePath + "\\" + flname + "_Info.txt", bt.Length.ToString() + "\r\n" + SIZE + "\r\n" + extension + "\r\n" + flname);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            //Считывает полный путь к файлу
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                // Open document
                filename = dialog.FileName;
                //TextBlock textBlock = new TextBlock();
                filePath = System.IO.Path.GetDirectoryName(filename);
                textBlock.Text = filename;
            }

            string infoFile = "";
            for (int i = filePath.Length + 1; i < filename.Length; i++)
            {
                infoFile += filename[i];
            }

            //string sz = System.IO.File.ReadAllText(filePath + "\\" + "size.txt");
            /*var indxx = sz.IndexOf('@');
            var indxx2 = sz.IndexOf('?');
            string btSz = "";
            string nbOfByte = "";
            string extension = "";
            for (int i = 0; i < indxx; i++)
            {
                btSz += sz[i];
            }

            for (int i = indxx + 1; i < indxx2; i++)
            {
                nbOfByte += sz[i];
            }


            for (int i = indxx2 + 1; i < sz.Length; i++)
            {
                extension += sz[i];
            }*/



            string btSz = File.ReadLines(filePath + "\\" + infoFile).Skip(0).First();

            string nbOfByte = File.ReadLines(filePath + "\\" + infoFile).Skip(1).First();

            string extension = File.ReadLines(filePath + "\\" + infoFile).Skip(2).First();

            string flname = File.ReadLines(filePath + "\\" + infoFile).Skip(3).First();


            string[] allFoundFiles = Directory.GetFiles(filePath, "*@*.zip", SearchOption.AllDirectories);
            long count = 0;
            int size = Convert.ToInt32(btSz);
            int bytesCount = Convert.ToInt32(nbOfByte);
            byte[] byteArray = new byte[size];
          
            int[,] array = new int[allFoundFiles.Length, 2];
            string tm = "";
            for (int i = 0; i < allFoundFiles.Length; i++)
            {
                var a = allFoundFiles[i];
                var fInd = a.IndexOf('@');
                tm = "";
                for (int j = fInd + 1; j <a.Length - extension.Length; j++)
                {
                    tm += a[j];
                }
                array[i, 0] = Convert.ToInt32(tm);
                array[i, 1] = i;

            }

            for (int i = 0; i < allFoundFiles.Length; i++)
            {
                for (int j = 0; j < allFoundFiles.Length - 1; j++)
                {
                    if (array[j, 0] > array[j + 1, 0])
                    {
                        int z = array[j, 0];
                        int y = array[j, 1];
                        array[j, 0] = array[j + 1, 0];
                        array[j, 1] = array[j + 1, 1];
                        array[j + 1, 0] = z;
                        array[j + 1, 1] = y;
                    }
                }
            }

            int[] indexOfFiles = new int[allFoundFiles.Length];

            for (int i = 0; i < allFoundFiles.Length; i++)
            {
                indexOfFiles[i] = array[i, 1];
            }

           
           // int iteratCount = 0;

            for (int i = 0; i < allFoundFiles.Length; i++)
            {
                using (FileStream zipToOpen = new FileStream(allFoundFiles[indexOfFiles[i]], FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        ZipArchiveEntry readmeEntry = archive.GetEntry(i + extension);
                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                        {
                            var _byte_counter = bytesCount;
                            while (_byte_counter > 0 && count < size)
                            {
                                _byte_counter--;
                                byteArray[count] = (byte)writer.BaseStream.ReadByte();
                                count++;

                            }


                        }
                    }
                }
            }

            using (FileStream zipToOpen = new FileStream(filePath + "\\" +flname+".zip", FileMode.OpenOrCreate))
            {
                using (ZipArchive zip = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = zip.CreateEntry(flname+extension);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (var item in byteArray)
                        {
                            writer.BaseStream.WriteByte(item);
                            progressBar.Value++;
                        }


                    }
                }
            }

        }
    }
    
}