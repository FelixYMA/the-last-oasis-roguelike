# The Last Oasis — 2D Roguelike Action Prototype (Unity)

A pixel-art roguelike action game prototype built in **Unity**. Players explore rooms, fight enemies, collect gold, and upgrade stats (HP / ATK) in a fast-paced loop with light narrative interactions.

> Team project (co-authors omitted in this public version).

---

## Demo
- **Gameplay video:** available under **GitHub Releases** (recommended)  
  *(Replace with your Release URL once published.)*

---

## Gameplay Overview
Core loop:
1. Explore rooms and defeat enemies
2. Collect gold from enemies / chests
3. Spend gold to upgrade **health** and **attack**
4. Continue to new rooms and aim for higher score / longer survival

---

## Controls
- **Move:** WASD  
- **Light attack:** Left Mouse Button  
- **Heavy attack:** Right Mouse Button  
- **Interact:** E (e.g., exit / next level prompt)  
- **Dialogue:** Space to advance

---

## Key Features
- Pixel-art cyberpunk visual style and UI
- Gold-based progression (HP/ATK upgrades)
- Basic enemy AI behaviors (patrol / tracking)
- UI systems: health bars, gold counter, upgrade menu, dialogue prompts
- Iterative playtesting and balancing improvements (controls, difficulty, feedback cues)

(Details documented in the final report under `docs/`.)

---

## Tech Stack (Unity)
Implemented using common Unity 2D systems:
- **Tilemap** for 2D grid-based level design
- **Cinemachine** for smooth camera follow
- **Unity UI (Canvas)** for HUD, menus, dialogue boxes
- **Animator** for player/enemy animation states
- **Collider2D / Rigidbody2D** for movement and interactions
- **ScriptableObjects** for upgrade management
- **AudioMixer** for audio control

---

## How to Run

### Option A — Run in Unity (recommended)
1. Install **Unity** (use the version specified in `ProjectSettings/ProjectVersion.txt`)
2. Open Unity Hub → **Add** → select this project folder
3. Open the main scene and press **Play**

### Option B — Download a build (optional)
If a Windows build is provided, download it from **Releases** and run the executable.
*(A build is optional; the Unity project itself is sufficient for evaluation.)*

---

## Repository Structure
```text
.
├── Assets/                 # game assets, scenes, scripts, prefabs
├── Packages/               # Unity package manifest
├── ProjectSettings/        # Unity project settings (includes ProjectVersion)
├── docs/
│   └── the-last-oasis_final-report_public.pdf
└── README.md
```

---

## Notes on Public Version

- This repository is published for portfolio / educational demonstration.
- Co-authors are omitted in the public version.

---

## License
MIT
