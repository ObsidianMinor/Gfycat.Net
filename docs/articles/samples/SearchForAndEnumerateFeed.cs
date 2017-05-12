GfyFeed catsFeed = await gfycat.SearchAsync("cat"); // Get a feed with Gfys that are returned by a query for "cat"
await catsFeed.Take(20).ForEachAsync(g => // Take the first 20 Gfys in this feed
{
    Console.WriteLine($"{g.Name} : {g.Url}"); // Write the Gfy's name and URL to the console
});