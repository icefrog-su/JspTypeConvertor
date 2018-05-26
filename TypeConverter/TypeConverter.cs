using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace TypeConverter
{
    public partial class WorkWinForm : Form
    {
        public WorkWinForm()
        {
            InitializeComponent();
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {

            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string item in files)
                {
                    FileInfo fileinfo = new FileInfo(item);
                    string stuffix = fileinfo.Extension;
                    if (stuffix == null || stuffix.Equals(string.Empty) || stuffix.Trim().Length == 0)
                    {
                        DirectoryInfo dinfo = new DirectoryInfo(item);
                        bool exists = dinfo.Exists;
                        if (exists)
                        {
                            //Calls2CallsForSelf(item);
                            FileInfo[] fs = dinfo.GetFiles("*", SearchOption.AllDirectories);
                            foreach (FileInfo f in fs)
                            {
                                listBox1.Items.Add(f.FullName);
                            }
                            continue;
                        }
                    }
                    listBox1.Items.Add(item);
                }
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < listBox1.Items.Count; i++)
                { }
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                long mi = ts.Ticks;
                if (listBox1.Items.Count >= 0)
                {
                    label9.Text = mi + " [毫秒]";
                }
                label6.Text = listBox1.Items.Count + " [项]";
                long size = 0L;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    FileInfo fileItem = new FileInfo(listBox1.Items[i].ToString());
                    size += fileItem.Length;
                }
                label5.Text = size + " [KB]";
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取文件时发生错误,请检查是否拥有足够的权限并请查阅关于联系开发者."
                    + System.Environment.NewLine + "错误信息:"
                    + System.Environment.NewLine + ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("转换会通过字节流形式重新创建新文件(防止数据乱码),但可能因为运行环境问题导致数据损坏,请确保有相应的数据备份并点击确定开始转换!","转换",MessageBoxButtons.OKCancel,MessageBoxIcon.Asterisk);
            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;
            string news = t2.Text;
            string[] ff2 = news.Split('.');
            if (ff2.Length == 1)
                news = ff2[0] + "";
            else
                news = ff2[ff2.Length - 1] + "";
            string saveURL = savePath.Text;
            if (listBox1.Items.Count <= 0)
            {
                MessageBox.Show("请拖入需要转换的文件.");
                return;
            }
            if (saveURL == null || saveURL.Equals(""))
            {
                MessageBox.Show("请选择需要保存的文件夹地址.");
                return;
            }
               
            DirectoryInfo dire = new DirectoryInfo(saveURL);
            pb1.Maximum = listBox1.Items.Count;
            pb1.Show();
            pb1.Value = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var item in listBox1.Items)
            {
                using (StreamReader reader = new StreamReader(item.ToString(), Encoding.UTF8))
                {
                    string content = reader.ReadToEnd();
                    reader.Close();
                    FileInfo file = new FileInfo(saveURL + @"\" + new FileInfo(item.ToString()).Name.Split('.')[0] + "." + t2.Text);

                    FileStream fs1 = new FileStream(saveURL + @"\" + new FileInfo(item.ToString()).Name.Split('.')[0] + "." + t2.Text,FileMode.Create);
                    StreamWriter write = new StreamWriter(fs1, Encoding.UTF8);

                    //StreamWriter write = file.AppendText();
                    if (checkBox1.Checked)
                        write.WriteLine("<%@ page language=\"java\" import=\"java.util.*\" pageEncoding=\"UTF-8\"%>");
                    write.Write(content);
                    write.Close();
                }
                pb1.Value += 1;
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            long mi = ts.Milliseconds;
            DialogResult resu = MessageBox.Show("转换完成!"+System.Environment.NewLine+"文件数量:"+listBox1.Items.Count+",转换用时:"+mi+"毫秒."+System.Environment.NewLine+"是否立即查看?","转换完成",MessageBoxButtons.YesNo);
            if (resu == System.Windows.Forms.DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(saveURL);
            }
        }

        public void writeFiles(string olds,string news)
        {
            FileStream fsr = new FileStream(olds, FileMode.Open);
            StreamReader sr = new StreamReader(fsr, Encoding.Default);
            string content = sr.ReadToEnd();
            sr.Close();
            fsr.Close();

            FileStream fsw = new FileStream(news,FileMode.Append);
            StreamWriter write = new StreamWriter(fsw);
            if(checkBox1.Checked)
                write.Write("<%@ page language=\"java\" import=\"java.util.*\" pageEncoding=\"UTF-8\"%>\r\n" + content);
            else
                write.Write(content);
            write.Flush();
            fsw.Flush();
            write.Close();
            fsw.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string sx = stuffix.Text;
                string[] ff = sx.Split('.');
                string fx = "";
                if (ff.Length == 1)
                {
                    fx = ff[0] + "";
                }
                else
                {
                    fx = ff[ff.Length - 1] + "";
                }
                int count = 0;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    string item = listBox1.Items[i].ToString();
                    string[] t = item.Split('.');
                    if (t.Length == 1)
                    {
                        string ts = t[0];
                        if (ts.ToUpper().Equals(stuffix.Text.ToUpper()))
                        {
                            count++;
                        }
                    }
                    else if (t.Length > 1)
                    {
                        string[] f = stuffix.Text.ToString().Split('.');
                        string fz = "";
                        if (ff.Length == 1)
                        {
                            fz = f[0] + "";
                        }
                        else
                        {
                            fz = f[f.Length - 1] + "";
                        }
                        string ts = t[t.Length - 1];
                        if (ts.Equals(fz))
                        {
                            count++;
                        }
                    }
                }
                MessageBox.Show("查询到:" + count + "个匹配项.");
            }
            catch
            {
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedItems.Count >= 1)
                {
                    System.Diagnostics.Process.Start(listBox1.SelectedItems[0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件打开错误,改错误可能由于文件类型不允许打开或文件路径导入错误." +
                    System.Environment.NewLine + "错误信息:" + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string saveEdStuffix = txt_bl.Text;
            string vickEdStuffix = txt_tc.Text;
            if (saveEdStuffix.Equals(string.Empty) && vickEdStuffix.Equals(string.Empty))
                return;
            if (saveEdStuffix.Equals(vickEdStuffix))
            {
                MessageBox.Show("剔除条件和保留条件不能相同!","筛选");
                return;
            }
            if (saveEdStuffix.Split('.').Length == 1)
                saveEdStuffix = saveEdStuffix.Split('.')[0];
            else
                saveEdStuffix = saveEdStuffix.Split('.')[saveEdStuffix.Split('.').Length - 1];
            if (vickEdStuffix.Split('.').Length == 1)
                vickEdStuffix = vickEdStuffix.Split('.')[0];
            else
                vickEdStuffix = vickEdStuffix.Split('.')[vickEdStuffix.Split('.').Length - 1];
            //MessageBox.Show("save:" + saveEdStuffix + ",vick:" + vickEdStuffix);
            ArrayList paths = new ArrayList();
            foreach (Object item in listBox1.Items)
            {
                paths.Add(item);
            }
            for (int i = 0; i < paths.Count; i++)
            {
                string sx = paths[i].ToString().Split('.')[paths[i].ToString().Split('.').Length-1];
                if (saveEdStuffix != null && !saveEdStuffix.Equals(string.Empty))
                {
                    if (!saveEdStuffix.ToUpper().Equals(sx.ToUpper()))
                    {
                        listBox1.Items.Remove(paths[i].ToString());
                    }
                }
                if (vickEdStuffix != null && !vickEdStuffix.Equals(string.Empty))
                {
                    if (vickEdStuffix.ToUpper().Equals(sx.ToUpper()))
                    {
                        listBox1.Items.Remove(paths[i].ToString());
                    }
                }
            }
        }

        private void WorkWinForm_Load(object sender, EventArgs e)
        {
            pb1.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog open = new FolderBrowserDialog();
            DialogResult result = open.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                savePath.Text = open.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("[文件类型转换工具] Copyright 2017 by ICE FROG" + System.Environment.NewLine + "开发者:ICE FROG[刘苏文]" + System.Environment.NewLine+
                "联系邮箱:icefrogsu@outlook.com/icefrogsu@gmail.com" + System.Environment.NewLine+"本软件使用过程中全部免费."+
                System.Environment.NewLine+"目前测试允许转换类型:[html/jsp/css/js/txt]"+
                System.Environment.NewLine+"版本号:V1.0.0");
        }

        private void 清除该项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                }
            }
            catch { }
        }

        private void 清除所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.Items.Count>=1)
                {
                    listBox1.Items.Clear();
                }
            }
            catch { }
        }
    }
}
