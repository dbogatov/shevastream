git pull
dotnet restore
supervisorctl stop shevastream
rm -rf /var/aspnet/shevastream/*
dotnet publish -o /var/aspnet/shevastream
supervisorctl start shevastream
