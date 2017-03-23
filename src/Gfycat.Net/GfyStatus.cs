using System;
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
            if (model.GfyName != null)
                GfyName = model.GfyName;
        }
        
        public async Task UpdateAsync(RequestOptions options = null)
            => Update(await _client.ApiClient.GetGfyStatusAsync(GfyName, options));

        /// <summary>
        /// Gets the <see cref="Gfy"/> this status is checking if it's upload task is complete
        /// </summary>
        /// <param name="options"></param>
        /// <returns>An awaitable <see cref="Gfy"/></returns>
        /// <exception cref="InvalidOperationException">If the gfy's upload task isn't set to complete, running this method will be an invalid operation</exception>
        public async Task<Gfy> GetGfyAsync(RequestOptions options = null)
        {
            if (Task != UploadTask.Complete)
                throw new InvalidOperationException("The Gfy's upload isn't complete!");

            return await _client.GetGfyAsync(GfyName, options);
        }
    }
}
