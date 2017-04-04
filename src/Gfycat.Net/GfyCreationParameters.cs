using System.Collections.Generic;
using System.Linq;

namespace Gfycat
{
    /// <summary>
    /// Represents creation parameters for gfys
    /// </summary>
    public class GfyCreationParameters
    {
        internal API.Models.GfyParameters CreateModel()
        {
            return new API.Models.GfyParameters()
            {
                Captions = Captions?.Select(cap => (cap == null) ? null : new API.Models.Caption()
                {
                    Duration = cap.Duration,
                    FontHeight = cap.FontHeight,
                    RelativeFontHeight = cap.RelativeFontHeight,
                    RelativeX = cap.RelativeXPosition,
                    RelativeY = cap.RelativeYPosition,
                    StartSeconds = cap.StartSeconds,
                    Text = cap.Text,
                    X = cap.XPosition,
                    Y = cap.YPosition,
                }),
                Crop = (Crop == null) ? null : new API.Models.Crop()
                {
                    H = Crop.Height,
                    W = Crop.Width,
                    X = Crop.XPosition,
                    Y = Crop.YPosition
                },
                Cut = (Cut == null) ? null : new API.Models.Cut()
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

        internal string FetchUrl { get; set; }
        /// <summary>
        /// Sets the title of this gfy
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Sets the description of this gfy
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Sets the tags of this gfy
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// Instructs the uploader to skip the file duplication check
        /// </summary>
        public bool NoMd5 { get; set; }
        /// <summary>
        /// Sets whether this gfy will be private
        /// </summary>
        public bool Private { get; set; }
        /// <summary>
        /// Sets whether this gfy will be not safe for work
        /// </summary>
        public NsfwSetting Nsfw { get; set; }
        /// <summary>
        /// Sets the number of seconds into the video at the specified url to start reading
        /// </summary>
        public float FetchSeconds { get; set; }
        /// <summary>
        /// Sets the number of minutes into the video at the specified url to start reading
        /// </summary>
        public float FetchMinutes { get; set; }
        /// <summary>
        /// Sets the number of hours into the video at the specified url to start reading
        /// </summary>
        public float FetchHours { get; set; }
        /// <summary>
        /// Sets the list of captions to apply to this gfy
        /// </summary>
        public IEnumerable<Caption> Captions { get; set; }
        /// <summary>
        /// Sets the cut of this gfy
        /// </summary>
        public Cut Cut { get; set; }
        /// <summary>
        /// Sets the crop of this gfy
        /// </summary>
        public Crop Crop { get; set; }

        /// <summary>
        /// Constructs new creation parameters using the specified title, description, privacy setting, and nsfw setting
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="isPrivate"></param>
        /// <param name="isNsfw"></param>
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
    
    /// <summary>
    /// Represents a gfy caption
    /// </summary>
    public class Caption
    {
        /// <summary>
        /// Sets the text for this caption
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Sets the seconds for this caption to start at
        /// </summary>
        public int? StartSeconds { get; set; }
        /// <summary>
        /// Sets the duration for this caption in seconds
        /// </summary>
        public int? Duration { get; set; }
        /// <summary>
        /// Sets the font height of this caption
        /// </summary>
        public int? FontHeight { get; set; }
        /// <summary>
        /// Sets the X position of this caption
        /// </summary>
        public int? XPosition { get; set; }
        /// <summary>
        /// Sets the Y position of this caption
        /// </summary>
        public int? YPosition { get; set; }
        /// <summary>
        /// Sets the relative font height of this caption
        /// </summary>
        public int? RelativeFontHeight { get; set; }
        /// <summary>
        /// Sets the relative X position of this caption
        /// </summary>
        public int? RelativeXPosition { get; set; }
        /// <summary>
        /// Sets the relative Y position of this caption
        /// </summary>
        public int? RelativeYPosition { get; set; }

        /// <summary>
        /// Creates a new caption using the specified text
        /// </summary>
        /// <param name="text"></param>
        public Caption(string text)
        {
            Text = text;
        }
    }
    
    /// <summary>
    /// Represents a cut in a video or gfy
    /// </summary>
    public class Cut
    {
        /// <summary>
        /// Sets the duration of this cut in seconds
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Sets the start time of the duration of this cut in seconds
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Creates a new cut using the specified start time and duration
        /// </summary>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        public Cut(int start, int duration)
        {
            Duration = duration;
            Start = start;
        }
    }
    
    /// <summary>
    /// Represents an image crop for a gfy
    /// </summary>
    public class Crop
    {
        /// <summary>
        /// Sets the X position for this crop
        /// </summary>
        public int XPosition { get; set; }
        /// <summary>
        /// Sets the Y position for this crop
        /// </summary>
        public int YPosition { get; set; }
        /// <summary>
        /// Sets the width of this crop
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Sets the height of this crop
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Creates a new crop using the specified X position, Y position, width, and height
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Crop(int xpos, int ypos, int width, int height)
        {
            XPosition = xpos;
            YPosition = ypos;
            Width = width;
            Height = height;
        }
    }
}
