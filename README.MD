# BiztalkPowershellAdapter

This is a custom BizTalk in-process send adapter for running powershell scripts/commands from BizTalk on local or remote servers.

## Introduction

This code implements an async sendport adapter to Microsoft BizTalk Server 2016 for runnning powershell scripts/commands locally or at remote server.

## Requirements

Things needed for building:
- Microsoft BizTalk Server 2016 Developer full installation with SDK.
- Visual Studio 2015 or later.

## Usage

Normal procedures for creating send ports in BizTalk.
Pick 'Powershell' for the type of send port adapter.

### Send port settings

Send port configuration
- Script: powershell script to be running
- Host: computername of the server where script should be run. Not used when running script in local server
- Username: user for running remote scripts, if needed
- Password: password for user

### Passing message to script

Adapter passes the message coming to send port to script as a parameter $message (local scripts) or $xmlmessage (remote running scripts).

Example of how to read contents of $message in script:
```
# read message body (xml) to variable
$xml = [xml] $message.GetBody()
# get element value from xml
$param = $xml.PowershellAdapter.Parameter1 
```

Example of how to read contents of $xmlmessage in script and pass it to cmd-file in remote server: 
```
# get the $xmlmessage param as xml
param([xml]$xmlmessage)
# get element value from xml
$param = $xmlmessage.PowershellAdapter.Parameter1
# pass $param value to cmd-file and run it
cmd.exe /c 'C:\Test\run.bat' $param 
```

## Installation

Required steps for installing the adapter
- Build solution
  - notice the location of Microsoft.Samples.BizTalk.Adapter.Common -project
- Run PowershellAdapterSetup.msi from Installer-projects' output
- Register adapter to BizTalk with BizTalk Administration Console
  - Platform Settings -> Adapters -> New -> Adapter
    - Name: Powershell
    - Adapter: PowershellAdapter

## TODO

- Needs extensive testing
- Better install and usage instructions
- Include install binary to repo
- Must try it on BizTalk 2013 R2 and BizTalk 2020
