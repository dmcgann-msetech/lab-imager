# Versioning Policy

## Purpose

This document defines how version numbers are assigned to Lab Imager.

Version history is maintained separately in CHANGELOG.md.

---

## Version Format

MAJOR.MINOR.PATCH

Examples:

1.0.0
1.1.0
1.1.1

Pre-release examples:

1.0.0-alpha
1.0.0-beta
1.0.0-rc1

---

## MAJOR Version Changes

Increment when:

- Significant architectural changes occur
- Breaking compatibility changes occur
- Major subsystem introductions occur
- Production milestone releases occur

Example:

1.0.0 → 2.0.0

---

## MINOR Version Changes

Increment when:

- New user-facing functionality is added
- New workflows are introduced
- Significant capabilities are expanded

Examples:

1.0.0 → 1.1.0

Possible Lab Imager examples:

- Recording subsystem added
- Playback subsystem added
- Annotation subsystem added
- Evidence package workflow added

---

## PATCH Version Changes

Increment when:

- Bugs are fixed
- Validation issues are corrected
- UI defects are repaired
- Documentation corrections are made

Examples:

1.1.0 → 1.1.1

---

## Pre-Release Labels

### Alpha

Internal development builds.

### Beta

Feature-complete builds undergoing validation.

### Release Candidate (RC)

Candidate for production release.

### Production

No suffix.

Example:

1.0.0

---

## Current Development Target

Current Production Goal:

1.0.0

Version assignments should follow validated milestones and completed functionality rather than individual commits, pushes, or documentation updates.

Refer to CHANGELOG.md for historical release progression.