# LabImager

Windows-native microscope and HDMI capture workstation for electronics inspection, soldering documentation, timestamped evidence capture, and laboratory workflows.

---

## Overview

LabImager is a Windows-native engineering workstation designed for electronics inspection, microscope imaging, evidence capture, annotation workflows, repair documentation, and laboratory record generation.

Unlike general-purpose camera software, LabImager is focused on bench-top technical work where images, notes, metadata, and supporting documentation must be captured as part of a repeatable engineering workflow.

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

## Release Information

**Current Version:** v1.0.0

**Status:** Released

**Release Date:** June 2026

**Distribution:** GitHub Releases

**Installer:** LabImager v1.0 Setup.exe

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
* Freeze preview
* Resume preview

### Evidence Capture

* Original PNG capture
* Evidence PNG generation
* Evidence PDF export
* Timestamped evidence workflow
* Metadata integration
* Original image preservation
* Multiple evidence captures per session

### Session Metadata

* Project field
* Board / Device field
* Technician field
* Multi-line notes
* Session documentation workflow

### Rich Notes Editor

* Font selection
* Font sizing
* Text formatting
* Text alignment
* Bullet lists
* Numbered lists
* Annotation preservation

### Help Center

* Integrated Help Center
* User Guide
* FAQ
* Keyboard Shortcuts
* Release Notes
* About Page
* License Viewer

### Documentation Framework

* Engineering logbook
* Roadmap tracking
* Release documentation
* Governance documentation
* Offline packaged documentation

---

## Validation Status

### Release Validated

* Camera enumeration
* Camera selection
* Capture format selection
* Live preview
* Freeze preview
* Resume preview
* Evidence capture
* Evidence PDF export
* Metadata workflow
* Multiple captures per session
* Capture while preview is running
* Capture after freeze
* Capture after resume
* Capture after camera reselection
* Rich Notes editor
* Integrated Help Center
* Installer packaging

### Deferred Validation

Recording functionality has been deferred beyond Version 1.0 and will be revisited during future development cycles.

---

## Project Status

### Current Release

**LabImager v1.0**

Status:

Released

### Completed

* Camera detection
* Camera persistence
* Camera format selection
* Live preview workflow
* Freeze preview workflow
* Resume preview workflow
* Evidence capture workflow
* Original PNG preservation
* Evidence PDF generation
* Metadata integration
* Multiple evidence captures per session
* Rich Notes editor modernization
* Integrated Help Center
* About page
* Documentation framework
* Installer packaging
* GitHub release management
* Main branch release merge
* Version tagging and release process

### Post-Release Development

#### Version 1.0.1

* Remove obsolete Settings gear from main UI
* Repository cleanup review
* Nullable reference warning cleanup
* General UI polish

#### Version 1.1

* Disable camera source changes while preview is active
* Disable capture format changes while preview is active
* Prompt user before preview graph reconfiguration
* Recording hardening
* Recording validation
* Recording workflow stabilization

#### Future Releases

* Recording playback
* Rewind controls
* Frame stepping
* Timeline navigation
* Zoom and pan viewport enhancements
* Additional evidence workflow improvements

---

## Requirements

### Operating System

* Windows 10
* Windows 11

### Runtime

* .NET 8 Desktop Runtime

### Hardware

* DirectShow-compatible camera
* HDMI microscope
* HDMI capture device

---

## Installation

Download the latest installer from the GitHub Releases page.

Install:

```text
LabImager v1.0 Setup.exe
```

Follow the installer wizard to complete installation.

---

## Build From Source

```bash
git clone https://github.com/dmcgann-msetech/lab-imager.git

cd lab-imager

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
   │  ├─ Reporting/
   │  └─ Help/
   ├─ Views/
   ├─ Templates/
   ├─ MainWindow.xaml
   └─ MainWindow.xaml.cs

docs/
├─ application/
├─ governance/
├─ help/
├─ releases/
├─ assets/
├─ cover.html
├─ roadmap.html
└─ lab-imager-log.html

tools/
└─ RecordingProbe/
```

---

## Project Documentation

The project maintains three primary engineering documents:

### docs/cover.html

Project cover page and repository documentation front matter.

### docs/roadmap.html

Active roadmap, backlog tracking, release planning, and future development tracking.

### docs/lab-imager-log.html

Engineering history, architecture decisions, validation records, implementation checkpoints, troubleshooting records, release milestones, and development progression.

---

## Branch Strategy

### Primary Branch

```text
main
```

### Historical Release Branch

```text
feature/wpf-frame-pipeline
```

Retained for historical reference, rollback support, and development history preservation.

---

## Development Status

LabImager v1.0 has been released.

The application is currently in active maintenance and enhancement mode, with future functionality planned for Version 1.0.1, Version 1.1, and later releases.

---

## Ownership and Copyright

LabImager is developed and maintained by MSE McGann Systems Engineering.

Copyright © 2026 MSE McGann Systems Engineering.

All rights reserved.

This repository is publicly visible for development transparency, project demonstration, portfolio presentation, testing, and engineering documentation purposes.

The Software is proprietary and is not open source.

No permission is granted to copy, modify, redistribute, sublicense, commercialize, create derivative works from, or otherwise use the Software except as expressly authorized in writing by MSE McGann Systems Engineering.

See LICENSE.md for full licensing terms.
