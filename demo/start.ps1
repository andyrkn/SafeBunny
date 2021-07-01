[string[]]$projects = "SafeBunny.Consumer.Delivery", "SafeBunny.Consumer.Billing", "SafeBunny.Consumer.Invoicing", "SafeBunny.Consumer.Orders"

foreach($p in $projects) {
    Write-Host "---------------------------------"
    Write-Host "Building $p/$p.csproj"
    dotnet build "$p/$p.csproj" --configuration=Release
    Start-Process -FilePath dotnet -ArgumentList "run --configuration=Release --project $p/$p.csproj"
}