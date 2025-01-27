﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FirstProject
{
    public partial class Form1 : Form
    {
        public string[] fileNames = Array.Empty<string>();
        public string[] filePaths = Array.Empty<string>();
        public int dcount = 0, drcount = 0, i, count;
        public string singleFilePath, folderPath, commandText;
        public string[] directoryFiles, dragFiles, dragBtnFiles, dragFilePaths, dragFileNames;
       
        //Initial Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            filesListView.Columns.Add("Files Selected", 610, HorizontalAlignment.Left);
            label3.Hide();
            progressBar1.Hide();
        }

        //Clear All files button
        private void Button1_Click(object sender, EventArgs e)
        {
            filesListView.Clear();
            commandTxt.Text = string.Empty;
            filesListView.Columns.Add("Files Selected", 610, HorizontalAlignment.Left);
            label3.Hide();
            progressBar1.Hide();
        }

        //Single file selection button
        private void SingleFileebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog singleFileDialog = new OpenFileDialog
            {
                Title = "Browse to select a single file",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
            };

            if (singleFileDialog.ShowDialog() == DialogResult.OK)
            {
                singleFilePath = singleFileDialog.FileName;
                filesListView.Items.Add(singleFilePath);
            }
        }

        //Directory/folder selection button
        private void FolderSelectorbtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {

                folderPath = folderBrowser.SelectedPath;
                directoryFiles = Directory.GetFiles(folderPath);
                dcount = directoryFiles.Length;
                foreach (string item in directoryFiles)
                {
                    filesListView.Items.Add(item);
                }
            }
        }

        //Run button
        private void Run_Btn_Click(object sender, EventArgs e)
        {
            commandText = commandTxt.Text.ToString().Trim();
            if (String.IsNullOrWhiteSpace(singleFilePath) && dcount < 1 && drcount < 1)
            {
                MessageBox.Show("Please select input files to run the command", "Error");
            }
            else
            {
                var result = MessageBox.Show("Please confirm to run the command", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    int i;
                    if (!String.IsNullOrWhiteSpace(commandText))
                    {
                        Array.Resize(ref fileNames, filesListView.Items.Count);
                        Array.Resize(ref filePaths, filesListView.Items.Count);
                        count = fileNames.Length;
                        string[] prefix = new string[count];
                        string[] suffix = new string[count];
                        string[] finalCommand = new string[count];
                        string[] commands = new string[count];
                        for (i = 0; i < count; i++)
                        {
                            fileNames[i] = filesListView.Items[i].Text;
                            fileNames[i] = Path.GetFileName(fileNames[i]);
                            prefix[i] = filesListView.Items[i].Text;
                            prefix[i] = Path.GetFileNameWithoutExtension(prefix[i]);
                            suffix[i] = filesListView.Items[i].Text;
                            suffix[i] = Path.GetExtension(suffix[i]);
                            filePaths[i] = filesListView.Items[i].Text;
                            filePaths[i] = Path.GetDirectoryName(filePaths[i]);
                        }                       

                        for (i = 0; i < count; i++)
                        {
                            commands[i] = commandText;
                            commands[i] = commands[i].Replace("%filename%", fileNames[i], StringComparison.CurrentCultureIgnoreCase);
                            commands[i] = commands[i].Replace("%filepath%", filePaths[i], StringComparison.CurrentCultureIgnoreCase);
                            commands[i] = commands[i].Replace("%prefix%", prefix[i], StringComparison.CurrentCultureIgnoreCase);
                            commands[i] = commands[i].Replace("%suffix%", suffix[i], StringComparison.CurrentCultureIgnoreCase);
                            finalCommand[i] = commands[i];
                        }

                        try
                        {
                            for (i = 0; i < count; i++)
                            {
                                ProcessStartInfo processInfo;
                                Process process;
                                processInfo = new ProcessStartInfo("cmd.exe", "/c " + finalCommand[i]);
                                processInfo.CreateNoWindow = true;
                                processInfo.UseShellExecute = true;
                                process = Process.Start(processInfo);
                                process.WaitForExit();
                                process.Close();
                                progressBar1.Show();
                                label3.Show();
                                progressBar1.Minimum = 0;
                                progressBar1.Maximum = count;
                                progressBar1.Value = i + 1;
                                if(i== count - 1)
                                {
                                    MessageBox.Show("Completed executing the commands", "Completion Message");
                                }
                            }
                            //for (i = 0; i < count; i++)
                            //{
                            //    var process = new Process();
                            //    process.StartInfo.CreateNoWindow = false;
                            //    process.StartInfo.UseShellExecute = true;
                            //    process.StartInfo.FileName = finalCommand[i];
                            //    process.Start();
                            //    process.Close();
                            //    progressBar1.Show();
                            //    label3.Show();
                            //    progressBar1.Minimum = 1;
                            //    progressBar1.Maximum = count;
                            //    progressBar1.Value = i + 1;
                            //    //System.Diagnostics.Process.Start("CMD.exe", finalCommand[i]);
                            //    //MessageBox.Show(finalCommand[i]);
                            //}
                        }
                        catch (Exception runException)
                        {
                            MessageBox.Show("Unable to run the command on file" + fileNames[i], "Error");
                            try
                            {
                                StreamWriter sw = new StreamWriter(@"C:\Apps\Error_Log.txt");
                                sw.WriteLine(DateTime.Now);
                                sw.WriteLine("-----------------------");
                                sw.WriteLine(runException);
                                sw.WriteLine("---------------------------------------------------");
                                sw.Close();
                                MessageBox.Show(@"Check failure log under C:\Apps\Error_Log.txt", "Information");
                                Application.Restart();
                            }
                            catch (Exception e1)
                            {
                                MessageBox.Show("Exception caused in writing log: " + e1.Message, "Warning");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No command file was pasted", "Missing Information");
                    }
                }
                else
                {
                    MessageBox.Show("User cancelled running the batch command", "Cancelled Operation");
                }
            }
        }

        //Drag files button
        private void Drag_Button_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dragFileDialog = new OpenFileDialog
            {
                Title = "Browse to select the input file/s",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Multiselect = true
            };

            if (dragFileDialog.ShowDialog() == DialogResult.OK)
            {
                dragBtnFiles = dragFileDialog.FileNames;
                drcount = dragBtnFiles.Length;
                foreach (string item in dragBtnFiles)
                {
                    filesListView.Items.Add(item);
                }
            }
        }

        //Function to enable dragging of items
        private void Drag_Button_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        //Function to drop items into event selected
        private void Drag_Button_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                dragFiles = (string[])(e.Data.GetData(DataFormats.FileDrop));
                drcount = dragFiles.Length;
            }
            foreach (string item in dragFiles)
            {
                filesListView.Items.Add(item);
            }
        }

        public Form1()
        {
            InitializeComponent();

        }
    }
}
