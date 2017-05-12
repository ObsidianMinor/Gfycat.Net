var currentUser = await gfycat.GetCurrentUserAsync(); // Get the current user
var folders = await currentUser.GetFoldersAsync(); // Get the current user's folders
var overwatch = folders.FirstOrDefault(x => x.Title == "Overwatch Stuff"); // Get info for a folder titled "Overwatch Stuff"
var contents = await overwatch.GetContentsAsync(); // Get the folder tied to the folder info we got above
var content = contents.Content; // Get the content of the folder
foreach (var gfy in content)
    Console.WriteLine($"{gfy.Title} | {gfy.Url}"); // Write the title and URL for all the Gfys in this folder to the console