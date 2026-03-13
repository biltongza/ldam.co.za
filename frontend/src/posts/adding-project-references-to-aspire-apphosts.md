---
title: 'Adding Project References to Aspire AppHosts'
date: 2026-03-13
excerpt: 'How to allow Aspire AppHosts to reference other libraries in your solution without using them as Aspire Resources'
tags:
  - dotnet
  - aspire
---

# TL;DR

If you're here from Google and just want to know how to add a project reference to an Aspire AppHost as a normal reference (without Aspire trying to orchestrate it), add the `IsAspireProjectResource` property to the `ProjectReference`, like so:

```xml
<ItemGroup>
    <ProjectReference Include="./Library/Library.csproj">
      <IsAspireProjectResource>false</IsAspireProjectResource> <!-- Tells Aspire not to treat this project reference as something we want to orchestrate, i.e. a normal project reference -->
    </ProjectReference>
    <ProjectReference Include="./Api/Api.csproj" />
</ItemGroup>
```

# Background

If you haven't used [Aspire](https://aspire.dev/) yet or don't know what it is, it's a pretty nifty orchestration system that you can use to accelerate your local development cycle in dotnet ([among other langs](https://aspire.dev/#polyglot-multilanguage-support)).

I've been using it at work to simplify the local development experience, i.e. press F5 and watch the entire application spin up ready for me to go, no need to make sure I have a database running or remember to start another app.

It does this by using a special **AppHost** project, which you use to tell Aspire what you want it to orchestrate.

For example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add API service
var api = builder.AddProject<Projects.ApiService>("api");

builder.Build().Run();
```

The `AddProject()` method takes in a name, and this name is used to identify the project you want Aspire to orchestrate for you.

You can tell Aspire that your project has a dependency on another project like so:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add API service
var api = builder.AddProject<Projects.ApiService>("api");

var api2 = builder.AddProject<Projects.WeatherService>("api2")
    .WithReference(api);

builder.Build().Run();
```

This tells Aspire that `api2` knows about `api1`, and sets up **Service Discovery** so that `api2` can communicate with `api1` without needing to know the IP address/hostname of `api1`.

That's pretty cool, because it means you can do this in `api2`:

```csharp
var client = new HttpClient("https://api/");
var response = await client.GetFromJsonAsync<Stuff>("/stuff");
```

`HttpClient` gets to know that `api` exists and figures out how to talk to it on your behalf. No need to figure out which port it's running on either, Aspire handles that for you.

Same thing works for database connection strings:

```csharp
// AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

// Add database
var postgres = builder.AddPostgres("db")
    .AddDatabase("appdata");

// Add API service and reference the database
var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres);

builder.Build().Run();
```

```csharp
// ApiService/Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<MyDbContext>(connectionName: "appdata"); // use the same name you gave the DB in Aspire here
```

Aspire handles the connection string for you.

# Magic Strings

While this is very cool, I personally don't like that there is a magic string that both projects need to know about. What if we decide to rename it? What if someone makes a typo? What if we want to find everyone that _really_ references the project?

So I thought, well, we can just make a class library with some constants and have both my project and the AppHost reference it. So that's what I did:

```csharp
// Library.csproj
namespace Library;
public static class ResourceNames
{
    public const string Api = "api";
    public const string Database = "appdata";
}
```

```xml
<!-- AppHost.csproj -->
<Project Sdk="Aspire.AppHost.Sdk/13.1.2">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../api/Api.csproj" />
    <ProjectReference Include="../lib/Library.csproj" /> <!-- I added this -->
  </ItemGroup>
</Project>
```

Then I expected to be able to reference Library in my `AppHost/Program.cs`:

```csharp
// AppHost/Program.cs
using Library; // CS0246: The type or namespace name 'Library' could not be found

var builder = DistributedApplication.CreateBuilder(args);

// Add database
var postgres = builder.AddPostgres("db")
    .AddDatabase(ResourceNames.Database);

// Add API service and reference the database
var api = builder.AddProject<Projects.ApiService>(ResourceNames.Api)
    .WithReference(postgres);

builder.Build().Run();
```

I was very confused, because I just added a project reference, this is bread and butter stuff. What was going wrong?

It turns out I had forgotten that Aspire treats all project references in the AppHost as projects that you want to use with Aspire orchestration (which is where the `Projects.Api` comes from).
If you want Aspire _not_ to do that, you need to explicitly tell it, like this:

```xml
<ProjectReference Include="../lib/Library.csproj">
  <IsAspireProjectResource>false</IsAspireProjectResource>
</ProjectReference>
```

Once that is added, it goes back to being a plain old project reference and the errors go away.
