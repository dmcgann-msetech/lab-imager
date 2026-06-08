# Lab Imager

Windows-native microscope and HDMI capture workstation for electronics inspection, soldering documentation, timestamped evidence capture, and laboratory workflows.

---

## Overview

Lab Imager is a Windows-native engineering workstation designed for electronics inspection, microscope imaging, evidence capture, annotation workflows, repair documentation, and laboratory record generation.

Unlike general-purpose camera software, Lab Imager is focused on bench-top technical work where images, notes, metadata, and future recordings must be captured as part of a repeatable engineering workflow.

Primary use cases include:

* PCB inspection
* Soldering documentation
* HDMI microscope workflows
* Electronics diagnostics
* Timestamped evidence collection
* Laboratory documentation
* Repair and rework validation
* Technical reporting and evidence generation

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
* Start preview
* Stop preview
* Freeze frame
* Resume preview

### Evidence Capture

* PNG image capture
* Timestamped evidence export
* Metadata embedding
* Evidence PDF generation
* Original image preservation
* Multiple evidence captures per session

### Session Metadata

* Project field
* Board / Device field
* Technician field
* Multi-line notes
* Session documentation workflow

---

## Validation Status

### Validated

* Camera enumeration
* Camera selection
* Live preview
* Freeze frame
* Resume preview
* Evidence capture
* Metadata workflow
* Multiple captures per session
* Capture while preview is running
* Capture after freeze
* Capture after resume
* Capture after camera reselection
* Capture format selection UI

### Under Active Validation

* Capture format application to preview graph
* Preview graph reconfiguration
* Recording graph architecture
* DirectShow recording pipeline integration

---

## Project Status

### Current Milestone

**M1 – Evidence Capture and Camera Integration**

### Completed

* Camera detection
* Live preview workflow
* Freeze frame workflow
* Resume preview workflow
* Evidence capture workflow
* PNG evidence export
* Evidence PDF generation
* Metadata integration
* Multiple evidence captures per session
* Capture format selection UI
* Camera persistence
* Engineering documentation framework

### In Progress

* Capture format application and validation within preview graph
* DirectShow recording pipeline implementation
* Recording graph stabilization
* Smart Tee architecture validation
* Preview graph reliability testing

### Planned

* Recording export workflow
* Session persistence enhancements
* Screenshot annotation export
* Combined PNG and PDF evidence packaging
* Playback and review workflow
* Release packaging
* Installer generation

---

## Requirements

* Windows 10 or Windows 11
* .NET 8 Desktop Runtime
* DirectShow-compatible camera or HDMI capture device

---

## Build

```bash
git clone https://github.com/dmcgann-msetech/lab-imager.git

cd Lab-Imager

dotnet restore

dotnet build src\LabImager\LabImager.csproj

dotnet run --project src\LabImager\LabImager.csproj
```

---

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
├─ cover.html
├─ roadmap.html
├─ lab-imager-log.html
└─ assets/

tools/
└─ RecordingProbe/
```

---

## Project Documentation

The project maintains three primary engineering documents:

### docs/cover.html

Project cover page and repository documentation front matter.

### docs/roadmap.html

Active roadmap, backlog tracking, milestone planning, and implementation priorities.

### docs/lab-imager-log.html

Engineering history, architecture decisions, validation records, implementation checkpoints, troubleshooting records, and milestone progression.

---

## Development Branch

The primary development branch is:

```text
feature/wpf-frame-pipeline
```

---

## Development Status

This project is currently under active development.

Features and architecture may change between milestones as validation and hardware testing continue.

---

## Ownership and Copyright

Lab Imager is developed and maintained by MSE McGann Systems Engineering.

Copyright © 2026 MSE McGann Systems Engineering.

All rights reserved.

This repository is publicly visible for development transparency, project demonstration, portfolio presentation, testing, and engineering documentation purposes.

The Software is proprietary and is not open source.

No permission is granted to copy, modify, redistribute, sublicense, commercialize, create derivative works from, or otherwise use the Software except as expressly authorized in writing by MSE McGann Systems Engineering.

See `LICENSE.md` for full licensing terms.
