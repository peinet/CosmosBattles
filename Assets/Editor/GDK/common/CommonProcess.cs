using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Editor.GDK.common
{
    class CommonProcess : System.Diagnostics.Process
    {
        public void exit()
        {
            StandardInput.WriteLine("exit");
            WaitForExit();
            StandardError.Close();//关闭流
            StandardOutput.Close();
            Close();
        }
    }
}
