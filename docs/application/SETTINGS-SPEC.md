# Settings Specification

## Purpose

This document defines all configurable user settings available within Lab Imager.

Settings should be persisted between application launches and restored automatically when the application starts.

---

# General

## Launch Maximized

Type:
Boolean

Default:
True

Description:
Launch the application in a maximized state.

---

## Remember Window Size

Type:
Boolean

Default:
True

Description:
Restore the previous application window size.

---

## Remember Window Position

Type:
Boolean

Default:
True

Description:
Restore the previous application position.

---

# Camera

## Default Camera

Type:
String

Default:
Last Used Camera

Description:
Automatically select the configured camera source during startup.

---

## Default Capture Format

Type:
String

Default:
Last Used Format

Description:
Automatically apply the previously selected capture format.

---

## Auto Start Preview

Type:
Boolean

Default:
False

Description:
Automatically start preview after application launch.

---

# Evidence

## Save Original PNG

Type:
Boolean

Default:
True

Description:
Preserve original captured image.

---

## Generate Evidence PNG

Type:
Boolean

Default:
True

Description:
Generate annotated evidence image.

---

## Generate PDF

Type:
Boolean

Default:
True

Description:
Generate PDF evidence package when available.

---

## Evidence Output Folder

Type:
Folder Path

Default:
Application Default

Description:
Location where evidence artifacts are stored.

---

# Notes

## Default Font

Type:
String

Default:
Segoe UI

Description:
Default font used by Notes Editor.

---

## Default Font Size

Type:
Integer

Default:
11

Description:
Default Notes Editor font size.

---

# Recording

## Recording Output Folder

Type:
Folder Path

Default:
Application Default

Description:
Location where AVI recordings are saved.

---

## Recording Format

Type:
String

Default:
AVI

Description:
Recording container format.

---

# Appearance

## Theme

Type:
String

Default:
Dark

Description:
Application visual theme.

---

## UI Scale

Type:
Integer

Default:
100

Description:
User interface scaling percentage.