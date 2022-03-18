---
title: 'Building my own website: Part 3'
date: 2022-03-16
excerpt: 'How I built my own website: syncing images & the fun of enumerables'
tags:
  - meta
  - tech
  - azure
  - dotnet
  - adobe
---

### Syncing images

As I've alluded to before, I'm quite lazy. This entire ordeal started as a way for me to be lazy about uploading images (truthfully there's been some scope creep since then 🥲).

This post will be a bit of a deep dive on how the image synchronization process works.

#### Lightroom History

If you're not familiar with Lightroom, it is Adobe's image editing and organization solution. There are two versions: Lightroom Classic, and Lightroom CC. Lightroom Classic has been around for ages, [originally introduced back in 2007](https://en.wikipedia.org/wiki/Adobe_Lightroom#Version_1.0) and is still updated [to this day](https://en.wikipedia.org/wiki/Adobe_Lightroom#Adobe_Lightroom_Classic_CC_(version_8.0+)). Lightroom CC (aka just Lightroom these days) is a ground-up cloud-based rewrite of Lightroom (it also has a mobile version!).

The major difference these days is Lightroom Classic is much more suited for batch editing, and many professionals still prefer it for that fact. It took quite a while to reach more or less feature parity (there's still some stuff that Classic is better at doing, like image stacking) which left a lot of people (including myself) asking, why bother switching?

Lightroom Classic has a cloud sync feature, but it only synchronizes specific images that you select, and only what Adobe calls "Smart Previews", which are not full resolution images, but smaller versions that are close enough to get most of the editing job done. Lightroom Classic works entirely with local copies of your images, and expects them to be there all the time. If you're not aware, RAW image files are _large_. The images I work with are regularly 30-35mb per image; fancier cameras get much larger! This adds up to several hundreds of gigabytes of images over the years. I'm currently at about 25000 images taking up 550GB of storage. Keeping that much storage online all the time kinda sucks. 

Lightroom CC instead takes advantage of ubiquitous internet connectivity and prefers to keep _all_ of your images on their cloud servers instead, and only pull down images to work with temporarily while you're editing them. It keeps a small cache of full size RAWs and swaps them in and out pretty seamlessly.

I eventually decided to take the plunge because I wanted to take advantage of the cloud features of the new platform (mainly, to have solid synchronization between my PC and my phone). 

#### Organization is key

Since all my images are now synchronized to Adobe, and Adobe exposes an API to interact with their services, all I need to do is make sure they're organized in a way that makes it simple to interact with.

Lightroom lets you create an organizational hierarchy of Folders and Albums. Folders can contain 0 or more other Folders or Albums, while Albums may contain zero or more images. An image may also be present in multiple Albums at any given time.

In the API, the root organizational unit is the Catalog. Everything else is stored within the Catalog. The Catalog can contain zero or more Assets (which are images for my purposes) as well as zero or more Albums (which _link_ to zero or more Assets).

Each of these kinds of objects have an ID associated with them which is how you reference them in the API.

So for my simple purposes, I just need a single album containing the images I want to synchronize, and then ask the API for the image IDs within that Album, then ask the API for an image stream for each of the images to pull them.

#### Abusing enumerables for fun and profit
If I were to do this process synchronously, I would either need a bunch of temporary storage (probably in memory), or I would need a lot of time. Neither of these are conducive to a serverless environment where those are the exact two aspects that push the price up.

I decided I would take advantage of a really (in my opinion) underused feature of C#: enumerables and deferred execution.

By chaining together multiple enumerables together, you can set up an almost pipeline-like approach. By this I mean, you can have data "stream" directly from one part of a process to another before you've even read everything from your source. You're no longer bound to having distinct "phases" of typical processing, like buffering all of your source data ahead of time, then putting all that data through the next phase or stage of the process, and then on to the next one etc (like a batch process). Instead, each item or record can go through the entire process one by one, and you can have very quick results coming out the other end with a much smaller total memory footprint.

Entity Framework has been teaching us this for years, because it exposes all of its data as enumerables, and its deferred execution nature frequently catches out newer developers (if I see another `.ToList()`...). I rarely see any _other_ use of this technique outside of EF though, and I think that's a shame. I am a massive fan of LINQ, so I will use it at any chance I get. I find it so descriptive and expressive, and deferred execution lets us use it much more _efficiently_ than if we had to buffer our entire workset in memory.

_I last wrote about enumerables in 2017 in an internal blog at Entelect, which I should definitely get published on here..._

#### Recap: IEnumerable and IAsyncEnumerable

If you aren't well versed in deferred execution, here's a TL;DR verison:
- C# has a language keyword called `yield`
- Using `yield` causes the compiler to rewrite your code into a state machine
- The state machine can return something and remember the "position" in your code, so it can pick up where it left off next time it is called
- This means you can return something, do some other stuff, then come back later and return something _again_
- Calling code can consume the returned data and depend on the runtime to handle the called code "remembering" where it is
- You can exploit this to process data iteratively.

How does that relate to `IEnumerable` or `IAsyncEnumerable`? It actually doesn't. These are simply helper interfaces to make sure your code is structured in a way that the compiler can use to wire this all up.

The `foreach` feature of C# is not magic, it depends on the object you're trying to enumerate over having a specific structure.
The compiler makes use of duck typing to make this work. That means as long as your object _looks_ like it can enumerate, it should work with `foreach`. The only conditions for this specific duck type is your object must expose a `Current` property, indicating the current value of the enumerator, and a `MoveNext()` method, which should advance the enumerator forward and change the `Current` property.

This is the exact structure of the [`IEnumerator`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerator?view=net-6.0) interface, and that is the only thing that the [`IEnumerable`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=net-6.0) interface requires you to be able to return.

So, if one were to make a method that uses `yield` (which we now call an _Iterator_), it only makes sense to make that method return an `IEnumerable` so the compiler can implement this structure for you with its magic state machine. Indeed, that is one of the requirements to use [`yield`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/yield#iterator-methods-and-get-accessors).

In other words, `yield` and `foreach` go together like butter on toast.

`IAsyncEnumerable` does exactly the same thing, but brings `async` support with it, so you can do `await foreach` on them.

#### Putting it all together

Getting this all to work needs every processing step to be aware of it. The nice part of all of this being on GitHub is I can just link to the source to show you an example 😊

First we have to start at the bottom, down at the "data" layer. The bit that talks to Lightroom.
As an example, [here](https://github.com/biltongza/ldam.co.za/blob/ccf798bf7db2ced459b223ade38a82163c95d35f/lib/Lightroom/LightroomClient.cs#L83) is the call to get the Assets (images) within an Album:

```csharp
public async Task<AlbumAssetResponse> GetAlbumAssets(string catalogId, string albumId, string after = null)
{
    var builder = new StringBuilder();
    builder.Append("v2/catalogs/");
    builder.Append(catalogId);
    builder.Append("/albums/");
    builder.Append(albumId);
    builder.Append("/assets?embed=asset;spaces;user&subtype=image");
    if (!string.IsNullOrWhiteSpace(after))
    {
        builder.Append("captured_after=");
        builder.Append(after);
    }
    var request = await PrepareRequest(HttpMethod.Get, builder.ToString());
    var response = await httpClient.SendAsync(request);
    var result = await HandleResponse<AlbumAssetResponse>(response);
    return result;
}
```

`PrepareRequest` simply adds an auth token to the request headers. `HandleResponse` strips the `while (1) { }` header that Adobe sticks on all their responses and then deserializes the content of the body using `System.Text.Json`'s newfangled source-generated optimizations.

Nothing fancy here, just making a standard HTTP API call, and deserializing it asynchronously (save for applying a regex on the content to strip out the `while` header).

[Next step](https://github.com/biltongza/ldam.co.za/blob/ccf798bf7db2ced459b223ade38a82163c95d35f/fnapp/Services/LightroomService.cs#L42) is to do something with the Lightroom data. In my case, I extract the metadata I'm interested in to use later.

```csharp
public async IAsyncEnumerable<ImageInfo> GetImageList(string albumId)
{
    string after = null;
    do
    {
        var albumAssetResponse = await lightroomClient.GetAlbumAssets(this.catalog.Value.Id, albumId, after);
        Link next = null;
        albumAssetResponse.Links?.TryGetValue("next", out next);
        var afterHref = next?.Href;
        after = !string.IsNullOrWhiteSpace(afterHref) ? afterHref[afterHref.IndexOf('=')..] : null;
        foreach (var asset in albumAssetResponse.Resources)
        {
            var make = asset.Asset.Payload.Xmp.Tiff.Make;
            var model = asset.Asset.Payload.Xmp.Tiff.Model;
            yield return new ImageInfo
            {
                // metadata goes here
            };
        }
    }
    while (!string.IsNullOrEmpty(after));
}
```

The Lightroom API returns data in pages. This code is checking the response for a `next` link, and then requesting it when it's reached the end of the current page. _Doesn't this sound familiar from earlier?_ We're using `yield` here to return each image's metadata individually, and we loop until we have no more images left.

So when we do a `foreach` over this, we'll make the initial API call, loop over the images until there's nothing left in the current page, then run back around and make another API call for the next page. Repeat until we have no `next` page left. But all of this is invisible to the calling code, as far as it is concerned, it will just get the next image whenever it's available.

How does this look in actual code? [Like this.](https://github.com/biltongza/ldam.co.za/blob/ccf798bf7db2ced459b223ade38a82163c95d35f/fnapp/Services/SyncService.cs#L89)

```csharp
var imageInfos = lightroomService.GetImageList(album.Key);
await foreach (var batch in imageInfos.Buffer(10))
{
    foreach (var imageInfo in batch)
    {
        // sync image here
    }
}
```
_some code has been omitted for brevity_

You can see here that I'm using `await foreach` on the results of my `GetImageList` call shown earlier.

This isn't a strictly clean implementation, but I think it is representative of a real world use case. I'm making use of [System.Interactive.Async](https://github.com/dotnet/reactive) (aka Reactive extensions for .NET) to group images into batches of 10 (a number chosen with about 5 minutes of testing and deciding it was "good enough"), and then looping over each batch. Without this, things were kind of slow, which is pretty normal when you aren't doing stuff in batch, because small overheads can start to add up. _I used this as an opportunity to sync images in parallel!_

Within this loop, the actual code goes and pulls the rendered image from Lightroom, updates the in-memory copy of the image manifest, then stores the image to a storage account (which has an Azure CDN endpoint sitting on it). Job done!

#### What's the point?

The important part is that none of the consuming code _actually_ cares what is going on under the hood. It just knows that it may or may not get another _thing_, and that _thing_ is its only input. It is all it needs to care about.

I also don't have all of my source data in memory at any given point. It is only in scope as and when it is needed. 

I see this as a modern, language integrated method of constraining your peak memory usage (or at least, that's what I've used it for in most cases!).

#### But what about buffering?

You can see quite clearly in my previous example that it is entirely possible to still do things in batches or do buffering by taking this approach. Buffering is generally A Good Thing™️. It's making use of any temporary storage you have to speed up access to resources you know you are going to need soon, and doing things iteratively seems to imply that you trade that off for the ease of working with a single item at a time. This does not have to be the case.

You still have ways of controlling how your data is accessed. I've used this technique to implement a fairly trivial wrapper over ADO.NET to instead "stream" records from the database through the application's processing. The end location for the data was right back in the database, and doing single item inserts via ADO.NET is painfully slow (_because overheads add up_). The limiting factor at the time was memory usage, we were regularly bumping up against 32 bit .NET Framework's 2GB limit and crashing with OOMs. Pulling 30,000 records 100+ field records from a database into a `DataSet`, combining them with binary image data, and then writing the results out into a single blob all at once simply didn't work. So I used enumerables to instead break it into 500 record chunks (which was configurable), and split out the blobs like that, batching them together to write them back into the database in chunks. I then changed the consuming application to expect the blobs to be split out, and recombine them when writing out the final product.

#### Caveats

Of course, this design is not without its problems. As I mentioned before, doing stuff iteratively causes overheads to start to build up, and it can take a bit of trial and error to figure out the correct "tuning" of batch sizes, and those batch sizes might not transfer to other environments.

Further, it's very easy to completely break the process by throwing in a careless `.ToList()`. By doing that, you're asking the runtime to get every result and stick it in a list, completely blocking until that is done. You can `foreach` over the resulting list all the same, so it can look completely innocuous to the novice developer, but it completely defeats the purpose of doing any of these changes in the first place. It takes a bit of practice to remember that you are in the middle of a much bigger picture in your foreach loop than you may initially realise.

Another drawback I can think of, your application can be prone to latency spikes and it can be hard to see exactly where they happen without debugging tools. If any link of the consuming chain is suddenly slow for whatever reason, you effectively stall the following links from doing anything useful.