# Kubernetes PowerShell Drive (aka K8sPSDrive)

This repository plays around the idea of exposing Kubernetes cluster as a PowerShell PSDrive.

This is binary PowerShell module, which means it is written in C#.
This is purely experimental and a pet project of mine to learn C# and Kubernetes together.

Tested on PowerShell Core and built on .NET Core 3.1

## Pre-requisites

- [Install latest .NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)
- [Install PowerShell Core](https://github.com/powershell/powershell)
- Valid kubeconfig set in default path `~/.kube/config` with access to a K8s cluster setup.

## Clone

Clone this repository

    git clone https://github.com/DexterPOSH/K8sPSDrive.git

## Building

Run the build script in the root of the project to install dependent modules and start the build.ps1 specifying bootstrap mode.
This will restore the dotnet cake tool along with installing required PowerShell modules.

```powershell
./build.ps1 -BootStrapOnly
```

To just run the build, execute 

```powershell  
./build.ps1 
```

If you encounter (Permission denied) issues with above use dotnet cake tool in your shell (bash/zsh)

```bash
# or run dotnet cake from the repository root
dotnet cake ./build.cake --target="Build"
```

Once the module is built the artifacts are placed under `ci/artifacts` folder.
In the PowerShell console open the repository root and run below commands.


Note - Mapping a K8sPSDrive uses the default KubeConfig at `~/.kube/config` , so if that is not set it will throw an error

```powershell
Import-Module -Name  ./ci/artifacts/K8sPSDrive.psd1 -Verbose
New-PSDrive -Name K8sPSDrive -PSProvider SHiPS -Root K8sPSDrive#K8sPSDrive.Root
```

If the drive is mapped, you can set the location  using the name `K8sPSDrive` specified earlier.

```powershell
Set-Location -Path K8sPSDrive:/
```

Once inside the PSDrive you can list the childrens.

```powershell
 Get-ChildItem
```

```Output
PS K8sPSDrive:\> Get-ChildItem


    Directory: K8sPSDrive:


Mode  Name
----  ----
+     namespaces
+     nodes
```

Now, one can change directory and move down the path under `namespaces`

```powershell
cd .\namespaces\
```

Under namespaces folder list all the namespaces

```powershell
Gte-ChildItem
```

```output
PS K8sPSDrive:\namespaces> ls


    Directory: K8sPSDrive:\namespaces


Mode NAME            STATUS AgeInDays
---- ----            ------ ---------
+    default         Active 16
+    kube-node-lease Active 16
+    kube-public     Active 16
+    kube-system     Active 16
```

Go inside a namespace and you'll see namspaced objects e.g. `Deployments` and `Pods` currently and list them.

```powershell
cd ./kubec-system/pods; ls
```

```output
PS K8sPSDrive:\namespaces> cd .\kube-system\pods\

PS K8sPSDrive:\namespaces\kube-system\pods> ls


    Directory: K8sPSDrive:\namespaces\kube-system\pods


Mode NAME                                   Ready Status  Restarts AgeInDays
---- ----                                   ----- ------  -------- ---------
.    coredns-f9fd979d6-qkrgm                1/1   Running 0        16
.    coredns-f9fd979d6-wl27c                1/1   Running 0        16
.    etcd-docker-desktop                    1/1   Running 0        0
.    kube-apiserver-docker-desktop          1/1   Running 0        0
.    kube-controller-manager-docker-desktop 1/1   Running 0        0
.    kube-proxy-2lwq6                       1/1   Running 0        16
.    kube-scheduler-docker-desktop          1/1   Running 0        0
.    storage-provisioner                    1/1   Running 0        16
.    vpnkit-controller                      1/1   Running 0        16
```