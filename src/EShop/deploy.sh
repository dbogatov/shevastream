supervisorctl stop shevastream
dotnet publish -o /var/aspnet/shevastream
supervisorctl start shevastream
