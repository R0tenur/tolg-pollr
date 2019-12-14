using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TolgPollr.Worker.Models;

namespace TolgPollr.Worker.Repository
{
    public interface ICurrentStateRepository
    {
        Task<HouseModel> GetState();
        Task SetState(HouseModel state);
    }
    public class CurrentStateRepository : ICurrentStateRepository
    {
        private readonly ILogger<CurrentStateRepository> _logger;
        private readonly string _filePath;
        public CurrentStateRepository(ILogger<CurrentStateRepository> logger)
        {
            _logger = logger;
            _filePath = Directory.GetCurrentDirectory() + "state.txt";
        }
        public async Task<HouseModel> GetState()
        {
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<HouseModel>(json);
            }

            return null;
        }

        public async Task SetState(HouseModel state)
        {
            if (state != null)
            {

                var json = JsonSerializer.Serialize(state);
                _logger.LogInformation("Writing to file", _filePath, json);
                await File.WriteAllTextAsync(_filePath, json);
            }
        }
    }
}