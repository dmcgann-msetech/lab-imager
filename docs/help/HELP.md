# Lab Imager Help

## Overview

Lab Imager is a Windows-native microscope and HDMI capture workstation designed for electronics inspection, laboratory documentation, evidence collection, troubleshooting, repair verification, and recording workflows.

The application combines live camera preview, freeze-frame inspection, annotations, evidence capture, technician notes, metadata collection, PDF reporting, and video recording into a single workstation-oriented environment.

---

# Quick Start

Follow these steps to capture evidence in under one minute.

## Basic Workflow

1. Select a camera from the **Source** dropdown.
2. Select a **Capture Format**.
3. Click **Start Preview**.
4. Position the device under inspection.
5. Click **Freeze** when the desired image is visible.
6. Add annotations if required.
7. Enter technician notes.
8. Verify session metadata.
9. Click **Capture**.
10. Click **Export PDF** if a report is required.

Evidence is automatically saved to the configured capture folder.

---

# Camera Operations

## Selecting a Camera

1. Open the **Source** dropdown.
2. Select the desired camera.
3. Click **Start Preview**.

If multiple cameras are connected, verify that the correct source is selected before capturing evidence.

---

## Selecting a Capture Format

The Capture Format dropdown allows selection of:

* Resolution
* Frame Rate
* Camera Format

Examples:

* 1920×1080 @ 30 FPS
* 1920×1080 @ 60 FPS
* 1280×720 @ 30 FPS
* 640×480 @ 30 FPS

Higher resolutions provide greater detail but may increase storage requirements.

---

# Live Preview

## Start Preview

Click **Start Preview** to begin displaying the live camera feed.

The viewport will display the active camera source.

---

## Stop Preview

Click **Stop Preview** to end the live feed.

No recording or capture operations can be performed while preview is stopped.

---

# Freeze Frame Inspection

Freeze Frame allows detailed inspection without stopping the camera connection.

## Freeze an Image

1. Start Preview.
2. Click **Freeze**.
3. Inspect the image.
4. Add notes or annotations if required.

---

## Resume Live View

Click **Resume** to return to the live camera feed.

---

# Annotations

Annotations may be used to identify defects, inspection areas, test points, components, or areas of concern.

Examples:

* Arrows
* Shapes
* Text labels

Recommended workflow:

1. Freeze image.
2. Add annotations.
3. Capture evidence.

Annotations become part of the generated evidence package.

---

# Notes Editor

The Notes Editor allows inspection observations to be stored with captured evidence.

## Font

Changes the selected text font.

Supported fonts may include:

* Segoe UI
* Arial
* Calibri
* Tahoma
* Times New Roman
* Verdana
* Courier New
* Georgia

---

## Size

Changes the selected text size.

Available sizes range from:

* 8 pt
* 9 pt
* 10 pt
* 11 pt
* 12 pt
* 14 pt
* 16 pt
* 18 pt
* 20 pt
* 24 pt
* 28 pt
* 36 pt
* 48 pt
* 72 pt

---

## Color

Applies text color to selected content.

Available colors include:

* Default
* Black
* Gray
* Yellow
* Orange
* Red
* Pink
* Purple
* Blue
* Cyan
* Green
* Brown

---

## Formatting

Available formatting actions:

* Bold
* Italic
* Underline
* Clear Formatting

---

## Lists

Available list styles:

* Bulleted List
* Numbered List

---

## Alignment

Available alignment options:

* Align Left
* Align Center
* Align Right
* Justify

---

# Evidence Capture

Evidence Capture generates documentation packages for inspection records.

## Capture Evidence

1. Verify metadata.
2. Verify notes.
3. Verify annotations.
4. Click **Capture**.

The application generates evidence artifacts automatically.

---

# Evidence Output Types

## Original PNG

Raw image captured directly from the viewport.

Contains:

* Captured image only

Does not contain:

* Notes
* Metadata
* Evidence formatting

---

## Evidence PNG

Formatted evidence image.

Contains:

* Captured image
* Session metadata
* Technician information
* Notes
* Annotations

---

## Evidence PDF

Printable evidence report.

Contains:

* Original captured image
* Session metadata
* Technician information
* Camera information
* Capture format information
* Notes

---

# Recording

Lab Imager supports video recording for inspection activities.

## Start Recording

1. Start Preview.
2. Click **Record**.
3. Perform inspection activities.

The recording timer will indicate active recording status.

---

## Stop Recording

Click **Stop Recording**.

The AVI file will be finalized and saved automatically.

---

## Recording Format

Current recording format:

* AVI

Future versions may support additional formats.

---

# Session Metadata

Session metadata provides traceability and documentation context.

Typical metadata fields include:

* Project
* Board / Device
* Technician
* Date
* Time
* Camera Source
* Capture Format

Verify metadata before capturing evidence.

---

# Evidence Storage Location

By default, captured evidence is stored in:

```text
Pictures\Lab Imager\Captures
```

Files are automatically timestamped.

Example:

```text
lab-imager-capture-20260609-134500.png
```

---

# Troubleshooting

## Camera Does Not Appear

1. Verify camera is connected.
2. Disconnect and reconnect the device.
3. Restart Lab Imager.
4. Verify Windows detects the camera.

---

## No Preview Image

1. Verify a camera is selected.
2. Verify a capture format is selected.
3. Click Start Preview.
4. Restart preview if necessary.

---

## Freeze Button Not Available

Preview must be running before Freeze can be used.

---

## Capture Button Not Producing Files

Verify:

* Save location is accessible
* Disk space is available
* Capture completed successfully

Check the capture folder for generated files.

---

## PDF Export Disabled

PDF export becomes available after evidence has been captured.

Generate evidence first.

---

## Recording File Missing

Verify:

* Recording was started
* Recording was stopped properly
* Save location is accessible

---

# Frequently Asked Questions

## Do I need to freeze before capturing?

No.

However, freezing is recommended for inspection documentation and annotation workflows.

---

## Can I capture while recording?

Yes.

Evidence capture and recording can be used together.

---

## Can I add notes to evidence?

Yes.

Notes entered into the Notes Editor become part of the evidence package.

---

## Can I create PDF reports?

Yes.

Capture evidence and then click **Export PDF**.

---

## Where are my files saved?

By default:

```text
Pictures\Lab Imager\Captures
```

---

# Keyboard Shortcuts

Common shortcuts include:

```text
Ctrl + Z    Undo
Ctrl + Y    Redo
Ctrl + B    Bold
Ctrl + I    Italic
Ctrl + U    Underline
```

Availability depends on the active control.

---

# Additional Documentation

See also:

* ABOUT.md
* KEYBOARD-SHORTCUTS.md
* CHANGELOG.md
* RELEASE-NOTES.md
* SETTINGS-SPEC.md

---

# About Lab Imager

Lab Imager is developed for laboratory imaging, electronics inspection, microscope capture, documentation, and evidence collection workflows.

For version information, licensing information, and release history, see ABOUT.md and RELEASE-NOTES.md.

---

Copyright © 2026 MSE McGann Systems Engineering.
