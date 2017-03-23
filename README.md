# Gfycat.Net
An unofficial wrapper around the Gfycat API written for .NET Standard 1.2

## Progress:
Finishing up, adding documentation, running tests, writing samples.

## FUTURE:
Add support for analytics endpoints (maybe namespace Gfycat.Analytics?)

### Unimplemented features: 
1. Secret endpoints not covered by documentation (like fetching, creating, and modifying API keys)

### Compatible frameworks (if you don't know the table)
* .NET Core 1.0 (and up)
* .NET Framework 4.5.1 (and up)
* Universal Windows Platform 10.0
* Universal Windows 8.1
* Universal Windows Phone 8.1

### Upload Gfy Example
```csharp
using Gfycat;
...
GfycatClient client = new GfycatClient("replace_with_your_client_ID", "replace_with_your_client_secret");
await client.AuthenticateAsync(); // gfy actions such as creating and fetching don't require that a user logs in

string gfyId = await client.CreateGfyAsync(File.Open("somefile", FileMode.Open));

GfyStatus status = await client.GetGfyUploadStatusAsync(gfyId);
while(status.Task == UploadTask.Encoding)
{
    await Task.Delay(TimeSpan.FromSeconds(status.Time));
    await status.UpdateAsync();
}

if (status.Task == Status.Invalid || status.Task == Status.NotFoundo)
{
  // well shit, it broke :( do something about it
}
    
Gfy yourGfy = await status.GetGfyAsync();
// congrats, you now have a gfy
```
