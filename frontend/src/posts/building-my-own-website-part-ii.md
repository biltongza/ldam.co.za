---
title: 'Building my own website: Part 2'
date: 2022-01-27
excerpt: 'How I built my own website: serverless is cool!'
tags: 
    - meta
    - tech
    - azure
---
### Serverless is cool!

#### Paradigms and buzzwords
The serverless buzzword is quite a misnomer. The current web does not exist without servers. There is always some sort of system that acts as a host, providing resources to clients that request them. 

The serverless paradigm removes the management part of all those servers and provides the developer with a means to execute code without having dedicated/assigned hardware (and all the associated admin that comes with that hardware), nor the "server" software package (running and maintaining an operating system, network stack, web servers like nginx/IIS, load balancers, etc). 

Most serverless computing providers give you a managed runtime for simple functions, and provide a few basic input/output points that your functions can be triggered by or write output to. 

These functions exist completely isolated from one another (save for some more complicated models, like durable functions), which gives us a few benefits. Since they're isolated, they are inherently less coupled, and there's (usually) less state to reason about. This isolated nature of functions also gives you the opportunity to execute them in a distributed manner. 

Going serverless is moving away from the idea of "reserving" compute power. In a typical server platform, you typically have a finite amount of compute power in the form of physical (or virtual) machines. This computer power can only be changed by either "scaling up" (beefing up the hardware resources) or "scaling out" (allocating more instances of machines). Whether you are using 0% of that compute power or 100% of it, the cost of it is the same (excluding electricity usage of course), hence we can say that it is effectively reserved compute power.

The serverless model instead gives you compute power on demand without any form of upfront reservation. If your functions aren't executing, you aren't paying for them. They are by and large billed by how much *memory* and *time* your functions use (at least Azure does, other providers might do things differently).  

#### What's this got to do with me?

A core idea while building this website was that I wanted nothing to do with managing servers, because a) I'm lazy and b) I'm cheap. I do not expect this site to get a lot of traffic, so it's a bit hard to justify the monthly cost of a server running all the time. 

From the beginning I had envisioned an entirely client rendered single page app. I figured this was a reasonable choice, given how prevalent SPAs are and how good the frameworks are. Also, having client rendering means my server can be pretty dumb (and cheap).

Typically for a SPA you'll have some sort of API that it uses to get or modify resources. Given that I don't want a server, that means I don't want an API either. This is a perfectly reasonable choice for the content of my site. My photos aren't updating in realtime (in fact, at the moment they're not updating in months, but that's because I haven't curated what I want to share in a while). I can simply have a file somewhere describing to clients what to render and where to get the pictures from. The client can then fetch that file and generate the corresponding HTML for it.

I just needed a way to produce that file and keep it up to date.

As I wrote about in my [previous post](./building-my-own-website), [Azure Functions](https://azure.microsoft.com/en-us/services/functions/) was a pretty natural choice. It is simply Microsoft's implementation of functions-as-a-service, and it is certainly not the only one out there (GCP has [Cloud Functions](https://cloud.google.com/functions), AWS has [Lambda](https://aws.amazon.com/lambda/), Cloudflare has [Workers](https://workers.cloudflare.com/), to name a few). I'm just sticking with Azure because it's what I know (although I must give credit to [Troy Hunt]((https://www.troyhunt.com/serverless-to-the-max-doing-big-things-for-small-dollars-with-cloudflare-workers-and-azure-functions/)) for planting the serverless seed in my head in 2018)

I'm making use of a simple timer trigger for my image sync function, with the timer set to trigger on every half-hour. I'm making use of a manifest to determine what to show on the site. It's just a simple JSON file with metadata and relative URLs to images. Every invocation of the function first tries to see if this manifest file exists in a blob storage account. If it does exist, it'll check the last modified time of each image in the manifest and see if it is outdated according to the last modified time in Lightroom (if the manifest does not exist, everything is assumed out of date). If it is out of date, it'll pull a render of the image from Lightroom and store it in the same blob storage account, otherwise the image is skipped and it moves on. If any modifications to the manifest need to be made, a flag is set, and the manifest is modified in memory. At the end of all these checks, it will check the flag, and store the updated manifest back into the blob storage account.

That's basically it. The manifest and the images are now in a blob storage account, which are exposed to the public internet via [Azure CDN](https://azure.microsoft.com/en-us/services/cdn/).

Now the client SPA just needs to fetch that manifest, and render everything it describes in a pretty way. As I have previously mentioned, the first iteration was done in Blazor WebAssembly, which I hosted on an [Azure Static WebApp](https://azure.microsoft.com/en-us/services/app-service/static/).

#### Return of the server

When I decided that the cold startup time for Blazor WebAssembly was just too long (especially since it was complete overkill, I just wanted it to render some images), I ported the site over to SvelteKit. I don't really have a good reason for picking SvelteKit, other than it wasn't Angular, Vue, or React. I like that it tries to do as much of the heavy lifting as it can as part of the compile step, instead of leaving that to happen at runtime, although it's not the only one to do it (see [Nuxt](https://nuxtjs.org/) for the Vue version, and [Remix](https://remix.run/) for the React version).

It was a simple ordeal, at the time I only had the homepage and image pages, so it took an afternoon to completely move everything over (you can see the delete of the Blazor site [here](https://github.com/biltongza/ldam.co.za/commit/6430b46a265c269097f1da06116d634f5206d1fe)).

SvelteKit is interesting because it is primarily designed for server side rendering, although it can function in [SPA mode](https://github.com/sveltejs/kit/tree/master/packages/adapter-static#spa-mode). I was initially very happy with keeping it in SPA mode, because it provided me with the templating abilities that I wanted, while having a fairly small footprint. It was, again, complete overkill in SPA mode, and I wasn't using SvelteKit the way it was designed to. 

I happened to come across a SvelteKit adapter for Azure Static WebApps, [svelte-adapter-azure-swa](https://github.com/geoffrich/svelte-adapter-azure-swa). It's quite clever, but also quite simple. SvelteKit is designed to be hosting provider agnostic, so there are lots of adapters for many different hosting providers. All the adapters really need to do is provide an entry point to the compiled Svelte routes and endpoints. Static WebApps provide a managed instance of Azure Functions to act as an API, so the svelte-adapter-azure-swa adapter simply produces a function with an HTTP trigger that forwards the request on to the already compiled Svelte application.

After a bit of hacking over Christmas 2021, I now have server side rendering, but without any servers. Dope!

#### At what cost?

The only thing this entire site has cost me (other than the domain registration) is my own time that I would otherwise be spending playing games or watching YouTube videos. 

I'm not joking, it has cost me less than R5 since I started it all.
![Azure bills](/assets/azure-billing.png)

In the most expensive month so far, KeyVault cost me 38 cents, while the remaining portion of the R1.89 (excl VAT) was accrued by Storage Account writes and CDN usage. Static WebApps have a free tier that is more than enough for my needs, and the rest of this costs so little it is entirely inconsequential. In fact I'm pretty sure the cost of my service usage is lower than the cost of actually billing me.

#### Final word

This is just one example of using the many tools available to me as a developer. I evaluated my needs, and decided that the serverless route would make sense for what I wanted to accomplish. It's certainly not the only way, in fact Azure also offers a free tier for App Services, which could have done exactly the same thing with a traditional server platform (though app services do remove a large part of the annoying server bits too!).

Going serverless certianly has its fair share of downsides, as discussed briefly by the [Wikipedia article](https://en.wikipedia.org/wiki/Serverless_computing). Debugging is certainly one of them. I currently have an issue hitting out of memory errors on the deployed FunctionApp instance, but I can't reproduce it locally, and there's just no way to reproduce the Azure environment 100%.

But it's all certainly good enough for me, and I'll certainly keep with it for a good while!

