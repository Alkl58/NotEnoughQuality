﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NotEnoughQuality
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                TextBoxReferenceFile.Text = openFileDialog.FileName;

        }

        private void ButtonOpenFileToCompare_Click(object sender, RoutedEventArgs e)
        {
            //Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                TextBoxDistortedFile.Text = openFileDialog.FileName;

        }

        private void ButtonStartComparison_Click(object sender, RoutedEventArgs e)
        {

            if (TextBoxDistortedFile.Text == " Input File to Compare")
            {
                MessageBox.Show("Please select a File to Compare");
            }else if(TextBoxReferenceFile.Text == " Input Reference File")
            {
                MessageBox.Show("Please select a reference File");
            }else
            {
                string RefFile = "";
                string DistortedFile = "";
                int height = 1080;
                int width = 1920;
                RefFile = TextBoxReferenceFile.Text;
                DistortedFile = TextBoxDistortedFile.Text;
                StartTask(RefFile, DistortedFile, height, width);
            }

        }

        private async void StartTask(string RefFile, string DistortedFile, int height, int width)
        {
            //Run encode class async
            await Task.Run(() => compare(RefFile, DistortedFile, height, width));
        }

        public void compare(string RefFile, string Distortedfile, int height, int width)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ffmpeg.exe -y -i " + '\u0022' + RefFile + '\u0022' + " -pix_fmt yuv420p -vsync 1 "+ '\u0022'+  RefFile + ".yuv"+ '\u0022';
            process.StartInfo = startInfo;
            Console.WriteLine(startInfo.Arguments);
            process.Start();
            process.WaitForExit();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ffmpeg.exe -y -i " + '\u0022' + Distortedfile + '\u0022' + " -pix_fmt yuv420p -vsync 1 " + '\u0022' + Distortedfile + ".yuv" + '\u0022';
            process.StartInfo = startInfo;
            Console.WriteLine(startInfo.Arguments);
            process.Start();
            process.WaitForExit();


            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C VMAF\\vmafossexec.exe yuv444p "+ width + " " + height + " " + '\u0022' + RefFile + ".yuv" + '\u0022' + " "+ '\u0022' + Distortedfile + ".yuv" + '\u0022' + " VMAF\\model\\vmaf_v0.6.1.pkl --psnr --ssim";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            Console.WriteLine(startInfo.Arguments);
            
            process.Start();

            StreamReader reader = process.StandardOutput;
            // Perform reading and writing of standard output to Console
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                TextBoxOutput.Dispatcher.Invoke(() => TextBoxOutput.Text = TextBoxOutput.Text + "\n" + line, DispatcherPriority.Background);
            } // end while

            process.WaitForExit();

            try
            {
                File.Delete(Distortedfile + ".yuv");
                File.Delete(RefFile + ".yuv");
            }
            catch { }
            

        }

    }
}
