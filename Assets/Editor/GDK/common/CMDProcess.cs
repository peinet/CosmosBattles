using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assets.Editor.GDK.common
{
    class CMDProcess : System.Diagnostics.Process
    {
        DataReceivedEventHandler outputCallBack;
        DataReceivedEventHandler errorCallBack;
        //快捷的创建一个CMD使用
        public static CMDProcess CreateAInstance(bool CreateNoWindow = true)
        {
            var p = new CMDProcess();
            p.StartInfo.FileName = "cmd.exe"; //命令
            p.StartInfo.StandardOutputEncoding = Encoding.Default;//不加这行会导致输出的信息为乱码 坑
            p.StartInfo.StandardErrorEncoding = Encoding.Default;//不加这行会导致输出的错误为乱码 坑
            p.StartInfo.UseShellExecute = false; //不启用shell启动进程
            p.StartInfo.RedirectStandardInput = true; // 重定向输入
            p.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            p.StartInfo.RedirectStandardError = true; // 重定向错误输出
            p.StartInfo.CreateNoWindow = CreateNoWindow; // 不创建新窗口
            System.Console.InputEncoding = System.Text.Encoding.UTF8;//不加这行会导致输入命令的首字母乱码 坑
            return p;
        }
        public CMDProcess()
        {
        }
        public void setDataReceived(DataReceivedEventHandler callBack)
        {
            outputCallBack = callBack;
            OutputDataReceived += outputCallBack;
            BeginOutputReadLine();
            //new DataReceivedEventHandler(delegate (object sender, DataReceivedEventArgs e)
            //{
            //});
        }
        public void setErrorReceived(DataReceivedEventHandler callback)
        {
            errorCallBack = callback;
            ErrorDataReceived += errorCallBack;
            BeginErrorReadLine();
            //new DataReceivedEventHandler(delegate (object sender, DataReceivedEventArgs e)
            //{
            //});
        }
        public void exit()
        {
            StandardInput.WriteLine("exit");
            WaitForExit();
            if(outputCallBack == null)
            {
                StandardOutput.Close();
            }else
            {
                outputCallBack = null;
            }
            if (errorCallBack == null)
            {
                StandardError.Close();//关闭流
            }
            else
            {
                errorCallBack = null;
            }
            Close();
        }

    }
}

