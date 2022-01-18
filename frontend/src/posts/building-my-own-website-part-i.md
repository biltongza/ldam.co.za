---
title: 'Building my own website: Part 1'
date: 2022-01-18
excerpt: 'How I built my own website: rationale and architecture'
tags: 
    - meta
    - tech
    - azure
---

### The problem 
I've had my own website since October 2016. I never really thought much of it, I just wanted somewhere to put my photography that wasn't social media so I could have full resolution versions more easily available. 

I used [Squarespace](https://www.squarespace.com/), since their templates were very pretty, and it meant I didn't need to manage any infrastructure. It was quick and painless. I also had (at the time) what I thought was a pretty cool domain: logans.camera. The domain cost me $50/yr, and Squarespace's hosting cost $144/yr, and I thought this was perfectly justifiable for the quality of the site that I got. I didn't need to worry about making my own design, it was just about making my photos the prime focus.

![My old website as at 2022-01-18](/assets/ldamcoza.png)

Fast forward to 2021, I haven't updated my site in months, because getting my photos out of [Lightroom](https://www.adobe.com/africa/products/photoshop-lightroom.html) and into Squarespace is just a chore (their image uploader didn't handle bulk uploads with large images particularly well, nevermind having to manually export photos). Me being the developer I am, I decided I would look for a way to automate it.

### Investigation
My first thought was to try automate the upload process, since that was the part I was having trouble with, but Squarespace don't offer a public API to upload images, so that idea was a no go. That was the first time I thought about making my own site, but it did nothing to satiate my desire to automate things.

That prompted me to do some research on Lightroom and whether it has any APIs I could use to automate the export. As it turns out, [it does](https://www.adobe.io/apis/creativecloud/lightroom/apidocs.html)!

Adobe exposes a means to:
  - Retrieve albums
  - Retrieve metadata about images within albums, and
  - Retrieve _rendered_ images

_(this is all much more than I expected Adobe to provide, given that I've not seen any public integrations with Lightroom)_

### Prototyping

Now that I knew that there was some possibility of automating part of my workflow, it was time to start experimenting to see how it all worked together.

Finding and grokking Adobe's documentation turned out to be quite the challenge. It took me a few months of late night/weekend hacking just to figure out how to get [authentication](https://www.adobe.io/apis/creativecloud/lightroom/docs.html#!quickstart/oauth.md) working. It seemed very simple on the surface, but between my lack of knowledge on OAuth, and figuring a) which scopes I needed and b) how these scopes needed to be presented to Adobe [which appears to be non-standard](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/pull/573#discussion_r663479205), it turned out to be quite the challenge.

I started out just using [Postman](https://www.postman.com/) to try call the APIs and see what the responses looked like, because the [documentation](https://www.adobe.io/apis/creativecloud/lightroom/apidocs.html#/assets/getAsset) isn't particularly clear on the data model.

Once I had authenticated and successfully called some APIs in Postman, it was time to finally write some code. My natural choice was .NET, since I'm quite fond of C#, and I had a few chats at work about [Blazor WebAssembly](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) and I wanted to see what that was all about. At the time, it was still in fairly early days, running on .NET 5.

I had a pretty grandiose idea that I'd be able to build a nice little client library implementation of the Lightroom APIs. It seemed like a simple enough task, since Adobe publishes a [Swagger doc](https://github.com/AdobeDocs/lightroom-partner-apis/blob/master/docs/api/LightroomPartnerAPIsSpec.json) so it should be trivial to use [NSwag](https://github.com/RicoSuter/NSwag) to generate a client library for me. That was wishful thinking, since at the time of writing, that spec is not quite standard compliant (see [1](https://github.com/AdobeDocs/lightroom-partner-apis/issues/159), [2](https://github.com/AdobeDocs/lightroom-partner-apis/issues/160)) which left NSwag quite unhappy, and I decided that trying to fix this JSON that I don't own and don't fully understand was not the path I wanted to take.

I decided to hand roll my own solution instead (which you can see [here](https://github.com/biltongza/ldam.co.za/tree/master/lib/Lightroom)) by observing what the API responded with, and cherry picking the bits that I was interested in. 

### Build
Once I understood the API integration, I needed to figure out some architecture. I had a means to get information and images from Adobe, and I needed some way to show it to people. 

Since Adobe don't publish any way of receiving events or any other pub/sub sort of architecture, I was going to have to resort to polling. On my current project I've been exposed to serverless hosting in the form of [Azure App Services](https://azure.microsoft.com/en-us/services/app-service/) and [Azure Functions](https://azure.microsoft.com/en-us/services/functions/) which I've grown quite fond of. I really like the idea that I only have to care about my application, and managing the hosting environment is not even a thought in my mind.

The task was fairly simple in my head, I could use a serverless function to synchronize my images and metadata to some sort of store, and then build a static website that rendered contents based on the synchronized data.

So, fully intending to stick to the serverless path, I built an Azure FunctionApp to poll Adobe and monitor for changes to a configured album in my Lightroom catalogue, and synchronize the rendered images and a manifest file containing the metadata about the images to an Azure Storage Account.

One problem that I needed to solve no matter what platform I chose was that Adobe's authentication process for Lightroom is entirely focussed on existing in the browser. There's no way of using a [service account](https://www.adobe.io/developer-console/docs/guides/authentication/JWT/) to authenticate with the API, so I was going to have to capture the access token in the authentication process and store it for the function to use. 

That's exactly what I wound up doing (and along the way I implemented the [AspNet.Security.OAuth.AdobeIO](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/pull/573) package). I needed to set up some sort of browser based application that would authenticate with Adobe, then store the access token in Azure Keyvault so I could later use that token in my functions (which is probably _completely_ against their policies, but they haven't noticed me yet!).

Not long after that I had my images and metadata in my storage account. Now to make it pretty!

The static site just needed to fetch the manifest out of storage (which I expose via a CDN, which comes free with storage accounts), then render the images on the client side based on that, just like any SPA would typically render stuff based on an API call.

As I alluded to before, I wanted to try out Blazor WebAssembly. While it was _completely_ overkill for what I needed, it satisfied the SPA requirement, and it was a fun way to experiment with some new tech. Also, I could reuse code between my Azure functions and the client. Plus, Azure had just released their [Static Web App](https://azure.microsoft.com/en-us/services/app-service/static/) service which I could use to publish my static site and have it hosted for me. No servers! Woo!

It all worked just fine on a desktop browser, with a nice fast internet connection, _but_ my initial bundle was 3MB, which is larger than the initial bundle of the entire transactional website I work on at my current client. _For a simple photography portfolio!_ It also took upwards of 20 seconds for Blazor to boot on my phone (a OnePlus Nord), even with AOT compilation and trimming enabled. 

That just wasn't acceptable for me, so I searched for an alternative. My current day to day work is Angular, so that was out. I've used React on a side project (that I would love to talk about here one day!) and I'm not the biggest fan of it. I then found Svelte, and decided it was different enough and yet still in the same sort of paradigm of "web frameworks" that I'd be comfortable with to give it a try. 

Svelte has a SPA mode (which works quite well I might add), and I wound up with around a 40kb bundle using it. _Sweet! Everything sorted then!_ I just needed to port what I had done in Blazor over to Svelte. It took an afternoon to get a functionally equivalent website up.

Since then, I've taken a bit of an interest in SEO, and while Google [does index SPAs](https://developers.google.com/search/docs/advanced/javascript/javascript-seo-basics), server side rendering is the "correct" way to go about this, instead of relying on hoping that search engines can render my website correctly in whatever rendering environment they use.

Svelte encourages the use of SSR, and paired with the Azure Static Web App supporting Azure Functions for an "API" and a [community made adapter](https://github.com/geoffrich/svelte-adapter-azure-swa), that is probably how you are reading this (unless you are from the future and I've changed my mind!).

### Wrapping up
If you've got this far, congratulations! It took me a lot longer to get here than I expected, but I am glad you are with me. 

I intend to do a few more posts like this, delving into more detail of the challenges I had and what I did to resolve them.

I'd also love feedback! You can get hold of me via Twitter [@thebiltong](https://twitter.com/thebiltong).

Thanks for reading :)
