# Windows Packer Templates

<!-- TOC depthFrom:2 -->

- [Introduction](#introduction)
    - [Supported Builders](#supported-builders)
    - [Supported Operating Systems](#supported-operating-systems)
    - [Pre-build Images](#pre-build-images)
- [Prepare your system to run Packer](#prepare-your-system-to-run-packer)
    - [Ubuntu](#ubuntu)
    - [Windows](#windows)
- [Running the Build Script](#running-the-build-script)
    - [Building Vagrant Cloud Images](#building-vagrant-cloud-images)
    - [Building Hyper-V Images](#building-hyper-v-images)
    - [Building VirtualBox Images](#building-virtualbox-images)
- [Using the Vagrant Images](#using-the-vagrant-images)

<!-- /TOC -->

## Introduction
This repository contains build scripts to golden images using Packer.

Interested in some best practices when using Packer with Windows? Check out [my blog post on the topic](https://hodgkins.io/best-practices-with-packer-and-windows).

### Supported Builders

The supported builds are:
* VirtualBox
* Hyper-V

### Supported Operating Systems

The `build.supported_os.yaml` file contains the list of Operating Systems that are supported and their settings.

The supported Operating Systems to build are:
* Windows2012R2Core
* Windows2012R2
* Windows2016StdCore
* Windows2016Std

### Pre-build Images

If you just want to download the pre-build Vagrant images, download them from [Vagrant Cloud](https://app.vagrantup.com/MattHodge).

## Prepare your system to run Packer

Before you can run the build scripts, you need to prepare your system.

### Ubuntu

> :white_check_mark: Tested on Ubuntu 16.04

Mono is required to run the build script.

```bash
# Install Mono
sudo apt-get install mono-complete -y

# Give the script execute permission
chmod +x build.sh
```

Additionally you will need to install:

* Packer
* VirtualBox

### Windows

> :white_check_mark: Tested on Windows 10 Pro

```powershell
# Set PowerShell Execution Policy
Set-ExecutionPolicy RemoteSigned -Force

# Install Chocolatey
iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex

# Install Packer
choco install packer -y

# Install Hyper-V
Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All

# Create an external Hyper-V Switch
# Commands may vary depending on your system.
Get-NetAdapter

# Find the name of the network adapter (make sure its status is also not disconnected)

# Create a new VM Switch using the name of the network adapter
New-VMSwitch -Name "External VM Switch" -AllowManagementOS $true -NetAdapterName "<Your Adapter Name Here>"
```

## Running the Build Script

Depending on your platform, you either need to run:
* `build.ps1` on Windows
* `build.sh` on Linux / MacOS.

### Building Vagrant Cloud Images
If you are building images for Vagrant Cloud, you need to set the following environment variables:

```powershell
# Windows

$env:ATLAS_TOKEN = "ABC123XYZ"

$env:ATLAS_USERNAME = "MattHodge" # Username to upload the boxes under

$env:ATLAS_VERSION = "1.0.1" # Version of the box being uploaded
```

```bash
# MacOS / Linux

export ATLAS_TOKEN="ABC123XYZ"

export ATLAS_USERNAME="MattHodge" # Username to upload the boxes under

export ATLAS_VERSION="1.0.1" # Version of the box being uploaded
```


### Building Hyper-V Images

The following commands will build you Hyper-V Images

```powershell
# Builds Windows 2016 Standard Core and runs the Vagrant post processor (local).
.\build.ps1 -Target "hyperv-local" -os="Windows2016StdCore"

# Builds Windows 2012 R2 Core and runs the Atlas post processor.
.\build.ps1 -Target "hyperv-vagrant-cloud" --os="Win2012R2Core"
```

### Building VirtualBox Images

The following commands will build you VirtualBox Images

```bash
# Builds Windows 2016 Standard Core and runs the Vagrant post processor (local).
./build.sh --target "virtualbox-local" -os="Windows2016StdCore"

# Builds Windows 2012 R2 Core and runs the Atlas post processor.
./build.sh --target "virtualbox-vagrant-cloud" -os="Win2012R2Core"
```

## Using the Vagrant Images

If you aren't pushing your boxes to Atlas, you can import the `*.box` files for use in Vagrant:

```powershell
vagrant box add .\win2016stdcore-wmf5-nocm-hyperv.box --name Windows2016StdCore
```

You can also find all the boxes ready to be used with `vagrant up` over at my [VagrantBoxes Repository](https://github.com/MattHodge/VagrantBoxes).
