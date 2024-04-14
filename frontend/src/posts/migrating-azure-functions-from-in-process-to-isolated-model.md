---
title: 'Migrating an Azure Functions from the in-process model to the isolated worker model'
date: 2024-03-31
excerpt: 'Step by step process to migrate to the new model'
tags:
  - meta
  - tech
  - azure
  - dotnet
  - functions
  - serverless
---

# Time to upgrade!

This week I got an email from Azure alerting me that the in-process model for Azure Functions is deprecated and will no longer be supported from **10 November 2026**, and that the course of action is to migrate to the **isolated worker** model.

![Call to action from Microsoft](/assets/call-to-action-azure-functions.png)

# What's the difference?

First let's discuss what these models are.

The name _in-process_ implies that our functions are executed in the same process as the Azure Functions host. This has some implications:

- We have to use whatever version of .NET the Azure Functions host uses. This has caused some [confusion in the past](https://github.com/Azure/Azure-Functions/issues/1380). The Azure Functions versions and their counterpart .NET versions are [documented](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v4%2Ccmd#supported-versions). At this time the supported versions are v1 and v4, v1 runs in .NET Framework 4.8 and v4 runs in .NET 6.
- We have to live with whatever versions of assemblies the host uses. This can cause conflicts if we want to use a package that uses newer dependencies than the host has.
- We don't control how the process starts, which means we have to live with whatever configuration, or more importantly, dependency injection system the host decides.

The _isolated worker_ model implies that our functions are executed _isolated_ from the host. This is similar in concept to how web browsers (and Electron) work, where you have different, isolated processes running each tab and they all communicate and coordinate with the main, startup process. That way if one tab crashes, your whole browser doesn't crash, and you get some extra safety too by means of sandboxing (though that's more than just using isolated processes).

# Why?

Reading the [documentation](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows) we see that these problems above are addressed by the isolated worker model. So, this is a pretty good idea to do regardless of the in-process model being deprecated.

**An important detail to remember is that the support period of .NET 6 ends 12 November 2024.** This implies that if you're using the in-process model after that date, you are running on an unsupported version of .NET, unless the Azure Functions team releases a new version of the Functions SDK targeting .NET 8 or newer.

# Upgrade Steps

Microsoft has published a [guide](https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model?tabs=net8) detailing how to do the migration. The guide also upgrades from .NET 6 to .NET 8 at the same time. I rather want to switch the the new model first, validate it works, _then_ upgrade to .NET 8.

## 1. Migrate the local project

First, we need to update our .csproj file. We need to make the following changes:

- Change the `OutputType` to `Exe`. If you don't already have an `OutputType` node, add it.
- Add the following to the `PackageReference` list:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
<PackageReference Include="Microsoft.Azure.Functions.Worker" Version="1.21.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.17.2" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore" Version="1.2.1" />
<PackageReference Include="Microsoft.ApplicationInsights.WorkerService" Version="2.22.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.ApplicationInsights" Version="1.2.0" />
```

- Remove reference to `Microsoft.Azure.Functions.Extensions`
- Remove reference to `Microsoft.NET.Sdk.Functions`
- Check which bindings you are using, and update them as [detailed in the guide](https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model?tabs=net8#package-references). In my case I am using timers, so I need to add reference to `Microsoft.Azure.Functions.Worker.Extensions.Timer`.
- Remove and replace any package starting with `Microsoft.Azure.WebJobs.*`

## 2. Add a Program.cs file

Since we're no longer running in the Azure Functions host, we must provide our own `Main` method in the `Program.cs` file. Follow the MS guide [here](https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model?tabs=net8#programcs-file).

Start out with the following code:

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();


    })
    .Build();

host.Run();
```

If you previously used a `Startup` class decorated with the `FunctionsStartup` attribute, you need to migrate that code to this file.

In my case, it is mostly a copy paste job, but I did need to add a `context` parameter to the `ConfigureServices` delegate so I could access configuration. My final `Program.cs` looks like this:

```csharp
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using ldam.co.za.fnapp;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Configuration;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddMemoryCache();
        services.AddTransient<ISecretService, SecretService>();

        services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
        services.AddHttpClient("lightroom")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true,
            })
            .RedactLoggedHeaders(new string[] { "Authorization", "X-API-KEY" });

        services.AddTransient<ILightroomClient, LightroomClient>();
        services.AddTransient<ILightroomService, LightroomService>();
        services.AddTransient<IStorageService, StorageService>();
        services.AddTransient<IClock, Clock>();
        services.AddTransient<RefreshTokenService>();
        services.AddTransient<SyncService>();
        services.AddTransient<ILightroomTokenService, LightroomTokenService>();
        services.AddTransient<IMetadataService, MetadataService>();
        services.AddTransient<IWebPEncoderService, WebPEncoderService>();
        services.AddSingleton<TokenCredential>((_) =>
                new ChainedTokenCredential(
#if DEBUG
                    new AzureCliCredential(),
#endif
                    new ManagedIdentityCredential()
                ));
        services.AddSingleton<ArmClient>();
        services.AddTransient<ICdnService, CdnService>();

        var config = context.Configuration;
        services.Configure<FunctionAppAzureResourceOptions>(config.GetSection("Azure"));
        services.Configure<AzureResourceOptions>(config.GetSection("Azure"));
        services.Configure<FunctionAppLightroomOptions>(config.GetSection("Lightroom"));
        services.Configure<LightroomOptions>(config.GetSection("Lightroom"));
    })
    .Build();

host.Run();
```

## 3. Remove the Startup.cs file

Once the code from the `FunctionsStartup` class has been migrated to the `Program.cs` file, it should be safe to delete it.

## 4. Update function signatures

The isolated worker model brings with it some changes to some types and so we need to update our function signatures accordingly. This step will differ depending on what function bindings you have, so I will only document what is required for my project. The details are well documented [here](https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model?tabs=net8#function-signature-changes).

### Change `FunctionName` attributes to `Function`

This is a simple change from `FunctionName` to `Function`. The signature stays the same so you can do a find/replace.

### Inject `ILogger`

The in-process model provided you with an `ILogger` in your function method. This has changed and must be injected into your function class instead. Remove `ILogger` from your method, add an `ILogger` field, and add an `ILogger` parameter to your constructor and assign it to the field.

For example, this:

```csharp
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class RefreshAccessTokenFunc
{
    private readonly RefreshTokenService refreshTokenService;
    public RefreshAccessTokenFunc(RefreshTokenService refreshTokenService)
    {
        this.refreshTokenService = refreshTokenService;
    }

    [Function(nameof(RefreshAccessTokenFunc))]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, ILogger logger)
    {
        await refreshTokenService.RefreshAccessToken();
    }
}

```

Becomes:

```csharp
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class RefreshAccessTokenFunc
{
    private readonly RefreshTokenService refreshTokenService;
    private readonly ILogger logger;

    public RefreshAccessTokenFunc(RefreshTokenService refreshTokenService, ILogger<RefreshAccessTokenFunc> logger)
    {
        this.refreshTokenService = refreshTokenService;
        this.logger = logger;
    }

    [Function(nameof(RefreshAccessTokenFunc))]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
    {
        logger.LogInformation("RefreshAccessToken fired");
        await refreshTokenService.RefreshAccessToken();
    }
}
```

Repeat this for all your functions.

### Update bindings and triggers

Again, what this involves depends on what bindings and triggers you have. The full details are [here](https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model?tabs=net8#trigger-and-binding-changes).

In my case, it was as simple as removing all `using Microsoft.Azure.WebJobs;` statements.

`HttpTrigger` is defined in the `Microsoft.Azure.Functions.Worker` namespace so replacing the WebJobs one with this one fixed that.

The same applies to `TimerTrigger`.

### Update local.settings.json

Simply change the value for `FUNCTIONS_WORKER_RUNTIME` from `dotnet` to `dotnet-isolated`.

**NB: After deploying the isolated worker changes, don't forget to update the `FUNCTIONS_WORKER_RUNTIME` setting in your FunctionApp configuration in the Azure Portal!**

## 5. Test

With those changes made, my project builds. It's time to test it!

First, we build the project with `dotnet build`:

```plaintext
MSBuild version 17.9.6+a4ecab324 for .NET
  Determining projects to restore...
/Users/logan/projects/ldam.co.za/fnapp/fnapp.csproj : warning NU1903: Package 'SixLabors.ImageSharp' 3.0.2 has a known high severity vulnerability, https://github.com/advisories/GHSA-65x7-c272-7g7r
  All projects are up-to-date for restore.
/Users/logan/projects/ldam.co.za/fnapp/fnapp.csproj : warning NU1903: Package 'SixLabors.ImageSharp' 3.0.2 has a known high severity vulnerability, https://github.com/advisories/GHSA-65x7-c272-7g7r
  lib -> /Users/logan/projects/ldam.co.za/lib/bin/Debug/net6.0/lib.dll
  fnapp -> /Users/logan/projects/ldam.co.za/fnapp/bin/Debug/net6.0/fnapp.dll
  Determining projects to restore...
  Restored /var/folders/y8/npcjv69d7qnbk_q0wl4wdhw80000gn/T/qndzag4t.rqr/WorkerExtensions.csproj (in 316 ms).
  WorkerExtensions -> /var/folders/y8/npcjv69d7qnbk_q0wl4wdhw80000gn/T/qndzag4t.rqr/buildout/Microsoft.Azure.Functions.Worker.Extensions.dll

Build succeeded.

/Users/logan/projects/ldam.co.za/fnapp/fnapp.csproj : warning NU1903: Package 'SixLabors.ImageSharp' 3.0.2 has a known high severity vulnerability, https://github.com/advisories/GHSA-65x7-c272-7g7r
/Users/logan/projects/ldam.co.za/fnapp/fnapp.csproj : warning NU1903: Package 'SixLabors.ImageSharp' 3.0.2 has a known high severity vulnerability, https://github.com/advisories/GHSA-65x7-c272-7g7r
    2 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.55
```

I'll sort out that warning later when I upgrade to .NET 8.

_I'm not worried about this security vulnerability warning, as there's no way for people to upload arbitrary images to my application, nor do I use PNGs anywhere in my project._

Let's make sure my unit tests still work with `dotnet test`:

```plaintext
Microsoft (R) Test Execution Command Line Tool Version 17.9.0 (arm64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9, Duration: 14 ms - fnapp.Tests.dll (net6.0)
```

All green! I didn't expect any issues here since no logic changed. Still a good idea to check though!

Now let's run it with `func host start`. This command dumps out a lot of output, so I won't put it all here, but I did observe a pesky warning:

```plaintext
No job functions found. Try making your job classes and methods public. If you're using binding extensions (e.g. Azure Storage, ServiceBus, Timers, etc.) make sure you've called the registration method for the extension(s) in your startup code (e.g. builder.AddAzureStorage(), builder.AddServiceBus(), builder.AddTimers(), etc.).
```

I had a hunch that the version of the Azure Functions Core Tools I had on my machine was out of date, so I updated it. You can find platform specific instructions [here](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=macos%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp#install-the-azure-functions-core-tools).

In my case on macOS:

```plaintext
brew tap azure/functions
brew install azure-functions-core-tools@4
```

Now when I start up my project with `func host start`, I can see my functions are detected:

```plaintext
Functions:

        ManualImageSync: [POST] http://localhost:7071/api/ManualImageSync

        RefreshAccessTokenFunc: timerTrigger

        RefreshImagesTimer: timerTrigger
```

I tested my ManualImageSync function locally and all looks good, no explosions yet! Now is a good time to commit.

# Upgrade to .NET 8

Since we're now free to run .NET 8, let's do the upgrade.

First, I'll check the documented breaking changes from [.NET 7](https://learn.microsoft.com/en-us/dotnet/core/compatibility/7.0) and [.NET 8](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0). It's a big list, but luckily my project is tiny so the risk and impact of something big is relatively low.

I'll upgrade all of my projects at the same time. To do this, I'll update the `TargetFramework` property of each csproj to `net8.0`.

Next, I'll go through all my Nuget packages and check for upgrades. I'm expecting to update any ASP.NET Core packages from 6.x to 8.x. There's a lovely little extension for VSCode I like that makes this process quite simple: [aliasadidev.nugetpackagemanagergui](https://marketplace.visualstudio.com/items?itemName=aliasadidev.nugetpackagemanagergui). It gives you a nice list of all the packages your projects reference, what the latest version is, and gives you options to update them. Note that when you update packages with this tool, it only updates the csproj. You must then manually do a restore to see the effects.

So once I've updated all my projects with new versions, I can do a `dotnet clean` and a `dotnet restore`, followed by a `dotnet build` (do this at solution level to save yourself some time).

```plaintext
dotnet build
MSBuild version 17.9.6+a4ecab324 for .NET
  Determining projects to restore...
  All projects are up-to-date for restore.
  lib -> /Users/logan/projects/ldam.co.za/lib/bin/Debug/net8.0/lib.dll
  fnapp -> /Users/logan/projects/ldam.co.za/fnapp/bin/Debug/net8.0/fnapp.dll
  backend -> /Users/logan/projects/ldam.co.za/backend/bin/Debug/net8.0/backend.dll
  Determining projects to restore...
  Restored /var/folders/y8/npcjv69d7qnbk_q0wl4wdhw80000gn/T/pq3cgsif.eae/WorkerExtensions.csproj (in 470 ms).
  WorkerExtensions -> /var/folders/y8/npcjv69d7qnbk_q0wl4wdhw80000gn/T/pq3cgsif.eae/buildout/Microsoft.Azure.Functions.Worker.Extensions.dll
  fnapp.Tests -> /Users/logan/projects/ldam.co.za/fnapp.Tests/bin/Debug/net8.0/fnapp.Tests.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.06
```

All good! Now I can run tests and run my functions to check if they still work.

```plaintext
dotnet test
Test run for /Users/logan/projects/ldam.co.za/fnapp.Tests/bin/Debug/net8.0/fnapp.Tests.dll (.NETCoreApp,Version=v8.0)
Microsoft (R) Test Execution Command Line Tool Version 17.9.0 (arm64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9, Duration: 31 ms - fnapp.Tests.dll (net8.0)
```

As expected, no tests failures here.

Starting up my functions host I see my functions being registered and executing normally. Running an image sync, all went fine, so I am declaring it good to go.

Finally, I need to update my Github Action to use the .NET 8 SDK, and with that, we are done! :)
