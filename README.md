# Gfycat.Net
An unofficial wrapper around the Gfycat API written for .NET Standard 1.2

## Progress:
Finishing up, adding documentation, running tests, writing samples.

## FUTURE:
Add support for analytics endpoints (maybe namespace Gfycat.Analytics?)

### Compatible frameworks (if you don't know the table)
* .NET Core 1.0 (and up)
* .NET Framework 4.5.1 (and up)
* Universal Windows Platform 10.0
* Universal Windows 8.1
* Universal Windows Phone 8.1

### Unimplemented features: 
1. Secret endpoints not covered by documentation (like fetching, creating, and modifying API keys)

### Upload Gfy Example
```csharp
using Gfycat;
...
GfycatClient client = new GfycatClient("replace-with-clientid", "replace-with-clientsecret");
await client.Authentication.AuthenticateClientAsync(); // authenticate without a user as the client

string gfyId = await client.CreateGfyAsync(System.IO.File.Open("somevideo.mp4")); // upload a video, get back a name

GfyStatus status = await _client.CheckGfyUploadStatusAsync(gfyId); // use the name to get the status
while(status.Task == Status.Encoding)
{
  await Task.Delay(TimeSpan.FromSeconds(status.Time));
  status = await _client.CheckGfyUploadStatusAsync(gfyId);
}

if (status.Task == Status.Invalid || status.Task == Status.NotFoundo)
{
  // well shit, it broke :( do something about it
}
    
Gfy yourGfy = await client.GetGfyAsync(status.GfyName);
// congrats, you now have a gfy
```
