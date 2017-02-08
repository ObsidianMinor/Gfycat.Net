using Gfycat;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

    public Task Start() => SimpleFileFetch();

    public async Task SimpleFileFetch()
    {
        GfycatClient client = new GfycatClient("2_9x3b-P", "L6H4sz0rsLxdTaKJrEY8DD7WLBwv7GppCZAQXOV0FljUGxM5-XsIIBkt4AHfAdMe");
        
        // authenticate as the client (don't log in as anyone)
        await client.AuthenticateAsync();

        string gfycatId;
        while (true)
        {
            Console.WriteLine("Please enter a video URL to get and turn into a gfycat: ");
            string url = Console.ReadLine();
            try
            {
                gfycatId = await client.CreateGfycat(url);
                Console.WriteLine($"Success! Your gyfcat will have the Id {gfycatId}");
                break;
            }
            catch (GfycatException e)
            {
                Console.WriteLine($"Invalid input: {e}");
            }
        }

        // now we can wait...
        Console.Write("Encoding...");
        GfyStatus status;
        do
        {
            status = await client.CheckGfyUploadStatus(gfycatId);
            await Task.Delay(TimeSpan.FromSeconds(status.Time));
        }
        while (status.Task == Status.Encoding);
        gfycatId = status.GfyName; // if an equal MD5 hash of the upload file is detected, gfycat will send us back the already existing gfycat name
        
        // get the gfycat data
        Gfy resultGfycat = await client.GetGfy(gfycatId);

        // do whatever you want with it, as an example we'll download the webm file
        Console.WriteLine("Downloading gfycat as webm...");
        using (HttpClient webClient = new HttpClient())
        using (FileStream file = File.OpenWrite(gfycatId + ".webm"))
        {
            Stream downloadStream = await webClient.GetStreamAsync(resultGfycat.WebmUrl);
            await downloadStream.CopyToAsync(file);
            file.Flush();
            downloadStream.Dispose();
        }
    }
}