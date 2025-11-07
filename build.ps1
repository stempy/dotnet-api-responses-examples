$src = Join-Path $PSScriptRoot "src"
push-location $src
dotnet build --no-incremental
pop-location

