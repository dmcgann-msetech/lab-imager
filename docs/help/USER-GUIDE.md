# Lab Imager User Guide

## 1. Introduction

Lab Imager is a Windows-native imaging workstation developed for microscope inspection, electronics diagnostics, repair documentation, evidence collection, and laboratory workflows.

Unlike general-purpose webcam software, Lab Imager is designed around technical inspection workflows where captured images, technician notes, metadata, and recordings become part of a repeatable documentation process.

Typical use cases include:

* PCB inspection
* Soldering documentation
* HDMI microscope workflows
* Failure analysis
* Repair verification
* Timestamped evidence collection
* Laboratory documentation
* Technical reporting

---

## 2. System Requirements

### Operating System

* Windows 10
* Windows 11

### Runtime

* .NET 8 Desktop Runtime

### Hardware

* DirectShow-compatible camera
* USB microscope
* HDMI capture device
* Inspection camera

---

## 3. First Launch

When Lab Imager launches:

1. Select a camera source.
2. Select a capture format.
3. Start Preview.
4. Verify image quality.
5. Enter session information.

Recommended session information:

* Project
* Board / Device
* Technician
* Notes

---

## 4. User Interface Overview

### Preview Viewport

Displays the live or frozen camera image.

### Source Selector

Selects the active camera.

### Format Selector

Selects camera resolution and format.

### Session Panel

Stores metadata associated with captures.

### Notes Editor

Used to document findings, observations, and repair notes.

### Transport Controls

Provides:

* Start Preview
* Stop Preview
* Freeze
* Resume
* Record
* Capture

---

## 5. Camera Operations

### Camera Selection

Select the desired camera from the Source dropdown.

Supported devices include:

* USB microscopes
* HDMI capture cards
* USB cameras
* Inspection cameras

### Capture Format Selection

Choose the desired resolution and video format.

Higher resolutions generally provide improved inspection detail.

---

## 6. Live Preview

### Start Preview

Starts the active camera stream.

### Stop Preview

Stops the active preview session.

### Freeze

Locks the currently displayed image.

### Resume

Returns to live preview mode.

---

## 7. Session Metadata

Session metadata provides context for evidence captures.

Recommended fields:

### Project

Project identifier or work order.

### Board / Device

Device under inspection.

### Technician

Individual performing the work.

### Notes

Observations, findings, repairs, and validation results.

---

## 8. Notes Editor

The Notes Editor supports:

* Font selection
* Font sizing
* Bold
* Italic
* Underline
* Colors
* Alignment
* Bulleted lists
* Numbered lists

Recommended note content:

* Defects found
* Repairs performed
* Measurements
* Validation results
* Additional observations

---

## 9. Evidence Capture

Evidence capture generates documentation artifacts.

Current outputs may include:

* Original PNG
* Evidence PNG
* HTML evidence report

Future outputs:

* Evidence PDF package

Captured information may include:

* Timestamp
* Project
* Device
* Technician
* Notes
* Camera information

---

## 10. Recording

Lab Imager supports AVI recording.

Recording may be used to document:

* Inspections
* Repairs
* Demonstrations
* Validation procedures

Future enhancements:

* Playback
* Timeline navigation
* Review workflows

---

## 11. Troubleshooting

### No Camera Found

Verify:

* Device connection
* Device Manager visibility
* Driver installation

### Preview Does Not Start

Verify:

* Camera selected
* Format selected
* Device not in use elsewhere

### Recording Fails

Verify:

* Disk space
* Folder permissions
* Camera availability

---

## 12. Best Practices

* Verify metadata before capture.
* Use descriptive technician notes.
* Preserve original evidence files.
* Validate image quality before recording.
* Maintain consistent project naming.

---

## 13. Additional Documentation

* HELP.md
* FAQ.md
* CHANGELOG.md
* RELEASE-NOTES.md
* LICENSE.md
