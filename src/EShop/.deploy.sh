#!/bin/bash

set -e

SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
  DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
  SOURCE="$(readlink "$SOURCE")"
  [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE" # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
done
DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"

cd $DIR

git fetch origin
git reset --hard origin/master

# remmeber build hash
printf "{\n\t\"Version\" : {\n\t\t\"GitHash\" : \"$(git rev-parse --short HEAD)\"\n\t}\n}\n" > version.json

dotnet restore

supervisorctl stop shevastream

rm -rf /var/aspnet/shevastream/*

dotnet publish -o /var/aspnet/shevastream

supervisorctl start shevastream
