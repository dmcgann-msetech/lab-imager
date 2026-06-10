# Dependency Audit

| Dependency | Purpose |
|------------|---------|
| .NET 8 Desktop Runtime | Runtime |
| WPF | UI |
| DirectShowLib | Camera Integration |
| PDF Library | PDF Generation |


# Dependency Audit

## Purpose

This document tracks all external frameworks, libraries, runtimes, and supporting technologies used by Lab Imager.

The objective is to:

* Understand third-party dependencies
* Track licensing requirements
* Identify upgrade risks
* Identify replacement strategies
* Support future release packaging
* Support long-term maintainability

---

# Runtime Dependencies

## .NET 8 Desktop Runtime

### Purpose

Primary application runtime.

Provides:

* WPF framework support
* Core CLR execution
* Base class libraries
* Windows desktop integration

### Vendor

Microsoft

### License

Microsoft .NET License

### Required

Yes

### Replacement Difficulty

Critical

### Risk Assessment

High

Removal of .NET would require a complete application rewrite.

### Upgrade Considerations

Future migration expected:

* .NET 9
* .NET 10 LTS

Pending Microsoft support lifecycle.

---

# User Interface Dependencies

## Windows Presentation Foundation (WPF)

### Purpose

Primary desktop user interface framework.

Provides:

* Windows
* Controls
* Layout engine
* RichTextBox
* Styling
* Data binding

### Vendor

Microsoft

### License

Microsoft

### Required

Yes

### Replacement Difficulty

Critical

### Risk Assessment

High

Replacement would require complete UI redevelopment.

### Current Usage

* MainWindow
* Notes editor
* Viewport controls
* Session panel
* Toolbar controls
* Settings UI

---

# Camera and Video Dependencies

## DirectShow

### Purpose

Primary camera integration framework.

Provides:

* Camera enumeration
* Device capabilities
* Live preview
* Capture format discovery
* Recording graph construction

### Vendor

Microsoft

### Required

Yes

### Risk Assessment

High

Entire imaging subsystem depends on DirectShow.

### Current Usage

* Camera detection
* Preview graph
* Recording graph
* Format selection

### Notes

DirectShow remains stable for current project requirements.

Migration to Media Foundation is not currently planned.

---

## DirectShowLib

### Purpose

Managed .NET wrapper around DirectShow COM interfaces.

### Vendor

Open Source Community

### License

MIT

### Required

Yes

### Risk Assessment

Medium

Project depends heavily on this library for DirectShow interoperability.

### Current Usage

* IBaseFilter
* ICaptureGraphBuilder2
* IGraphBuilder
* ISampleGrabber
* IAMStreamConfig
* Video capture interfaces

### Replacement Strategy

Direct COM interop implementation if project maintenance ceases.

---

# Imaging Dependencies

## Windows Imaging Components (WIC)

### Purpose

Image encoding and image export.

### Current Usage

* PNG generation
* Bitmap conversion
* Image persistence

### Vendor

Microsoft

### Required

Yes

### Risk Assessment

Low

Included with Windows.

---

## RenderTargetBitmap

### Purpose

WPF visual rendering for evidence generation.

### Current Usage

* Notes rendering
* Evidence image generation
* Export workflows

### Vendor

Microsoft

### Risk Assessment

Low

Core WPF component.

---

# Reporting Dependencies

## HTML Report Generator

### Purpose

Evidence report generation.

### Current Usage

* Evidence reports
* Metadata reports
* Documentation exports

### Vendor

Internal

### License

Proprietary

### Risk Assessment

Low

Maintained directly within Lab Imager.

---

## PDF Generation Library

### Purpose

Evidence package generation.

### Current Status

Under evaluation / implementation.

### Required

Future

### Risk Assessment

Medium

Final dependency selection pending.

### Evaluation Criteria

* License compatibility
* Image rendering support
* Metadata support
* Long-term maintenance

---

# Operating System Dependencies

## Windows 10

Supported

### Minimum Version

Windows 10 22H2 Recommended

---

## Windows 11

Supported

### Recommended

Windows 11 24H2 or newer

---

# Development Dependencies

## Visual Studio 2022

### Purpose

Primary development environment.

### Usage

* Development
* Debugging
* Validation

---

## Git

### Purpose

Source control.

### Usage

* Repository management
* Branching
* Release management

---

# Future Dependency Review Items

The following areas should be reviewed before Version 1.0:

* PDF generation library selection
* Installer generation framework
* Auto-update framework (if implemented)
* Logging framework standardization
* Recording codec strategy

---

# Audit Summary

Current External Dependency Count:

Runtime:

* .NET 8

UI:

* WPF

Video:

* DirectShow
* DirectShowLib

Imaging:

* WIC

Development:

* Visual Studio
* Git

Overall Risk Assessment:

Moderate

No known dependency currently presents a release-blocking licensing concern.
