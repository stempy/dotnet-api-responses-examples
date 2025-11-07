# PowerShell script to generate OpenAPI document from the running application
# This script starts the application, fetches the OpenAPI document, and saves it to a file

param(
    [string]$OutputPath = "openapi.json",
    [int]$Port = 5000
)

Write-Host "Starting application to generate OpenAPI document..." -ForegroundColor Cyan

# Start the application in the background
$app = Start-Process -FilePath "dotnet" -ArgumentList "run --no-build" -PassThru -NoNewWindow

# Wait for the application to start
Start-Sleep -Seconds 3

try {
    Write-Host "Fetching OpenAPI document from http://localhost:$Port/openapi/v1.json" -ForegroundColor Cyan
    
    # Fetch the OpenAPI document
    $response = Invoke-WebRequest -Uri "http://localhost:$Port/openapi/v1.json" -UseBasicParsing
    
    # Save to file
    $response.Content | Out-File -FilePath $OutputPath -Encoding UTF8
    
    Write-Host "OpenAPI document saved to $OutputPath" -ForegroundColor Green
}
catch {
    Write-Host "Error fetching OpenAPI document: $_" -ForegroundColor Red
    exit 1
}
finally {
    # Stop the application
    Stop-Process -Id $app.Id -Force
    Write-Host "Application stopped" -ForegroundColor Cyan
}
