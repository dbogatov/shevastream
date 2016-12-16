#!/bin/bash

set -e

PUBLISH_DIR = "/srv/www/dotnetcore/shevastream/"

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

npm install
bower install --allow-root
gulp

supervisorctl stop shevastream

rm -rf $PUBLISH_DIR/*

dotnet publish -o $PUBLISH_DIR

cp {appsettings,version}.json $PUBLISH_DIR

supervisorctl start shevastream
