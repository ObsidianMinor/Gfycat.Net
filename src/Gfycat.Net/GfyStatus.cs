using System.Threading.Tasks;
using Model = Gfycat.API.Models.Status;

namespace Gfycat
{
    public class GfyStatus : IUpdatable
    {
        readonly GfycatClient _client;

        public UploadTask Task { get; private set; }
        public int Time { get; private set; }
        public string GfyName { get; private set; }

        internal GfyStatus(GfycatClient client, Model model)
        {
            _client = client;
            Update(model);
        }

        internal void Update(Model model)
        {
            Task = model.Task;
            Time = model.Time;
            GfyName = model.GfyName;
        }
        
        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await _client.ApiClient.GetGfyStatusAsync(GfyName, options));
    }
}
