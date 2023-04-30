This is the source code repository for my public website, currently hosted at https://ldam.co.za.

It contains:
- `frontend`: the public SvelteKit site that you see at https://ldam.co.za
- `frontend-ng`: Angular clone of `frontend`, available at https://angular.ldam.co.za
- `fnapp`: Azure Functions project that sync images from Adobe Lightroom to an Azure Storage Account
- `backend`: an ASP.NET Core app to login to Adobe so `fnapp` can access the Lightroom APIs
- `lib`: a .NET library containing code shared among all other projects
