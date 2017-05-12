// The following are all valid
var gfy = await gfycat.GetGfyFromUrlAsync("https://gfycat.com/FatalLavishGlobefish");
Console.WriteLine(gfy.Title);
gfy = await gfycat.GetGfyFromUrlAsync("https://gfycat.com/gifs/detail/FatalLavishGlobefish");
Console.WriteLine(gfy.Title);
gfy = await gfycat.GetGfyFromUrlAsync("https://gfycat.com/@obsidianminor/albums/titanfall_2_stuff/detail/FatalLavishGlobefish");
Console.WriteLine(gfy.Title);