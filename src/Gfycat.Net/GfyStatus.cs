using Model = Gfycat.API.Models.Status;

namespace Gfycat
{
    public class GfyStatus
    {
        public UploadTask Task { get; internal set; }
        public int Time { get; internal set; }
        public string GfyName { get; internal set; }

        internal GfyStatus(Model model)
        {
            Task = model.Task;
            Time = model.Time;
            GfyName = model.GfyName;
        }
    }
}
