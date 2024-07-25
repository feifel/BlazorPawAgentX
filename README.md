# Summary

Agent-X is an outdoor game that requires at least 2 players. The first player is Agent-X and all other players are police officers that try to catch Agent-X. All player see a map showing where the all the players are. The position of police officers is updated in real time, but the position of Agent-X is updated only every 60 seconds.

Agent-X is a [Blazor Progressive Web Application (PAW)](https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app) written in C# (.NET). PAW have the benefit that they can run in the browser, but can also be installed as Native Application on any mobile or PC. 

To communicate between 2 applications running on a mobile phone, a WebServer is required, since direct communication is not possible. [BlazorPawAgentX](https://github.com/feifel/BlazorPawAgentX/tree/main) is using [SignalRServiceHub](https://github.com/feifel/SignalRServiceHub) for this:

```mermaid
classDiagram
    SignalRServiceHub<|-- [BlazorPawAgentX](https://github.com/feifel/BlazorPawAgentX/tree/main)-1
    SignalRServiceHub<|-- [BlazorPawAgentX](https://github.com/feifel/BlazorPawAgentX/tree/main)-2
    SignalRServiceHub<|-- [BlazorPawAgentX](https://github.com/feifel/BlazorPawAgentX/tree/main)-3
```

# Project creation tutorial

This sections describes how this  project was created. In case you want to build something similar, this could be a good starting point. 

1. Download Visual Studio 2022 Community Edition
2. When install Visual Studio, make sure that you select “ASP.NET and web development”
3. Open “developer command prompt” by typing this to the Windows Search bar
4. Create a directory for your source code repositories: `mkdir userprofile%\source\repos`
5. Change to this directory: `cd %userprofile%\source\repos`
6. Create an empty web project: `dotnet new blazorwasm -o BlazorPawAgentX --pwa` 
7. Change the directory to the project folder: `cd BlazorPawAgentX` 
8. In order to use [Git](https://git-scm.com/) to track your changes in the project:
    1. Install Git: `winget install --id Git.Git -e --source winget`
    2. Restart the “developer command prompt” and change to the current directory again
    3. Configure git:        
        `git config --global user.email "[you@example.com](mailto:you@example.com)"`
        `git config --global [user.name](http://user.name/) "Your Name"`
    4. Initialize Git: `git init`
    5. Create a .gitignore: `dotnet new gitignore`
    6. Add files to Staging area: `git add .`
    7. Commit the changes: 
    `git commit -m "Project created with: dotnet new blazorwasm -o BlazorPawAgentX --pws"`
9. Before we do apply further changes I push it to GitHub:
    1. Create a free account on github for free by using the “Sign up” on [https://github.com/](https://github.com/)
    2. Create a new repository “SignalRServiceHub" on github. **NOTE: To pervent merge conflict, it is very important to NOT initialize it with a README, .gitignore, or license.**
    3. Add the project to the remote repo in the “developer command prompt” with:
    `git remote add origin <your-repository-URL>`
    4. The remote branch is main, the local is master, so we need to: `git branch -M main`
    5. Push the local changes to GitHub: `git push -u origin main`
10. Build the project: `dotnet build`
11. Open the project in Visual Studio: `devenv BlazorPawAgentX.csproj`
12. You can start the App by pressing the green play button (https). This opens a browser and shows a sample app, that we will modify in the following steps.
13. Modify the sample App like you can see in [this](https://github.com/feifel/BlazorPawAgentX/commit/1c6ee5aebd7470c902814f12572b5519259a2fe0) commit.
14. In order to test it, we need to start the [SignalRServiceHub](https://github.com/feifel/SignalRServiceHub).
15. The URL of the SignalRServiceHub need to be configured [here](https://github.com/feifel/BlazorPawAgentX/blob/1c6ee5aebd7470c902814f12572b5519259a2fe0/App.razor#L15).
16. When you now press the green play button (https) in Visual Studio again, you can test the initial version of the Agent-X App.

# Deployment to GitHub Pages

To simplify the access to this application for other users we need to upload the binaries to a public server. Since Blazor Web Applications are fully running in the Browser (no server code is running), we can deploy it as a Static Files. And Static Files can be deployed on GitHub-Pages for free. The section below describe how to do this. It is based on the following links:
- https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly#github-pages
- https://dev.to/j_sakamoto/the-easier-way-to-publish-your-blazor-webassembly-app-for-github-pages-319l

1. Go to the Actions page of the GitHub Repository: [https://github.com/feifel/BlazorPawAgentX/actions/new](https://github.com/feifel/BlazorPawAgentX/actions/new)
2. There you can find a link [set up a workflow yourself](https://github.com/feifel/BlazorPawAgentX/new/main?filename=.github%2Fworkflows%2Fmain.yml&workflow_template=blank) to start the process.
3. Paste the content of main.yml and commit it.
4. Go to [https://github.com/feifel/BlazorPawAgentX/settings/actions](https://github.com/feifel/BlazorPawAgentX/settings/actions) and scroll to the bottom 
5. Under Workflow permissions, select “Read and Writer permissions” and press save
6. Every time when you commit something to the main branch, the main.yml file is triggered and will run a build and deploy the binaries to the gh-pages branch. 
7. The App is the available on: [https://feifel.github.io/BlazorPawAgentX/](https://feifel.github.io/BlazorPawAgentX/)

## Trouble Shooting

The deployment of Progressive Web Applications to GitHub Pages is quiet tricky and even unstable. Here are a couple of things that may be useful to know:

1. I observed several times that the build was triggered and the files in the gh-pages branch are correctly updated, but when I opened [https://feifel.github.io/BlazorPawAgentX/](https://feifel.github.io/BlazorPawAgentX/) it does not show my latest changes! Sometimes the Browser has cached the old files, but this can be easily fixed by pressing F12 → Application → Storage → Clear Site Data. However this did not always fixed the issue, sometimes I had to go to GitHub → Settings → Pages → and then:
    
    ![https://i.imgur.com/5xvU8OL.png](https://i.imgur.com/5xvU8OL.png)
    
    And after that I had to go to Actions → pages-build-deployment → “pages build and deployment” → Re-run all jobs. So it seems to me that GitHub pages is sometimes not providing the binary files that are visible in the gh-pages branch. It seems to have maybe also a cache that still serves old files!
    
2. Another issue with the GitHub deployment is, that the application is deployed not to a subfolder, e.g. [https://feifel.github.io/BlazorPawAgentX/](https://feifel.github.io/BlazorPawAgentX/). This can cause issues for the navigation inside the application. If you navigate to “/” it will go to https://feifel.github.io/ instead of https://feifel.github.io/BlazorPawAgentX/. This needs to be fixed within the App. Instead of `Navigation.NavigateTo("/");` we call `Navigation.NavigateTo(Navigation.BaseUri);`
3. Testing the issue 2. could be very painful because of issue 1., therefore I wanted to deploy it locally in the same way, so that I can test it locally before deploying to GitHub Pages. This requires some extra steps:
    1. Open “Turn On Windows Features”, select Internet Information Service (IIS)
    2. In Visual Studio right click on the Project and select publish
    3. Add a new profile and select: Web Server (IIS) → Web Deploy
        1. Server: ROMBUSX3D
        2. IIS Application/Site name: `Default Web Site/BlazorPawAgentX`
        3. Destination URL/Site: [`http://localhost/BlazorPawAgentX`](http://localhost/BlazorPawAgentX)
    4. Once deployed this will add in IIS this: `..\Sites\Default Web Site\BlazorPawAgentX`
    5. Note: To run in locally you need to modify the base tag in index.html to : 
    `<base href="/BlazorPawAgentX/" />`
4. Unfortunately the method in step 3 cannot be used for debugging, even when Configuration “Debug” is selected in the publish profile it cannot load the symbols
    1. I also tried to configure and Start “Local IIS” from Visual Studio. It starts and the PWA is working, but Breakpoints do not work!
    2. I also tried to us IIS Express for this, but I was not able to get it even working.
    3. The same is for HTTPS and HTTP launch config. This works but only with base path `/`
    4. Conclusion: Debugging with base path `/BlazorPawAgentX/` does not work!
    5. This is a pity but I lost too many hours without finding a solution. So for the moment for debugging only works with http and https lunch profile but those only work with:
    `<base href="/" />`
5. Last point to mention is that, on GitHub Pages, I have now 2 PWAs (BlazorPawAgentX and WebXrFileworkSimulator). This creates the following issue:
[https://web.dev/articles/building-multiple-pwas-on-the-same-domain](https://web.dev/articles/building-multiple-pwas-on-the-same-domain)
    
    When both are installed on an Android Device then it gets confused, e.g. with the Icons. The BlazorPawAgentX became the Icon of WebXrFileworkSimulator.
1. 