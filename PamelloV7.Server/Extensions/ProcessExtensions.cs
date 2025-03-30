using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
 
namespace PamelloV7.Server.Extensions
{
    /// <summary>Suspend/resume a <see cref=”System.Diagnostics.Process”/>,
    /// by suspending or resuming all of its threads.</summary>
    /// <remarks><para>
    /// These methods use the <see cref=”CollectionExtensions.ToArray&lt;T&gt;”/> extension method
    /// on <b>ICollection</b> to convert a <b>ProcessThreadCollection</b> to an array.</para>
    /// <para>
    /// The threads are suspended/resumed in parallel for no particular reason, other than the hope
    /// that it may be better to do so as close to all at once as we can.</para></remarks>
    public static class ProcessExtensions
    {
		[DllImport("libc", SetLastError = true)]
		private static extern int kill(int pid, int sig);

		private const int SIGCONT = 18;
		private const int SIGSTOP = 19;

		private static void Execute(string file, string args) {
			var psi = new ProcessStartInfo();
			psi.FileName = file;
			psi.Arguments = args;

			using var process = Process.Start(psi);

			process?.WaitForExit();
		}


        #region Methods

        public static void SuspendLinux(this Process process) {
			kill(-process.Id, SIGSTOP);
		}
        public static void ResumeLinux(this Process process) {
			kill(-process.Id, SIGCONT);
		}
 
        public static void SuspendWindows(this Process process)
        {
            //Console.WriteLine("suspending");
            Action<ProcessThread> suspend = pt =>
            {
                var threadHandle = NativeMethods.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pt.Id);
 
                if (threadHandle != IntPtr.Zero)
                {
                    try
                    {
                        NativeMethods.SuspendThread(threadHandle);
                    }
                    finally
                    {
                        NativeMethods.CloseHandle(threadHandle);
                    }
                };
            };
 
            var threads = process.Threads;
 
            if (threads.Count > 1)
            {
                foreach (ProcessThread pt in threads) {
                    suspend(pt);
                }
            }
            else
            {
                suspend(threads[0]);
            }
        }
 
        public static void ResumeWindows(this Process process)
        {
            //Console.WriteLine("resuming");
            Action<ProcessThread> resume = pt =>
            {
                var threadHandle = NativeMethods.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pt.Id);
 
                if (threadHandle != IntPtr.Zero)
                {
                    try
                    {
                        NativeMethods.ResumeThread(threadHandle);
                    }
                    finally
                    {
                        NativeMethods.CloseHandle(threadHandle);
                    }
                }
            };

            var threads = process.Threads;
 
            if (threads.Count > 1)
            {
                foreach (ProcessThread pt in threads) {
                    resume(pt);
                }
            }
            else
            {
                resume(threads[0]);
            }
        }
 
        #endregion
 
        #region Interop
 
        static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);
 
            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
 
            [DllImport("kernel32.dll")]
            public static extern uint SuspendThread(IntPtr hThread);
 
            [DllImport("kernel32.dll")]
            public static extern uint ResumeThread(IntPtr hThread);
        }
 
        [Flags]
        enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }
 
        #endregion
    }
}
