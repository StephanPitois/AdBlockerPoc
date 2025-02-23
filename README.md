# AdBlockerPoc

AdBlockerPoc is a proof-of-concept application demonstrating how to block advertisements in a web browser using the WebView2 control in a WPF application.

## Features

- Blocks requests to known ad domains.
- Displays a list of blocked items.
- Toggle ad blocker on and off.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or any other compatible IDE

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/AdBlockerPoc.git
    ```
2. Open the solution file `AdBlockerPoc.sln` in Visual Studio.
3. Restore the NuGet packages:
    ```sh
    dotnet restore
    ```

### Running the Application

1. Build the solution:
    ```sh
    dotnet build
    ```
2. Run the application:
    ```sh
    dotnet run --project AdBlockerPoc
    ```

## Usage

- The application starts with the ad blocker enabled by default.
- Click the "Disable Ad Blocker" button to toggle the ad blocker on and off.
- The list at the bottom displays the URLs of blocked items.

