# Changelog

All notable product-level changes to Lab Imager are documented here.

This changelog summarizes the historical evolution of the application. Detailed engineering notes, troubleshooting records, implementation attempts, rejected approaches, validation records, and repository checkpoints remain in `docs/lab-imager-log.html`.

---

## 0.9.0 — Rich Notes Modernization

### Summary

This release modernized the Notes system from a basic text-entry area into a functional rich-text documentation workspace suitable for evidence capture, reporting, and technician-facing lab records.

The change was required because Lab Imager’s notes are not casual comments. They are part of the evidence package and must preserve technician formatting, structure, emphasis, and readability when exported.

### Added

* Rich text formatting support
* Font family selection
* Font size selection
* Text color selection
* Bold formatting
* Italic formatting
* Underline formatting
* Alignment controls
* Bulleted lists
* Numbered lists
* Clear formatting support
* Dropdown-based toolbar architecture
* Formatting dropdown
* Lists dropdown
* Alignment dropdown
* Color dropdown
* Rich notes evidence rendering
* FlowDocument cloning for export
* White evidence-output rendering surface
* Dynamic note-height calculation for long notes
* Checkmark state tracking inside formatting controls

### Changed

* Replaced the original Notes toolbar with a structured two-row editor toolbar.
* Converted individual formatting buttons into grouped dropdown menus.
* Improved the Notes editor layout so it behaves more like a professional document editor.
* Updated evidence rendering so formatted notes are preserved instead of flattened into plain text.
* Corrected WPF font-size handling so user-selected point sizes render correctly.
* Rolled dropdown rendering back to native Windows styling after custom dark dropdown styling caused readability regressions.

### Fixed

* Fixed selection loss during formatting operations.
* Fixed formatting handlers that failed when the toolbar stole focus.
* Fixed font-size mismatch caused by WPF using device-independent pixels instead of typographic points.
* Fixed long-note clipping during evidence generation.
* Fixed plain-text-only export limitations.
* Fixed dark editor styling contaminating evidence output.
* Fixed dropdown readability failures including white-on-white, dark-on-dark, and broken disabled-state rendering.
* Fixed Source Selector and Capture Format Selector rendering after toolbar changes.

### Why This Changed

The Notes system had to become evidence-grade. Basic note text was not enough once Lab Imager started generating evidence artifacts. Technicians need to document findings with emphasis, lists, color, and structured formatting, and those formatting choices must survive export.

This release moved the Notes system from “text box” behavior to “document editor” behavior while preserving evidence export compatibility.

### Validation

Validated:

* Bold formatting
* Italic formatting
* Underline formatting
* Font family changes
* Font size changes
* Text color changes
* Alignment
* Bulleted lists
* Numbered lists
* Clear formatting
* Mixed formatting
* Long notes
* Evidence rendering
* Source selector recovery
* Capture format selector recovery
* Build stability

### Known Follow-Up

* Annotation preservation validation remains pending.
* Evidence PDF packaging remains pending.
* Final release-level UI polish remains pending.

---

## 0.8.0 — Integrated Recording Pipeline

### Summary

This release added real AVI recording capability to Lab Imager and moved recording from a placeholder control into a working DirectShow recording path.

The original approach attempted to attach a recording branch to an already-running preview graph. That proved unstable. The final working approach rebuilt the DirectShow graph with the recording branch prepared before execution.

### Added

* Integrated AVI recording pipeline
* Record button integration
* Recording state handling
* Recording timer
* Recording status display
* Recording file output
* DirectShow recording branch construction
* Recording output folder support
* Recording validation workflow
* Standalone recording probe
* Recording diagnostics during development

### Changed

* Reworked recording architecture away from live graph mutation.
* Updated DirectShow preview/recording workflow to prepare the recording branch before graph execution.
* Updated roadmap to mark integrated recording architecture as implemented and validated.
* Updated engineering log with recording pipeline findings and validation.

### Fixed

* Fixed recording failures caused by attempting to mutate a running DirectShow graph.
* Fixed preview interruption during recording start.
* Fixed DirectShow connection failures caused by unreliable Smart Tee insertion timing.
* Fixed output failure where recording appeared active but no file was created.
* Fixed recording state/UI mismatch after failed recording attempts.

### Why This Changed

Recording could not remain a cosmetic placeholder. Lab Imager is intended to support microscope and bench-session documentation, and some workflows require motion capture, not just still evidence.

Testing showed that DirectShow graph mutation during runtime was unreliable and could wedge some camera devices until they were physically unplugged. The architecture had to be corrected so recording was part of the graph before the graph started.

### Validation

Validated:

* AVI file output
* Non-zero recording file size
* Record button triggers real output
* Recording timer runs
* Preview remains active while recording
* Selected capture format works with preview/recording path
* Build passes

### Known Follow-Up

* Recording repeatability testing still required.
* Long-duration recording testing still required.
* File size optimization is deferred.
* MP4 export is not implemented.
* Playback and rewind are future work.

---

## 0.7.0 — Capture Format Architecture

### Summary

This release introduced camera capture format enumeration and selection. Lab Imager moved beyond simply detecting devices and began exposing available camera resolutions, frame rates, and pixel formats.

This was necessary because camera quality problems could not be solved while DirectShow was allowed to negotiate whatever default format it wanted.

### Added

* Capture format enumeration
* Capture format selector
* Device-specific format population
* FOURCC/subtype display support
* Capture format display formatting
* Capture format dropdown UI
* Format enumeration in RecordingProbe
* Subtype translation for formats such as MJPG, YUY2, NV12, H264, H265, and HEVC

### Changed

* Camera source workflow now includes format discovery.
* Capture format selector updates when the selected source changes.
* Preview graph was updated to support selected format application.

### Fixed

* Fixed raw GUID display for known video subtypes.
* Fixed incomplete format visibility in the UI.
* Fixed format selector population after camera reselection.
* Fixed capture quality investigation path by exposing actual supported camera modes.

### Why This Changed

The project hit a practical quality problem: a supposedly high-resolution camera could still look poor if DirectShow selected a low-quality or compressed default mode.

Capture format visibility and application were required before any serious recording, evidence capture, or image-quality validation could happen.

### Validation

Validated:

* Multiple capture formats display in the UI
* Format list updates per camera
* Common subtypes display with readable labels
* Selected format path was investigated and later wired into preview graph handling

### Known Follow-Up

* Additional device testing required across multiple cameras and capture cards.
* Format persistence may need hardening before release.

---

## 0.6.0 — UI Modernization

### Summary

This release transformed Lab Imager from a basic WPF shell into a workstation-style imaging interface.

The original interface was too generic and did not match the approved design direction. The application needed to feel like a technical bench instrument, not a default desktop form.

### Added

* Modern dark theme
* Custom window chrome
* Dark title bar
* Viewport-focused layout
* Right-side Lab Session panel
* Transport control area
* Bottom status bar
* Camera/source selector area
* Capture button area
* Engineering-style viewport visuals
* Approved UI screenshot assets
* Responsive layout experiments
* Scrollable panel experiment branch

### Changed

* Replaced default Windows white chrome with custom dark UI styling.
* Reworked the layout into a viewport-left/session-panel-right structure.
* Moved preview transport controls toward the viewport workflow.
* Reduced oversized UI proportions.
* Shifted toward compact workstation controls.
* Preserved screenshots of approved UI states in the project log.

### Fixed

* Fixed malformed viewport styling.
* Fixed oversized dotted grid visuals.
* Fixed misplaced reticles/corner markers.
* Fixed overly thick top/bottom UI bars.
* Fixed right-panel crowding in multiple layout passes.
* Fixed UI glyph corruption and question-mark artifacts.
* Fixed fragile Unicode symbols by replacing unsafe display markers where needed.
* Fixed visual drift through stable rollback checkpoints.

### Why This Changed

The UI matters because Lab Imager is a visual inspection tool. If the interface is cluttered, oversized, unreadable, or visually inconsistent, it directly interferes with the technician’s work.

The design direction settled on a dark engineering workstation style with controlled blue accents, strong readability, minimal visual noise, and clear separation between the live image and session metadata.

### Validation

Validated:

* Application launches
* Custom window controls work
* Source selector remains usable
* Notes panel remains usable
* Transport controls visible
* Capture controls visible
* Approved UI screenshots archived
* Build passes after UI revisions

### Known Follow-Up

* Final responsive scaling review still required.
* Small-screen layout still needs validation.
* Right-panel overflow remains a monitoring item.
* Further cosmetic changes should be deferred unless tied to functionality.

---

## 0.5.0 — Evidence Workflow

### Summary

This release introduced evidence-oriented capture behavior. Lab Imager moved from ordinary screenshot capture toward structured evidence generation using technician-entered metadata and session notes.

### Added

* Evidence PNG generation
* Original PNG preservation
* Session metadata model
* Metadata capture from technician-entered fields
* Project name field integration
* Board/device field integration
* Technician field integration
* Source device metadata capture
* Notes text capture
* Evidence report generation foundation
* HTML evidence report generation

### Changed

* Capture workflow now collects session metadata before saving evidence.
* Capture output began separating original capture data from evidence output.
* Evidence artifacts were directed toward structured output folders.
* Notes and metadata became part of the capture workflow.

### Fixed

* Fixed capture output folder routing.
* Fixed metadata collection from UI fields.
* Fixed evidence PNG generation so technician-entered data is represented in output.
* Fixed missing report workflow scaffolding.

### Why This Changed

A lab evidence tool cannot simply dump screenshots. The captured image needs context: who captured it, what board/device it belongs to, what project it supports, which camera/source was used, and what the technician observed.

This release established the evidence model needed for lab documentation, repair validation, and future PDF packaging.

### Validation

Validated:

* Capture while preview is running
* Capture after freeze
* Capture after resume
* Capture after camera reselection
* Metadata collection
* Notes extraction
* Evidence PNG output
* HTML evidence report generation

### Known Follow-Up

* Unified save naming scheme still required.
* Evidence PDF package generation still required.
* Annotation preservation validation still required.
* Original/evidence/report output rules require final release hardening.

---

## 0.4.0 — Capture System

### Summary

This release added the first working still-capture pipeline. Lab Imager could begin exporting image artifacts instead of only displaying a preview UI.

### Added

* Screenshot capture service
* Viewport capture service
* PNG export pipeline
* Timestamped filename generation
* Capture button wiring
* Output folder creation
* Status bar capture feedback

### Changed

* The application moved from preview-only behavior to artifact-producing behavior.
* Capture output was routed to the user’s Pictures/Lab Imager capture area.
* Capture was abstracted behind a service instead of being embedded directly in UI logic.

### Fixed

* Fixed missing capture output behavior.
* Fixed capture button wiring.
* Fixed basic viewport capture flow.
* Fixed status feedback after capture.

### Why This Changed

Still capture is one of the core reasons Lab Imager exists. The application needed a reliable way to save microscope images before moving into advanced evidence packaging or annotation preservation.

### Validation

Validated:

* PNG files created
* Timestamped captures generated
* Capture button works
* Output path reported in status/status bar
* Build passes

### Known Follow-Up

* Capture needed later integration with metadata, notes, annotations, and PDF export.

---

## 0.3.0 — Frame Pipeline Stabilization

### Summary

This release explored and stabilized the lower-level frame pipeline needed to move beyond native DirectShow window rendering.

The work established frame delivery concepts, SampleGrabber/BufferCB behavior, and WPF dispatch handoff patterns.

### Added

* WPF frame preview service scaffold
* DirectShow frame graph prototype
* SampleGrabber integration
* BufferCB callback path
* Managed buffer copy
* FrameReady event pattern
* Dispatcher-safe WPF handoff
* FPS/frame metrics logging

### Changed

* Added separation between native DirectShow preview and WPF frame-preview experimentation.
* Introduced frame-level architecture for future overlays, processing, and annotation-safe rendering.

### Fixed

* Fixed early frame pipeline instability.
* Fixed callback wiring issues.
* Fixed dispatcher boundary concerns during WPF frame updates.

### Why This Changed

Native DirectShow rendering works for preview, but it creates overlay limitations because the video surface can render above WPF elements. A frame pipeline is required for future annotation overlays, inspection grids, image processing, and exact viewport export behavior.

### Validation

Validated:

* BufferCB callback activation
* Managed frame copy
* Frame event dispatching
* Build stability

### Known Follow-Up

* True WPF frame rendering requires additional pixel format and stride handling.
* Overlay-compatible rendering remains a future architectural target.

---

## 0.2.0 — DirectShow Preview Infrastructure

### Summary

This release established the core camera and preview foundation. Lab Imager became capable of detecting video devices and rendering live preview through DirectShow.

### Added

* DirectShow camera enumeration
* Camera/source selector
* Device selection workflow
* Default source selection
* Persistent JSON settings service
* Source restoration at launch
* Live preview service
* Start Preview control
* Stop Preview control
* Freeze Preview control
* Resume Preview control
* Preview state handling
* Source-change interruption handling

### Changed

* Replaced stub device services with DirectShow-backed enumeration.
* Replaced placeholder preview state with working preview lifecycle handling.
* Added persistent settings storage under `%APPDATA%`.
* Camera terminology was shifted toward “Source” where appropriate.

### Fixed

* Fixed source selector not populating with real devices.
* Fixed default source not refreshing immediately after selection.
* Fixed preview state mismatch after source changes.
* Fixed camera availability confusion caused by hardware/camera state rather than application logic.

### Why This Changed

Lab Imager could not become an imaging workstation until it could reliably detect and preview real DirectShow-compatible video sources.

This milestone moved the app from UI scaffold to actual camera-connected software.

### Validation

Validated:

* Source enumeration
* Multiple source availability
* Source selection
* Default source persistence
* App restart source restoration
* Start Preview
* Stop Preview
* Freeze
* Resume
* Source status updates

### Known Follow-Up

* Capture format selection and quality control were not complete at this stage.
* Recording was not implemented at this stage.

---

## 0.1.0 — Initial Project Foundation

### Summary

This release established the Lab Imager project structure, repository baseline, visual direction, and documentation system.

The project began as a Windows-native microscope/camera workstation for soldering, electronics inspection, screenshot capture, annotations, timestamps, metadata, and lab documentation.

### Added

* Initial WPF application scaffold
* .NET 8 desktop project structure
* Repository initialization
* GitHub repository setup
* `.gitignore` cleanup
* Core folder structure
* Initial documentation framework
* Engineering log file
* Roadmap file
* Cover page
* Approved UI design direction
* Dark engineering workstation visual target
* UI screenshot asset archive
* Custom dark theme foundation
* Custom title bar foundation

### Changed

* Shifted from generic camera software concept to a purpose-built lab imaging workstation.
* Established dark navy/electric-blue visual language.
* Adopted HTML engineering log format for long-term development tracking.

### Fixed

* Removed tracked build artifacts.
* Repaired early log corruption.
* Fixed native white title bar issue through custom chrome direction.
* Corrected early malformed XAML attempts.

### Why This Changed

The project needed a stable foundation before camera work could begin. The first priority was not advanced imaging; it was creating a maintainable WPF application shell, repository structure, documentation habit, and approved design language.

### Validation

Validated:

* Repository initialized
* Build passes
* WPF shell launches
* Custom title bar works
* Minimize/maximize/close work
* Dark theme loads
* Engineering log structure established
* Approved design assets archived

### Known Follow-Up

* Real camera enumeration, live preview, evidence capture, recording, annotations, and packaging were all future milestones at this stage.
