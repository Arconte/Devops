using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNetTools
{
    public class ProcessController
    {
        public string Run(string args, string WorkingDirectory)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"powershell.exe";

            startInfo.Arguments = args;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.WorkingDirectory = WorkingDirectory;
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                StringBuilder values = new StringBuilder();

                bool errorReceived = false;
                process.OutputDataReceived += (s, e) =>
                {
                    lock (values)
                    {
                        values.Append(e.Data);
                    }
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    lock (values)
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            errorReceived = true;
                            values.Append("  ! >  " + e.Data);
                        }
                    }
                };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit();

                var success = process.ExitCode == 0 ? true : false;

                var resultString = values.ToString();

                if (errorReceived)
                    success = false;

                if (!success)
                    throw new ApplicationException(resultString);

                return resultString;
            }

        }
    }
}
