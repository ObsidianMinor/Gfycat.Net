GfyStatus gfyStatus = await gfycat.CreateGfyAsync(File.Open("somefile", FileMode.Open)); // Get a stream for the file we want to upload and upload it
Gfy completedGfy = await gfyStatus.GetGfyWhenCompleteAsync(); // Wait for the upload to complete and get the resulting Gfy
Console.WriteLine($"{completedGfy.Title} | {completedGfy.Url}"); // Write the title and URL for our new Gfy to the console