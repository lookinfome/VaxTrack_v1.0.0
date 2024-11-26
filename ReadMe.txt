====================================
-- Setup ASP.NET project in VS code
====================================
Install .NET SDK: First, ensure you have the .NET SDK installed on your system. You can download it from the official .NET website.
Install Visual Studio Code: If you haven’t already, download and install Visual Studio Code from the official website.
Install C# Extension: Open Visual Studio Code and install the C# extension from the Extensions view (Ctrl+Shift+X).
Create a New Project:
Open a terminal in Visual Studio Code (`Ctrl+``).
Navigate to the directory where you want to create your project.
Run the command: dotnet new webapp -n MyWebApp to create a new ASP.NET Core web application.
Open the Project:
Open the newly created project folder in Visual Studio Code using File > Open Folder.
Run the Application:
In the terminal, navigate to the project directory if you’re not already there.
Run the command: dotnet run.
Open a browser and navigate to http://localhost:5000 to see your application running.

================================
-- Setup MVC project in VS code
================================
Creating an ASP.NET Core MVC application using Visual Studio Code is straightforward. Here are the steps to get you started:

### Prerequisites
1. **Install .NET SDK**: Ensure you have the .NET SDK installed. You can download it from the official .NET website.
2. **Install Visual Studio Code**: Download and install Visual Studio Code from the official website.
3. **Install C# Extension**: Open Visual Studio Code and install the C# extension from the Extensions view (`Ctrl+Shift+X`).

### Steps to Create an ASP.NET Core MVC Application

1. **Open Visual Studio Code**:
   - Launch Visual Studio Code.

2. **Open a Terminal**:
   - Open the integrated terminal in Visual Studio Code by pressing `Ctrl+`` (backtick) or navigating to `View > Terminal`.

3. **Create a New Project**:
   - Navigate to the directory where you want to create your project.
   - Run the following command to create a new MVC project:
     ```sh
     dotnet new mvc -n MyMvcApp
     ```
   - This command creates a new ASP.NET Core MVC project named `MyMvcApp`.

4. **Open the Project**:
   - Open the newly created project folder in Visual Studio Code using `File > Open Folder`.

5. **Restore Dependencies**:
   - In the terminal, navigate to the project directory if you're not already there.
   - Run the command:
     ```sh
     dotnet restore
     ```

6. **Run the Application**:
   - Run the command:
     ```sh
     dotnet run
     ```
   - Open a browser and navigate to `http://localhost:5000` to see your application running.

### Additional Steps

- **Add a Controller**:
  - Right-click the `Controllers` folder in the Explorer view, select `New File`, and name it `HomeController.cs`.
  - Add the following code to `HomeController.cs`:
    ```csharp
    using Microsoft.AspNetCore.Mvc;

    namespace MyMvcApp.Controllers
    {
        public class HomeController : Controller
        {
            public IActionResult Index()
            {
                return View();
            }
        }
    }
    ```

- **Add a View**:
  - Right-click the `Views` folder, create a new folder named `Home`, and then create a new file named `Index.cshtml` inside the `Home` folder.
  - Add the following code to `Index.cshtml`:
    ```html
    @page
    @{
        ViewData["Title"] = "Home Page";
    }

    <div class="text-center">
        <h1 class="display-4">Welcome</h1>
        <p>Welcome to your ASP.NET Core MVC application!</p>
    </div>
    ```

======================
-- Launch Application
======================
To change the port your ASP.NET Core MVC project listens on, you can set the `ASPNETCORE_URLS` environment variable or use the `--urls` option with the `dotnet run` command. Here are the steps for both methods:

### Method 1: Using Environment Variable
1. **Windows PowerShell**:
   ```powershell
   $env:ASPNETCORE_URLS="http://localhost:3300"; dotnet run
   ```

2. **Windows Command Prompt**:
   ```cmd
   SET ASPNETCORE_URLS=http://localhost:3300 && dotnet run
   ```

3. **Unix-based Systems**:
   ```bash
   ASPNETCORE_URLS="http://localhost:3300" dotnet run
   ```

### Method 2: Using Command-Line Argument
1. Open your terminal or command prompt.
2. Navigate to your project directory.
3. Run the following command:
   ```bash
   dotnet run --urls "http://localhost:3300"
   ```

==========================
-- Install NuGet packages
==========================
You can install NuGet packages in Visual Studio Code using a few different methods. Here are the steps for the most common approaches:

### Using the Command Palette

1. **Open the Command Palette**:
   - Press `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (macOS).

2. **Search for NuGet Commands**:
   - Type `NuGet` and select `NuGet: Add NuGet Package`.

3. **Select the Project**:
   - If you have multiple projects, you'll be prompted to select the one you want to add the package to.

4. **Search and Install**:
   - Enter the name of the package you want to install, select it from the list, and choose the version.

### Using the Integrated Terminal

1. **Open the Terminal**:
   - Press `Ctrl+`` (backtick) or go to `View > Terminal`.

2. **Use the .NET CLI**:
   - Run the following command to install a package:
     ```sh
     dotnet add package <PACKAGE_NAME>
     ```
   - For example, to install `Newtonsoft.Json`:
     ```sh
     dotnet add package Newtonsoft.Json
     ```

### Using the NuGet Package Manager GUI Extension

1. **Install the Extension**:
   - Go to the Extensions view (`Ctrl+Shift+X`), search for `NuGet Package Manager GUI`, and install it.

2. **Open the Extension**:
   - Press `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (macOS) to open the Command Palette.
   - Type `NuGet` and select `NuGet Package Manager GUI`.

3. **Manage Packages**:
   - Use the GUI to search for, install, update, or remove packages.
