var trendingTags = await gfycat.GetTrendingTagsPopulatedAsync(); // Get a collection representing the trending tags/feeds on Gfycat today
await trendingTags.Take(10).ForEachAsync(async feed => // Get the first 10 trending tags/feeds
{
    Console.WriteLine(feed.Tag); // Write the feed's tag to the console
    await feed.Take(10).ForEachAsync(x => Console.WriteLine($"{x.Name} | {x.Url}")); // Take the first 10 Gfys in the feed and write each name and URL to the console
    Console.WriteLine(); // Extra line for readability
});