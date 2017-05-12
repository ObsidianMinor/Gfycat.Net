var currentUser = await gfycat.GetCurrentUserAsync(); // Get the current user
var bookmarks = await currentUser.GetBookmarkFoldersAsync(); // Get booksmark folders for the current user
var overwatchPotgs = bookmarks.FirstOrDefault(x => x.Title == "Overwatch POTGs"); // Find the bookmark folder info with the title "Overwatch POTGs"
var contents = await overwatchPotgs.GetContentsAsync(); // Get the bookmark folder for the info we got above
var content = contents.Content; // Get the content of the folder
foreach (var gfy in content)
    Console.WriteLine($"{gfy.Title} | {gfy.Url}"); // Write the title and URL for all the Gfys in this folder to the console