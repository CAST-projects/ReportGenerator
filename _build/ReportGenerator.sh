#!/bin/sh
cd `dirname $0`
dotnet CastReporting.Console.Core.dll "$@"
