# Gfycat.Net
An unofficial .NET wrapper around the Gfycat API

## Progress:
Finishing up, adding documentation, running tests, writing samples.

## FUTURE:
Add support for analytics endpoints (maybe namespace Gfycat.Analytics?)

### Unimplemented features: 
1. Adding and removing links to provider accounts
2. Adding, removing, and updating the current user's geo and domain whitelists
3. Secret endpoints not covered by documentation (like fetching, creating, and modifying API keys)

### Upload Gfy Example
```csharp
using Gfycat;
...
GfycatClient client = new GfycatClient("replace-with-clientid", "replace-with-clientsecret");
await client.Authentication.AuthenticateClientAsync(); // authenticate without a user as the client

string gfy = await client.CreateGfyAsync(System.IO.File.Open("somevideo.mp4")); // upload a video, get back a name

GfyStatus status = await _client.CheckGfyUploadStatusAsync(gfyId); // use the name to get the status
while(status.Task == Status.Encoding)
{
  await Task.Delay(TimeSpan.FromSeconds(status.Time));
  status = await _client.CheckGfyUploadStatusAsync(gfyId);
}

if (status.Task == Status.Invalid || status.Task == Status.NotFoundo)
  // well shit, it broke :( do something about it
    
Gfy yourGfy = await client.GetGfyAsync(status.GfyName);
// congrats, you now have a gfy
```
