var obsidianMinor = await gfycat.GetUserAsync("obsidianminor"); // Get the user with the ID of "obsidianminor"
var feed = await obsidianMinor.GetGfyFeedAsync(); // Get the user's Gfy feed (all their public uploaded Gfys)

await feed.Take(20).ForEachAsync(async g => // Take the first 20 Gfys from this feed
{
  Console.WriteLine($"Downloading {g.Title} ({g.WebmSize}MB)...");
  using (var client = new HttpClient()) // Create a new HTTP client
  using (var stream = new FileStream(g.Name + ".webm", FileMode.Create)) // Create a file to which we'll copy the Gfy
  using (var writer = new StreamWriter(stream)) // Create a writer for said file
  using (var response = await client.GetAsync(g.WebmUrl)) // Get the HTTP response from the Gfy's direct webm URL
  using (var content = response.Content)
  {
      var data = await content.ReadAsStreamAsync(); // Download the Gfy
      await data.CopyToAsync(stream); // Copy the Gfy's data to our file
  }
});