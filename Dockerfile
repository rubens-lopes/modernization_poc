FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8.1-windowsservercore-ltsc2022

WORKDIR /app

# Install .NET 8 Hosting Bundle (not just runtime)
ADD https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.18/dotnet-hosting-8.0.18-win.exe "C:\Users\Public\Downloads\dotnet-hosting.exe"
RUN powershell -Command \
     "Start-Process -FilePath 'C:\Users\Public\Downloads\dotnet-hosting.exe' -ArgumentList '/quiet' -Wait; \
     Remove-Item 'C:\Users\Public\Downloads\dotnet-hosting.exe'"

# Install .NET 8 Runtime separately for CLI support
ADD https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.18/aspnetcore-runtime-8.0.18-win-x64.exe "C:\Users\Public\Downloads\dotnet-runtime-cli.exe"
RUN powershell -Command \
     "Start-Process -FilePath 'C:\Users\Public\Downloads\dotnet-runtime-cli.exe' -ArgumentList '/quiet' -Wait; \
     Remove-Item 'C:\Users\Public\Downloads\dotnet-runtime-cli.exe'"

# Copy applications
COPY ./Legacy/publish/ /inetpub/legacy/
COPY ./Modern/publish/ .

# Configure IIS for Legacy app
RUN powershell -Command \
    "Import-Module WebAdministration; \
     Remove-WebSite -Name 'Default Web Site' -ErrorAction SilentlyContinue; \
     New-WebAppPool -Name 'LegacyPool' -Force; \
     Set-ItemProperty -Path 'IIS:\AppPools\LegacyPool' -Name managedRuntimeVersion -Value 'v4.0'; \
     New-WebSite -Name 'Legacy' -Port 5000 -PhysicalPath 'C:\inetpub\legacy' -ApplicationPool 'LegacyPool';"

EXPOSE 80

# Use HTTP instead of HTTPS to avoid certificate issues
ENTRYPOINT ["dotnet", "ModernizationPoC.Modern.dll", "--urls=http://0.0.0.0:80"]