var obsidian = await gfycat.GetUserAsync("obsidianminor"); // Get a user object for obsidianminor
var albums = await obsidian.GetAlbumsAsync(); // Get obsidianminor's albums
var titanfall = albums.FirstOrDefault(x => x.Title == "Titanfall 2 Stuff"); // Get album info for an album with the title "Titanfall 2 Stuff"
var contents = await titanfall.GetContentsAsync(); // Get the album for this album info
var content = contents.Content; // Get the content of the album
foreach (var gfy in content)
    Console.WriteLine($"{gfy.Title} | {gfy.Url}"); // Write the title and URL for all of the album's entries to the console