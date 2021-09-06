This is the source code repository for my public website, currently hosted at https://beta.ldam.co.za.

It contains:
- `client`: the public Blazor WebAssembly site that you see at https://beta.ldam.co.za
- `fnapp`: Azure Functions project that sync images from Adobe Lightroom to an Azure Storage Account
- `backend`: an ASP.NET Core app to login to Adobe so `fnapp` can access the Lightroom APIs
- `lib`: a .NET library containing code shared among all other projects