# Intro
## Packages
There are two libraries in the Gfycat.Net project: Gfycat.Net and Gfycat.Net.Analytics.
 * Gfycat.Net is the base library, it's the wrapper around the main API. You can read about the main API here: https://developers.gfycat.com/api
 * Gfycat.Net.Analytics is the analytics wrapper. It wraps around the Gfycat analytics and impression API. You can read about the API here: https://developers.gfycat.com/analytics/

## Installing
Gfycat.Net has two package sources: NuGet and MyGet
 * NuGet contains the official release packages. It's pushed to when AppVeyor builds from master branch. It's the version of the library which builds the docs.
 * MyGet contains the developer release packages. It contains new features which are stable however not fully tested.

#### NuGet
The standard release can be installed like any other package. Go to the "Manage NuGet Packages" page for you project and searh for and install "Gfycat.Net". Gfycat.Net.Analytics can be install the same way.

#### MyGet
MyGet requires a bit more work, to get dev builds, you must add the MyGet feed as a package source in Visual Studio.

  1. First, go to Tools -> Options -> NuGet Package Manager and select Package Sources.
  2. Click the green plus button in the top right to add a new source
  3. Near the bottom of the window, change the "Name" to something memorable, and change the "Source" to "https://www.myget.org/F/gfycat-net/api/v3/index.json"
  4. Click OK, then open your project's packages window and change the "Source" dropdown to the package source name you defined before.
  5. Search for Gfycat.Net like normal with "Include prereleases" checked

## Basics
#### GfycatClient
The GfycatClient contains all the main methods used get users, gfys, and feeds. It is constructed with either a GfycatClientConfig or a client ID and secret strings. You can get these on the [Gfycat developer site](https://developers.gfycat.com/signup).

With client ID and secret
```csharp
using Gfycat;
...
GfycatClient client = new GfycatClient("clientId", "clientSecret");
```

With client config
```csharp
using Gfycat;
...
GfycatClientConfig config = new GfycatClientConfig("clientId", "clientSecret");
// make changes to client's default values
GfycatClient client = new GfycatClient(config);
```

#### Authentication
All authentication falls under the method `AuthenticateAsync`. Depending on the provided parameters, different authentications are run. All method overloads and parameters are [documented here](https://obsidianminor.github.io/Gfycat.Net/api/Gfycat.GfycatClient.html#Gfycat_GfycatClient_AuthenticateAsync_Gfycat_RequestOptions_) and all authentication options explained on [the Gfycat docs](https://developers.gfycat.com/api/#authentication).

In the event you get a 401 (unauthorized) status code, by default the client will try to refresh the access token before retrying the request. If the refresh token is somehow null or invalid (by leaving the program running doing nothing for 6 months) or implicit browser authentication was used, the access token will not be refreshed and the request will fail. If you want to refresh the access token yourself, you can by calling `await RefreshTokenAsync()`. If you want to save the refresh token for later, you can use the property GfycatClient.RefreshToken to get the current refresh token. To reuse the token again, run `await RefreshTokenAsync(refreshToken)` using the saved token.

#### Feeds
Feeds in Gfycat.Net allow enumeration through large collections of items in batches. They are all defined by the interface IFeed<T> where T is the type of item in the feed. This can be a Gfy or another feed. IFeed<T> inherits IAsyncEnumerable<T> which means you can use LINQ on whole
