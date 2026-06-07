# Lab Imager

Windows-native microscope and HDMI capture workstation for electronics inspection, soldering documentation, timestamped evidence capture, and laboratory workflows.

---

## Overview

Lab Imager is a WPF desktop application designed to streamline the capture and documentation of electronics inspection work.

The application combines live camera preview, freeze-frame inspection, evidence capture, metadata tracking, and future recording capabilities into a single workstation-oriented interface.

Primary use cases include:

* PCB inspection
* Soldering documentation
* HDMI microscope workflows
* Electronics diagnostics
* Timestamped evidence collection
* Laboratory documentation

---

## Current Features

### Camera Management

* DirectShow camera enumeration
* Camera source selection
* Default camera persistence
* Capture format enumeration
* Capture format selection

### Live Preview

* Live camera preview
* Start / Stop preview controls
* Freeze frame support
* Resume preview support

### Evidence Capture

* PNG image capture
* Timestamped evidence export
* Metadata embedding
* Evidence PDF generation
* Original image preservation

### Session Metadata

* Project field
* Board / Device field
* Technician field
* Notes support
* Multi-line notes support

### Validation Status

Validated:

* Camera enumeration
* Live preview
* Freeze frame
* Resume preview
* Evidence capture
* Metadata workflow
* Multiple captures per session
* Capture while preview running
* Capture after freeze
* Capture after resume
* Capture after camera reselection

---

## Project Status

Current Milestone: M1

Completed:

* Camera detection
* Live preview
* Freeze frame workflow
* Evidence capture workflow
* Metadata integration
* Capture format selection UI

In Progress:

* Capture format application to preview pipeline
* Recording pipeline implementation

Planned:

* Recording export workflow
* Session persistence enhancements
* Release packaging

---

## Requirements

* Windows 10 or Windows 11
* .NET 8 Desktop Runtime
* DirectShow-compatible camera or HDMI capture device

---

## Build

```powershell
git clone https://github.com/dmcgann-msetech/lab-imager.git

cd Lab-Imager

dotnet restore

dotnet build src\LabImager\LabImager.csproj

dotnet run --project src\LabImager\LabImager.csproj
```

## Repository Structure

```text
src/
└─ LabImager/
   ├─ Models/
   ├─ Services/
   │  ├─ Camera/
   │  ├─ Capture/
   │  ├─ Preview/
   │  ├─ Recording/
   │  └─ Settings/
   ├─ MainWindow.xaml
   └─ MainWindow.xaml.cs

docs/
├─ lab-imager-log.html
└─ assets/

tools/
└─ RecordingProbe/
```

---

## Development Notes

This project is currently under active development.

The primary development branch is:

```text
feature/wpf-frame-pipeline
```

Engineering history, validation records, architecture notes, and milestone tracking are maintained in:

```text
docs/lab-imager-log.html
```
