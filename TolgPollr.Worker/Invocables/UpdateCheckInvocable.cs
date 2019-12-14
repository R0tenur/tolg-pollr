using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TolgPollr.Worker.Configuration;
using TolgPollr.Worker.Exceptions;
using TolgPollr.Worker.Models;
using TolgPollr.Worker.Repository;
using TolgPollr.Worker.Services;

namespace TolgPollr.Worker.Invocables
{
    public class UpdateCheckInvocable : IInvocable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private const string AuthHeaderName = "x-functions-key";

        public UpdateCheckInvocable(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }
        public async Task Invoke()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var excecutor = scope.ServiceProvider.GetRequiredService<ICommandExecutionService>();
                var repository = scope.ServiceProvider.GetRequiredService<ICurrentStateRepository>();
                try
                {
                    var checkerUrl = _configuration.GetValue<string>(ConfigurationConstants.CheckerUrl);
                    var authKey = _configuration.GetValue<string>(ConfigurationConstants.CheckerAuthKey);
                    var serverTask = checkerUrl.WithHeader(AuthHeaderName, authKey).GetJsonAsync<HouseModel>();

                    var currentStateTask = repository.GetState();

                    var serverState = await serverTask;

                    var currentState = await currentStateTask;
                    if (currentState != null && currentState.Heat == serverState.Heat == (await currentStateTask).Heat)
                    {
                        return;
                    }

                    if (serverState.Heat)
                    {
                        await excecutor.ExecuteCommand(
                            TerminalCommandConstants.LircCommand,
                            TerminalCommandConstants.LircHeatParameters
                        );
                    }

                    if (!serverState.Heat)
                    {
                        await excecutor.ExecuteCommand(
                            TerminalCommandConstants.LircCommand,
                            TerminalCommandConstants.LircHeatParameters
                        );
                    }

                    await repository.SetState(serverState);

                }
                catch (ExternalCommandException ex)
                {
                    // Should update this on server on failure
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    // Add more custom errors here
                    Console.WriteLine("Unexpeced error " + ex.Message);
                }
            }
        }
    }
}