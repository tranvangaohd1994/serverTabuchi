using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Giá trị luận lý cho biết ứng dụng này
            // có quyền sở hữu Mutex hay không.
            bool ownmutex;

            // Tạo và lấy quyền sở hữu một Mutex có tên là Server;
            using (Mutex mutex = new Mutex(true, "Server", out ownmutex))
            {
                // Nếu ứng dụng sở hữu Mutex, nó có thể tiếp tục thực thi;
                // nếu không, ứng dụng sẽ thoát.
                if (ownmutex)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Main_form());
                    //giai phong Mutex;
                    mutex.ReleaseMutex();
                }
                else
                    Application.Exit();
            }

        }
    }
}
