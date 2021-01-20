using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace Common
{
    /// <summary>
    /// RAR文件操作
    /// </summary>
    public class RarClass
    {
        /// <summary>
        /// 是否安装了Winrar
        /// </summary>
        /// <returns></returns>
        static public bool Exists()
        {
            RegistryKey the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
            return !string.IsNullOrEmpty(the_Reg.GetValue("").ToString());
        }

        /// <summary>
        /// 打包成Rar
        /// </summary>
        /// <param name="patch">要打包的目录</param>
        /// <param name="rarPatch">打包完成后保存的目录</param>
        /// <param name="rarName">生成rar文件名</param>
        /// <param name="password">密码</param>
        public void CompressRAR(string patch, string rarPatch, string rarName, string password)
        {
            string the_rar;
            RegistryKey the_Reg;
            object the_Obj;
            string the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                if (password != "")
                {
                    password = "-p" + password;
                }

                the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                //the_rar = the_rar.Substring(1, the_rar.Length - 7);
                Directory.CreateDirectory(rarPatch);
                //命令参数
                //the_Info = " a    " + rarName + " " + @"C:Test?70821.txt"; //文件压缩
                //the_Info = " a " + rarName + " " + patch + " -r " + password;
                the_Info = " a \"" + rarName + "\" \"" + patch + "\" -r " + password; //防止文件名有空格时中断
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //打包文件存放目录
                the_StartInfo.WorkingDirectory = rarPatch;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CompressRARNoDir(string patch, string rarPatch, string rarName, string password)
        {
            string the_rar;
            RegistryKey the_Reg;
            object the_Obj;
            string the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                if (password != "")
                {
                    password = "-p" + password;
                }

                the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                //the_rar = the_rar.Substring(1, the_rar.Length - 7);
                Directory.CreateDirectory(rarPatch);
                //命令参数
                //the_Info = " a    " + rarName + " " + @"C:Test?70821.txt"; //文件压缩
                //the_Info = " a " + rarName + " " + patch + " -ep1 " + password;
                the_Info = " a  -ibck -inul  \"" + rarName + "\" \"" + patch + "\" -ep1 " + password; //防止文件名有空格时中断
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //打包文件存放目录
                the_StartInfo.WorkingDirectory = rarPatch;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打包成Rar
        /// </summary>
        /// <param name="fileList">要打包的文件路径列表</param>
        /// <param name="rarPatch">打包完成后保存的目录</param>
        /// <param name="rarName">生成rar文件名</param>
        /// <param name="password">密码</param>
        public void CompressMultiRAR(List<string> fileList, string rarPatch, string rarName, string password)
        {
            string the_rar;
            RegistryKey the_Reg;
            object the_Obj;
            string the_Info;
            ProcessStartInfo the_StartInfo;
            Process the_Process;
            try
            {
                if (password != "")
                {
                    password = "-p" + password;
                }

                the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                //the_rar = the_rar.Substring(1, the_rar.Length - 7);
                Directory.CreateDirectory(rarPatch);
                //命令参数a -ag -ibck bak.rar filename1 filename2
                //the_Info = " a    " + rarName + " " + @"C:Test?70821.txt"; //文件压缩
                StringBuilder sb = new StringBuilder();
                sb.Append("a -ep1 -ibck " + rarName);//-ep1 :生成压缩文件时忽略目录路径
                foreach (string filePath in fileList)
                {
                    if (filePath.Contains(" ")) //路径或文件名中含有空格的话需要在两边加上双引号
                    {
                        sb.Append(" \"" + filePath + "\"");
                    }
                    else
                    {
                        sb.Append(" " + filePath);
                    }
                }
                //the_Info = " a " + rarName + " " + patch + " -r " + password;
                the_Info = sb.ToString();
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //打包文件存放目录
                the_StartInfo.WorkingDirectory = rarPatch;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="unRarPatch">RAR解压目录</param>
        /// <param name="rarPatch">RAR文件所在目录</param>
        /// <param name="rarName">RAR文件名</param>
        /// <returns></returns>
        public string unCompressRAR(string unRarPatch, string rarPatch, string rarName)
        {
            string the_rar;
            RegistryKey the_Reg;
            object the_Obj;
            string the_Info;


            try
            {
                the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                //the_rar = the_rar.Substring(1, the_rar.Length - 7);

                if (Directory.Exists(unRarPatch) == false)
                {
                    Directory.CreateDirectory(unRarPatch);
                }
                the_Info = "x " + rarName + " " + unRarPatch + " -y";

                ProcessStartInfo the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_StartInfo.WorkingDirectory = rarPatch;//获取压缩包路径

                Process the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return unRarPatch;
        }
    }
}
