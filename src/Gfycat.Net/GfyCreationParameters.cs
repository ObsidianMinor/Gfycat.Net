using System.Collections.Generic;
using Newtonsoft.Json;
using Gfycat.Converters;
using System.Linq;

namespace Gfycat
{
    public class GfyCreationParameters
    {
        internal API.Models.GfyParameters CreateModel()
        {
            return new API.Models.GfyParameters()
            {
                Captions = Captions.Select(cap => new API.Models.Caption()
                {
                    Duration = cap.Duration,
                    FontHeight = cap.FontHeight,
                    RelativeFontHeight = cap.RelativeFontHeight,
                    RelativeX = cap.RelativeX,
                    RelativeY = cap.RelativeY,
                    StartSeconds = cap.StartSeconds,
                    Text = cap.Text,
                    X = cap.X,
                    Y = cap.Y,
                }),
                Crop = new API.Models.Crop()
                {
                    H = Crop.H,
                    W = Crop.W,
                    X = Crop.X,
                    Y = Crop.Y
                },
                Cut = new API.Models.Cut()
                {
                    Duration = Cut.Duration,
                    Start = Cut.Start
                },
                Description = Description,
                FetchHours = FetchHours,
                FetchMinutes = FetchMinutes,
                FetchSeconds = FetchSeconds,
                FetchUrl = FetchUrl,
                NoMd5 = NoMd5,
                Nsfw = Nsfw,
                Private = Private,
                Tags = Tags,
                Title = Title
            };
        }

        public string FetchUrl { get; internal set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// Instructs the uploader to skip the file duplication check
        /// </summary>
        public bool NoMd5 { get; set; }
        public bool Private { get; set; }
        public NsfwSetting Nsfw { get; set; }
        public float FetchSeconds { get; set; }
        public float FetchMinutes { get; set; }
        public float FetchHours { get; set; }
        public IEnumerable<Caption> Captions { get; set; }
        public Cut Cut { get; set; }
        public Crop Crop { get; set; }

        public GfyCreationParameters(string title = null, string description = null, bool isPrivate = false, NsfwSetting isNsfw = NsfwSetting.Clean)
        {
            Title = title;
            Description = description;
            Private = isPrivate;
            Nsfw = isNsfw;
        }

        internal GfyCreationParameters(string fetchUrl)
        {
            FetchUrl = fetchUrl;
        }
    }
    
    public class Caption
    {
        public string Text { get; set; }
        public int? StartSeconds { get; set; }
        public int? Duration { get; set; }
        public int? FontHeight { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? RelativeFontHeight { get; set; }
        public int? RelativeX { get; set; }
        public int? RelativeY { get; set; }

        public Caption(string text)
        {
            Text = text;
        }
    }
    
    public class Cut
    {
        public int Duration { get; set; }
        public int Start { get; set; }

        public Cut(int start, int duration)
        {
            Duration = duration;
            Start = start;
        }
    }
    
    public class Crop
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public Crop(int xpos, int ypos, int width, int height)
        {
            X = xpos;
            Y = ypos;
            W = width;
            H = height;
        }
    }
}
