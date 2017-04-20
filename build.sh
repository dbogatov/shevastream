#!/bin/bash
set -e

# Ensure that the CWD is set to script's location
cd "${0%/*}"
CWD=$(pwd)

## TASKS

install-doc-generators () {

	cd $CWD

	echo "Installing node modules... Requires Yarn"
	yarn --ignore-engines > /dev/null

}

gen-server-docs () { 

	cd $CWD

	echo "Generating server side code documentation... Requires Doxygen"
	
	rm -rf src/wwwroot/docs/server
	mkdir -p src/wwwroot/docs/server
	doxygen Doxyfile > /dev/null
	mv src/wwwroot/docs/server/html/* src/wwwroot/docs/server

}

gen-client-docs () { 

	cd $CWD

	echo "Generating client side code documentation... Requires TypeDoc (Installed by Yarn)"
	mkdir -p src/wwwroot/docs/client
	$(yarn bin)/typedoc --logger none client/ts/ > /dev/null
	printf "\n"
	
}

gen-api-docs () {

	cd $CWD

	echo "Generating API documentation... Requires Spectacle (Installed by Yarn)"
	mkdir -p src/wwwroot/docs/api
	$(yarn bin)/spectacle api.yml -t src/wwwroot/docs/api > /dev/null

}

install-client-libs () {

	cd $CWD/client

	echo "Installing front-end libraries... Requires Yarn"
	yarn --ignore-engines > /dev/null

}

install-typings () {

	cd $CWD

	echo "Installing TypeScript typings... Requires Typings (Installed by Yarn)"
	mkdir -p client/typings
	$(yarn bin)/typings install > /dev/null

}

generate-client-bundle () {

	cd $CWD

	echo "Bundling front-end libraries... Requires Webpack (Installed by Yarn)"
	rm -rf src/wwwroot/{js,css}/*

	$(yarn bin)/webpack --context client/ --env prod --config client/webpack.config.js --output-path src/wwwroot/js > /dev/null

	mkdir -p src/wwwroot/css/
	mv src/wwwroot/js/app.css src/wwwroot/css/
	rm src/wwwroot/js/*.map
	rm src/wwwroot/js/less.*

}

generate-ccs-libs () {

	cd $CWD/client/lib/css

	echo "Generating CSS libs... Requires Yarn"
	rm -rf $CWD/src/wwwroot/lib/css/*

	yarn --ignore-engines > /dev/null

	mkdir -p $CWD/src/wwwroot/lib/css
	cp -r node_modules/* $CWD/src/wwwroot/lib/css

}

generate-js-libs () {

	cd $CWD/client/lib/js

	echo "Generating JS libs..."
	rm -rf $CWD/src/wwwroot/lib/js/*

	mkdir -p $CWD/src/wwwroot/lib/js
	cp -r ./* $CWD/src/wwwroot/lib/js

}

restore-dotnet () {

	cd $CWD/src

	echo "Restoring .NET dependencies... Requires .NET SDK"
	dotnet restore > /dev/null

}

build-dotnet () {

	cd $CWD/src

	echo "Building and publishing .NET app... Requires .NET SDK"
	dotnet publish -c release > /dev/null

}

build-dev-client () {

	cd $CWD/client

	echo "Cleaning dist/"
	rm -rf dist/

	echo "Copying source files"
	mkdir -p dist/{ts,less}
	cp -R ts/* dist/ts/
	cp -R less/* dist/less/

	echo "Bundling front-end libraries... Requires Webpack (Installed globally)"
	webpack --env dev --display-error-details --output-path ./dist/ts

	echo "Copying generated code"
	mkdir -p ../src/wwwroot/js/ts
	mkdir -p ../src/wwwroot/css
	cp -R dist/ts/* ../src/wwwroot/js/ts/
	cp dist/ts/app.css ../src/wwwroot/css/app.css

	echo "Removing temporary directory"
	rm -rf dist/

	echo "Done!"
}

## APP BUILDERS

build-app-parallel () {

	install-doc-generators &
	install-client-libs &
	restore-dotnet &
	gen-server-docs &
	generate-ccs-libs &
	generate-js-libs &

	wait %install-doc-generators
	
	gen-client-docs &
	gen-api-docs &

	wait %install-client-libs
	
	install-typings &

	wait %install-typings
	
	generate-client-bundle &

	wait
	
	build-dotnet

	echo "Build completed!"

}

build-app-sequential () {

	install-doc-generators
	install-client-libs

	gen-server-docs
	gen-client-docs
	gen-api-docs

	install-typings
	generate-client-bundle
	generate-ccs-libs
	generate-js-libs
	restore-dotnet

	build-dotnet

	echo "Build completed!"

}

usage () { 
	echo "Usage: $0 [-d -f <string>]" 1>&2 
}

while getopts "f:d" o; do
	case "${o}" in
		f)
			eval $OPTARG
			exit 0
			;;
		d)
			build-dev-client
			exit 0
			;;
		h)
			usage
			exit 0
			;;
		*)
			usage
			exit 1
			;;
	esac
done
shift $((OPTIND-1))

build-app-parallel
