# Kubernetes PowerShell Drive (aka K8sPSDrive)

This repository plays around the idea of exposing Kubernetes cluster as a PowerShell PSDrive.

This is binary PowerShell module, which means it is written in C#.
This is purely experimental and a pet project of mine to learn C# and Kubernetes together.

Tested on PowerShell 7 and built on .NET Core 3.1

## Clone

Clone this repository

    git clone https://github.com/DexterPOSH/K8sPSDrive.git

## Building

Run the build script in the root of the project to install dependent modules and start the build

    .\build.ps1

To just run the build, execute Invoke-Build

    Invoke-Build

    # or do a clean build
    Invoke-Build Clean,Default


Install dev version of the module on the local system after building it.

    Invoke-Build Install