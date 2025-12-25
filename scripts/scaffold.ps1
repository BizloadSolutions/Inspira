param(
    [string]$Root = (Get-Location).Path,
    [string]$Tfm = 'net8.0'
)

# Create root directory
New-Item -ItemType Directory -Path $Root -Force | Out-Null
Set-Location -Path $Root

# Create solution
dotnet new sln -n Inspira -o $Root

# Create projects
dotnet new classlib -n Inspira.Domain -f $Tfm -o src/Inspira.Domain
dotnet new classlib -n Inspira.Application -f $Tfm -o src/Inspira.Application
dotnet new classlib -n Inspira.Infrastructure -f $Tfm -o src/Inspira.Infrastructure
dotnet new webapi -n Inspira.API -f $Tfm -o src/Inspira.API
dotnet new xunit -n Inspira.Tests -f $Tfm -o tests/Inspira.Tests

# Add projects to solution
dotnet sln add src/Inspira.Domain/Inspira.Domain.csproj
dotnet sln add src/Inspira.Application/Inspira.Application.csproj
dotnet sln add src/Inspira.Infrastructure/Inspira.Infrastructure.csproj
dotnet sln add src/Inspira.API/Inspira.API.csproj
dotnet sln add tests/Inspira.Tests/Inspira.Tests.csproj

# Add project references
dotnet add src/Inspira.Application/Inspira.Application.csproj reference src/Inspira.Domain/Inspira.Domain.csproj
dotnet add src/Inspira.Infrastructure/Inspira.Infrastructure.csproj reference src/Inspira.Application/Inspira.Application.csproj
dotnet add src/Inspira.Infrastructure/Inspira.Infrastructure.csproj reference src/Inspira.Domain/Inspira.Domain.csproj
dotnet add src/Inspira.API/Inspira.API.csproj reference src/Inspira.Application/Inspira.Application.csproj
dotnet add src/Inspira.API/Inspira.API.csproj reference src/Inspira.Infrastructure/Inspira.Infrastructure.csproj
dotnet add tests/Inspira.Tests/Inspira.Tests.csproj reference src/Inspira.Application/Inspira.Application.csproj
dotnet add tests/Inspira.Tests/Inspira.Tests.csproj reference src/Inspira.Domain/Inspira.Domain.csproj

function Ensure-ProjectProperties($filePath) {
    try {
        [xml]$xml = Get-Content $filePath
    } catch {
        Write-Error ("Failed to load XML from {0}: {1}" -f $filePath, $_)
        return
    }

    # Find an existing PropertyGroup or create one
    $pg = $xml.Project.PropertyGroup | Select-Object -First 1
    if (-not $pg) {
        $newPg = $xml.CreateElement('PropertyGroup')
        $xml.Project.AppendChild($newPg) | Out-Null
        # Re-fetch the appended PropertyGroup as an XmlElement
        $pg = $xml.Project.PropertyGroup | Select-Object -First 1
    }

    if (-not $pg) {
        Write-Error ("Unable to obtain or create a PropertyGroup in {0}" -f $filePath)
        return
    }

    if (-not $pg.Nullable) {
        $nullable = $xml.CreateElement('Nullable')
        $nullable.InnerText = 'enable'
        $pg.AppendChild($nullable) | Out-Null
    }
    if (-not $pg.ImplicitUsings) {
        $iais = $xml.CreateElement('ImplicitUsings')
        $iais.InnerText = 'enable'
        $pg.AppendChild($iais) | Out-Null
    }

    $xml.Save($filePath)
}

Get-ChildItem -Path (Join-Path $Root 'src') -Recurse -Filter *.csproj |
ForEach-Object {
    Ensure-ProjectProperties $_.FullName
}

Get-ChildItem -Path (Join-Path $Root 'tests') -Recurse -Filter *.csproj |
ForEach-Object {
    Ensure-ProjectProperties $_.FullName
}

Write-Host 'Scaffold complete. Open Inspira.sln in Visual Studio or run dotnet build.'
