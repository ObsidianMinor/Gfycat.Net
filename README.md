# Gfycat.Net
[![Gfycat.Net](https://badge.fury.io/nu/Gfycat.Net.svg)](https://badge.fury.io/nu/Gfycat.Net) 
[![Build status](https://ci.appveyor.com/api/projects/status/ker3kcw9oht360lf/branch/master?svg=true)](https://ci.appveyor.com/project/ObsidianMinor/gfycat-net/branch/master)
[Join us on Discord](https://discord.gg/spvBGyn)

An unofficial wrapper around the Gfycat API written for .NET Standard 1.2

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

### Using Analytics
[![NuGet version](https://badge.fury.io/nu/Gfycat.Net.Analytics.svg)](https://badge.fury.io/nu/Gfycat.Net.Analytics)

Gfycat.Net also supports the analytics and impression endpoints under the project "Gfycat.Net.Analytics" or namespace "Gfycat.Analytics".

Creating an analytics client
```csharp
using Gfycat.Analytics;
...
string userTrackingCookie = GfycatAnalyticsClientConfig.GenerateCookie(); // not required
GfycatAnalyticsClient client = new GfycatAnalyticsClient("replace_with_app_name", "replace_with_any_app_identification", replaceWithAppVersion, userTrackingCookie); // the cookie may also be null
```
Session cookie will be auto generated, or you can provide one in a GfycatAnalyticsClientConfig
