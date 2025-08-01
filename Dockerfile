# Use the ASP.NET runtime image directly
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8.1-windowsservercore-ltsc2022

# Copy the pre-published application files to the default website location
COPY ./Legacy/publish/ /inetpub/wwwroot/

#RUN Get-ChildItem /inetpub/wwwroot/

# Expose port 80 internally
EXPOSE 80

# Start IIS using the default configuration
#CMD ["C:\\ServiceMonitor.exe", "w3svc"]