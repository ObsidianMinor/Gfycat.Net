# Gfycat.Net
An unofficial wrapper around the Gfycat API written for .NET Standard 1.2

## Progress:
Adding documentation, running tests, writing samples.

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
GfycatClient client = new GfycatClient("replace_with_your_client_ID", "replace_with_your_client_secret"); // client authentication happens during first 401

GfyStatus gfyStatus = await client.CreateGfyAsync(File.Open("somefile", FileMode.Open));
Gfy completedGfy = await gfyStatus.GetGfyWhenCompleteAsync();
// congrats, you now have a gfy
```

### Feed guidelines
Gfycat.Net uses the magic power of IAsyncEnumerable from Rx.Net. Using its magic it's extremely easy to accidentally loop through a whole feed of gfys (there's a lot of gfys out there so it might take a while and will probably get you banned from using the API).
To prevent this, read feeds like this:
```csharp
IAsyncEnumerable<Gfy> gfySearchFeed = await client.SearchAsync("stop");
gfySearchFeed.Take(20).ForEach(gfy => ...);
```
or
```csharp
IFeed<Gfy> gfySearchFeed = await client.SearchAsync("stop");
foreach(Gfy gfy in gfySearchFeed.Content)
{
  // do something
}
IFeed<Gfy> nextPageGfySearchFeed = await gfySearchFeed.GetNextPageAsync();
...
```
and not
```csharp
IAsyncEnumerable<Gfy> gfySearchFeed = await client.SearchAsync("stop");
gfySearchFeed.ForEach(gfy => ...);
```
