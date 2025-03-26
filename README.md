# SierraMadre Online

**SierraMadre Online** is a standalone multiplayer framework built for *Fallout: New Vegas* and *Fallout 3*, merging the single-player depth of the classic Fallout series with real-time multiplayer gameplay. The mod integrates systems inspired by *Van Buren* and classic Black Isle design concepts to create a new cooperative and competitive experience.

---

## Project Overview

This project is being developed by **YuhIcey** as part of a long-term, college-based development initiative with the vision of becoming the definitive multiplayer extension of the Fallout universe.

SierraMadre Online is built entirely from scratch using:
- C# (.NET 8) for backend and networking
- C++ via NVSE for direct memory control and in-engine hooks
- A custom WebSocket stack (`ShyroNet`) for player synchronization
- Modular plugin support and integrated Discord tooling

---

## Key Features

- **True Multiplayer**: Real-time multiplayer in Fallout: New Vegas without VaultMP or NVMP dependencies.
- **Custom Engine Core**: Built on a custom WebSocket engine (`ShyroNet`) supporting tick-based world sync and interpolation.
- **NVSE Integration**: In-game C++ memory hook system handles multiplayer saves and state extraction.
- **Factions & Capture Points**: Join, promote, and manage faction members with progression tiers and zone control systems.
- **Safe Zones & PvP**: Opt-in PvP and automatic godmode management within predefined safe zones.
- **Admin Control Suite**: CLI, role-based permissions, real-time moderation, ban/kick/mute, and Discord webhook relays.
- **Plugin Architecture**: Dynamically load plugins with NexusMod integration, `plugin.json` metadata, and optional dependency management.

---

## Project Roadmap

| Phase         | Status          | Description                                   |
|---------------|------------------|-----------------------------------------------|
| Pre-Alpha     | In Development  | Core networking, player sync, basic CLI       |
| Alpha         | Planned (2025)  | Interpolation, factions, admin tooling        |
| Private Tests | Q3 2025         | Closed multiplayer testing with feedback loop |
| Public Alpha  | Q1 2026         | Initial public release, GitHub + installer    |
| Beta          | TBD             | Mod support, PvE events, installer updates    |
| Final         | TBD             | Fully polished and expandable release         |

---

## Installation (Not Yet Available)

SierraMadre Online is under active development and **not currently playable**. Setup instructions, installation guides, and public builds will be added here once the Alpha version is released.

```bash
git clone https://github.com/YuhIcey/SierraMadreOnline.git
cd SierraMadreOnline
# Further setup instructions will be available upon release
