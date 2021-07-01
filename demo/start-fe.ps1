Start-Process -FilePath dotnet -ArgumentList "run --configuration=Release --project SafeBunny.Client/SafeBunny.Client.csproj"
Set-Location "C:\Users\SixthMagnitudeStar\Documents\repos\SafeBunny-docs\samples\SafeBunny.Client\ClientApp"
Start-Process -FilePath npm -ArgumentList "start"
Set-Location "C:\Users\SixthMagnitudeStar\Documents\repos\SafeBunny-docs\samples"