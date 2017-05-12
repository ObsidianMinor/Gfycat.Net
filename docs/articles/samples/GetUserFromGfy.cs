var targetGfy = await gfycat.GetGfyAsync("RepentantAmbitiousGerenuk"); // This is the Gfy who's author we want to get
var author = await targetGfy.GetUserAsync(); // Get the author from the Gfy
Console.WriteLine(author.Name); // Writes "ObsidianMinor" to the console, as he's the author of that Gfy