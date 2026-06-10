# Lab Imager Release Notes

This document contains release-specific notes intended for testers, evaluators, and future production deployments.

For detailed implementation history, architecture decisions, troubleshooting records, validation logs, and development milestones, see:

* CHANGELOG.md
* docs/lab-imager-log.html
* docs/roadmap.html

---

# Unreleased

## Current Development Branch

feature/wpf-frame-pipeline

## Current Status

Lab Imager remains under active development.

The application has progressed from a DirectShow preview prototype into a functional imaging workstation supporting:

* Camera detection
* Camera selection
* Capture format selection
* Live preview
* Freeze frame
* Resume preview
* Evidence capture
* Evidence PNG generation
* Original PNG preservation
* HTML evidence reporting
* Rich Notes documentation
* AVI recording

No production release has been issued at this time.

---

## Recently Completed

### Rich Notes Modernization

Completed:

* Rich text formatting
* Font family support
* Font size support
* Text color support
* Alignment controls
* Bulleted lists
* Numbered lists
* Clear formatting
* Selection preservation
* Rich notes evidence rendering

Additional improvements:

* Dropdown-based toolbar architecture
* Formatting menu
* Lists menu
* Alignment menu
* Color menu
* Improved evidence rendering fidelity

Resolved:

* Selection loss during formatting operations
* Font-size rendering inconsistencies
* Long-note clipping during export
* Dropdown readability issues

---

### Integrated Recording Pipeline

Completed:

* AVI recording support
* Recording controls
* Recording graph architecture
* Recording file generation
* Recording validation workflow

Resolved:

* Recording graph instability
* Preview interruption during recording
* Recording output generation failures

Validated:

* Start recording
* Stop recording
* Multiple recordings per session
* Recording after freeze/resume
* Recording after capture-format changes

---

### Capture Format Architecture

Completed:

* Capture format enumeration
* Device-specific format population
* Capture format selection
* Format persistence
* Preview graph format application

---

### Evidence Workflow

Completed:

* PNG capture workflow
* Evidence PNG generation
* Original PNG preservation
* Metadata integration
* HTML evidence reporting

---

## Planned For Next Release

### Annotation Preservation Validation

Validation targets:

* Annotation positioning
* Annotation scaling
* Annotation rendering fidelity
* Mixed annotation support
* Multi-layer annotation support

Verification:

* Evidence PNG contains annotations
* Annotation positioning matches viewport
* Annotation scaling matches viewport
* Annotation export remains accurate

---

### Evidence Packaging Workflow

Planned outputs:

* Original PNG
* Evidence PNG
* Evidence PDF package

Future package contents:

* Screenshot
* Annotations
* Metadata
* Notes
* Timestamp
* Camera information
* Capture format information

---

### Save Naming Standardization

Planned improvements:

* Metadata-aware filenames
* Invalid-character sanitization
* Consistent naming rules

Target artifacts:

* Original PNG
* Evidence PNG
* Evidence PDF
* AVI recordings

---

### Recording Hardening

Additional validation planned:

* Long-duration recording
* Repeated start/stop cycles
* Camera switching while recording
* Format switching while recording
* Extended stability testing

---

## Known Issues

Current non-blocking warnings:

### DirectShowCameraPreviewService

CS8602 possible null dereference

### DirectShowCameraDeviceService

CS8602 possible null dereference

These warnings are currently documented and are not considered release blockers.

---

## Release Template

Version:
Release Date:
Build Number:

### Summary

### Added

### Changed

### Fixed

### Known Issues

### Validation

### Upgrade Notes
