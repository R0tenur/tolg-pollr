using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TolgPollr.Worker.Exceptions;

namespace TolgPollr.Worker.Services
{
    public interface ICommandExecutionService
    {
        Task<string> ExecuteCommand(string command, params string[] args);
    }
    public class CommandExecutionService : ICommandExecutionService
    {
        public Task<string> ExecuteCommand(string command, params string[] args)
        {

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \" "  + command + ' ' + string.Join(' ', args)  + " \"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new ExternalCommandException(error);
            }

            return Task.FromResult(output);
        }
    }
}