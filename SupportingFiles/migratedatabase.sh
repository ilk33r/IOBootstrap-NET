#!/bin/sh

dotnet restore
cd ./${PROJECT_DIR}
dotnet ef database drop --verbose
dotnet ef migrations add InitialCreate --verbose
dotnet ef database update


dotnet ef migrations add X --context IODefaultDatabaseContext --verbose
dotnet ef migrations script Y --context IODefaultDatabaseContext --verbose -o ../Y.sql