#!/bin/sh

dotnet restore
cd ./${PROJECT_DIR}
dotnet ef database drop --verbose
dotnet ef migrations add InitialCreate --verbose
dotnet ef database update
