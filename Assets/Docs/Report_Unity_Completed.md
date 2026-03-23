# PROJECT REPORT: Tetra - The Shattered Realm
## Final Project - PRU213 (Unity Game Development)
**Prepared by:** Group 6 (LocLM, HungTQ, PhatNT, ThuanVH)
**Date:** March 2026
**Location:** FPT University, Cantho Campus

---

# 1 Title Page
## 1.1 Game Name
**Tetra: The Shattered Realm**
## 1.2 Tag line
*The 5th Element awakens as the last binding vessel to restore the shattered Core.*
## 1.3 Team
**Group 6**
- **Lead Developer/Systems:** LocLM
- **Level Designer/Mechanics:** HungTQ
- **Asset/Animation Manager:** PhatNT
- **UI/UX Designer:** ThuanVH
## 1.4 Date of last update
March 23, 2026

# 2 Game Overview
## 2.1 Game Concept
Tetra: The Shattered Realm is a high-fidelity 2D Side-scrolling Action-Platformer. Players inhabit the "Vessel," a hollow suit of armor designed to withstand the volatile energies of the four elements. The core gameplay loop involves exploring meticulously crafted elemental realms, engaging in tactical combat against corrupted guardians, and managing character growth through a deep RPG stat-allocation system.
## 2.2 Target Audience
- Fans of **Metroidvania** and **Soulslike** platformers (e.g., Hollow Knight, Dead Cells).
- Players who enjoy dark fantasy narratives and challenging boss encounters.
- Speedrunners and completionists who value tight physics and precise movement.
## 2.3 Genre(s)
Action-Adventure, Side-scroller, Platformer, Light-RPG.
## 2.4 Game Flow Summary
The journey begins at the **Login Screen**, where players authenticate with a cloud database. Upon entry, the player lands in the **Hub (Aethelgard)**. From here, players venture into four specialized realms: **Earth, Water, Fire, and Air**. Each realm consists of 6 Scenes:
1. **Intro:** Story context and theme establishment.
2. **Platforming:** Environmental hazard navigation.
3. **Combat:** Standard enemy encounters.
4. **Gauntlet/Elite:** High-density monster waves.
5. **Guard:** Elite mini-boss encounter.
6. **Boss:** Final Guardian battle for the Elemental Shard.
## 2.5 Look and Feel
- **Visual Style:** Dark Fantasy Pixel Art with atmospheric lighting. Each realm has a distinct palette (e.g., earthy browns/mossy greens for Earth, deep blues/ice for Water).
- **Atmosphere:** A sense of desolation and mystery. Ruined monuments and corrupted nature.
## 2.6 Major Features
- **FE-1: Cloud-Integrated Authentication:** Secure Login/Register system using **MongoDB Atlas**. Stores player identity and encrypted credentials.
- **FE-2: Persistent Progress Service:** Real-time cloud saving of Character Level, EXP, Gold, Map Unlocks, and Stats (`STR`, `INT`, `DEF`, `AGI`).
- **FE-3: Advanced Physics Controller:** Features "Coyote Time" (0.15s buffer), precise Gravity Scaling (3.5f), and I-Frame dashing for responsive movement.
- **FE-4: Dynamic RPG Stat System:** Deep character customization. Stats directly influence combat math (e.g., `AGI` reduces attack cooldowns and increases crit rates).
- **FE-5: Modular Shop & Economy:** NPC-driven shop with stock limits and dynamic pricing. Items include consumable potions and permanent buffs.
- **FE-6: Finite State Machine (FSM) AI:** Enemies exhibit complex behaviors including patrolling, chasing within a specific radius, attacking, and "returning home" when the player leaves their territory.
- **FE-7: Multi-Phase Boss Design:** Bosses change attack patterns, visuals, and difficulty at specific HP thresholds (50%, 40%, or 30%).

# 3 Gameplay
## 3.1 Objectives
- **Primary:** Collect all 4 Elemental Shards to repair the Tetra Core and defeat Malakor (The Void Emperor).
- **Secondary:** Maximize character level (Level 10 cap), upgrade all stats, and discover the hidden lore of the Vessels.
## 3.2 Use Cases
### 3.2.1 Diagram(s)
*(Technical UML Diagrams provided in Appendix/Internal Docs)*
### 3.2.2 Descriptions
- **Cloud Login:** Player enters credentials -> `AuthManager` queries MongoDB -> `GameSession` initializes with persistent data -> Player spawns in Hub.
- **Quest Progression:** Defeat Earth Guardian -> Obtain Earth Shard -> Portal to Water Realm activates in Hub.
- **Combat Loop:** Player uses Melee (L-Click) for STR-based damage or Skills (R-Click/Hotkeys) for INT-based magic.
## 3.3 Game Progression
- **Play Flow:** Safe Haven (Hub) -> Mission Prep (Shop/Dialogue) -> Dimensional Travel (Realms) -> Boss Victory (Shard Retrieval) -> Meta-Progress (Level Up).
- **Mission Structure:** 6 Scenes per Realm with escalating difficulty.
- **Puzzle Structure:** Lever-based gates, moving platforms over spikes, and timing-based hazard avoidance (Lava fountains, Water geysers).

# 4 Mechanics (Key Section)
## 4.1 Rules
- **Death:** HP reaching zero triggers a "Void Rebirth." Player returns to the Hub or last Checkpoint.
- **Skills/Mana:** Skills require MP. MP regenerates automatically at a base rate of 1/sec, enhanced by the `INT` stat (+0.2 MP/sec per point).
## 4.2 Model of the game universe
A 2D world with simulated depth via parallax scrolling. Interaction is strictly collision-based (Rigidbody2D/BoxCollider2D). 
## 4.3 Physics
- **Move Speed:** 6.0 units.
- **Jump Force:** 14.0 units (optimal for 4-unit high blocks).
- **Gravity:** 3.5x for snappy platforming feel.
- **Coyote Time:** 0.15s allowing jumps after leaving a ledge.
## 4.4 Economy
- **Earning:** Normal enemies = 1-3 Gold. Tankers = 5-8 Gold. Bosses = 100-200 Gold.
- **Spending:** Potions (50G HP / 30G MP). Stat Resets (High cost).
- **Balance logic:** 1 HP Potion requires ~20 normal kills (2 Scenes).
## 4.5 Character movement in the game
Full 360-degree control via WASD/Arrows. Double-jump is unlocked via progression. Dash provides temporary invincibility.
## 4.6 Objects
Interactable Chests, breakable vases for Gold, and "Pick-up" items (Health/Mana drops).
## 4.7 Actions
- **Interact (E):** Talk to NPCs, open Chests, enter Portals.
- **Quick-Use (1-3):** Instant consumption of potions from the hotbar.
## 4.8 Combat Math
- **1 STR:** +2 Melee Damage.
- **1 AGI:** +0.5% Crit Rate, +1% Atk Speed.
- **1 DEF:** +10 Max HP.
- **1 INT:** +5 Max MP, +0.2 MP/sec Regen.
- **Crit Damage:** 1.5x Multiplier.
- **I-Frames:** 1.0s duration after being hit.
## 4.9 Screen Flow
Splashes -> Login -> Main Menu -> Hub -> Realm Scenes -> Game Over / Victory Credits.
## 4.10 Game Options
Volume sliders (BGM/SFX), Resolution scaling, Keyboard remapping.
## 4.11 Replaying and saving
Dữ liệu được lưu trực tiếp vào **MongoDB Atlas** thông qua API Driver cho Unity.
- **UserDocument:** Lưu `_id`, `Username`, `Password` (hash), `Email`.
- **ProgressDocument:** Lưu `UserId`, `Level`, `EXP`, `Gold`, `MapProgress`, `Stats` (STR/AGI/DEF/INT).
## 4.12 Cheats and Easter Eggs
Konami Code unlocks the "Golden Armor" skin (Easter Egg).

# 5 Story and Narrative
## 5.1 Back story
Aethelgard used to be a paradise governed by the Tetra Core. Malakor, the Void Emperor, shattered the core to absorb its raw energy. The guardians of the realm became corrupted shadows of their former selves.
## 5.2 Plot elements
The player is the "5th Element" or "Vessel." Unlike living beings, your hollow nature prevents corruption. You are the only thing that can touch the Shards without perishing.
## 5.3 Game story progression
Four chapters (Earth, Water, Fire, Air) leading to the final descent into the Void Keep.
## 5.4 Cut scenes
Static splash art with scrolling text for intro/outro. In-game dialogues via the Shopkeeper and ancient Pedestals.

# 6 Game World
## 6.1 General look and feel of world
Environment changes from heavy, grounded rocks (Earth) to fluid, slippery temples (Water), intense volcanic peaks (Fire), and ethereal floating islands (Air).
## 6.2 Areas
- **Map 2 (Earth):** ~45 enemies. Hazards: Falling rocks. Boss: **Rannoch**.
- **Map 3 (Water):** ~55 enemies. Hazards: Slippery floors, Geysers. Boss: **Scylla**.
- **Map 4 (Fire):** ~65 enemies. Hazards: Lava pillars, self-exploding Imps. Boss: **Monstrosity**.
- **Map 5 (Air):** ~60 enemies. Hazards: High winds, abyss. Boss: **Sanctyrix**.

# 7 Characters.
## 7.1 For each character
### 7.1.1 Back story
**The Knight:** Forged by the ancient sages as an emergency vessel for the Tetra Core.
### 7.1.2 Personality
Stoic, determined, and silent.
### 7.1.3 Appearance
Glowing blue core inside silver-etched armor.
### 7.1.4 Abilities
- **Dash:** MP-costing evasion.
- **Heavy Strike:** Earth-based AoE.
- **Flame Burst:** Fire-based projectile.
## 7.2 Artificial Intelligence
AI follows an **FSM (Finite State Machine)**:
- **Patrol:** Moves between waypoints.
- **Chase:** Triggers when player enters `DetectSensor2D.radius`.
- **Attack:** Triggers when player enters `AttackRange2D.radius`.
- **Returning:** Triggers if player moves beyond `maxChaseDistance` from the enemy's home position.
## 7.3 Non-combat and Friendly Characters
**Shopkeeper:** An ancient spirit of trade. Offers cryptic advice about Malakor.

# 8 Levels
## 8.1 Training Level
The **Hub** contains a Training Dummy that mimics boss I-frames, allowing players to practice timing.
## 8.2 Map 2 - Earth Realm Breakdown
- **Synopsis:** The Knight descends into the Aethelgard Mines.
- **Intro:** Dialogue with the Mine Overseer spirit.
- **Details:** 50% Skeletons, 30% Archers, 10% Golems, 10% Bats.
- **Map Structure:** Linear 6-scene progression.

# 9 Interface
## 9.1 Visual System
### 9.1.1 HUD
The HUD uses dynamic `Image` filling for HP (Red) and MP (Blue) bars. It features a persistent Gold icon with `TextMeshPro` text that updates via C# Events (`OnGoldChanged`) to ensure zero-latency feedback. A mini-map indicator tracks the player's progress through the current Scene's 6 segments.
### 9.1.2 Menus
- **Auth Panel:** Responsive design with `TMP_InputField` for credentials and visual feedback text for server errors/success.
- **Inventory:** A grid-based system (`GridLayoutGroup`) allowing for item inspection with tooltips.
- **Shop UI:** Items listed in a `ScrollView`. Each row displays Icon, Description, Price, and remaining Stock.
### 9.1.3 Camera model
A 2D Orthographic camera powered by `Cinemachine`. It utilizes a `CinemachineConfiner2D` to prevent the camera from revealing the edges of the Map/Tilemap, combined with a `Pixel Perfect Camera` component to maintain art consistency across all resolutions.

## 9.2 Control System
- **Movement:** `WASD` or `Arrow Keys`. Logic handled in `PlayerMovement.cs` using `Rigidbody2D.velocity` for consistent platforming.
- **Jump:** `Space`. Implements a jump-buffer system and variable jump height (higher jump if held longer).
- **Combat:** `Mouse0` (Left Click) for sword combo; `Mouse1` (Right Click) or `K` for the current active elemental skill.
- **Interaction:** `E` key to talk to NPCs, activate levers, or enter portals.
- **Inventory/Hotbar:** `Tab` to toggle menu; `1`, `2`, `3` keys for instant potion consumption.

# 10 Commands
## 10.1 Audio, music, sound effects
- **BGM System:** Uses an `AudioMixer` to handle smooth snapshots between exploration (low-pass) and combat (full-spectral).
- **SFX:** "Game Feel" is enhanced by `AudioSource` pooling. Includes unique sounds for: Metal-on-Metal (hit), Ground-landing, Mana-charge-up, and Boss-roar.
## 10.2 Game Art – intended style
- **Art Style:** Pixel Adventure 16-bit style.
- **Animations:** Sprite-sheet based animations with a `Mecanim` state machine. Blending used for transitions (e.g., Run -> Slide Stop).
## 10.3 Help System
- **In-game:** Provided via the **Shopkeeper** and **Pedestals** which explain new mechanics as they are unlocked (e.g., explaining the Earth Shard's skill).
- **UI:** Conditional tooltips and "Interact" prompts (`TextMeshPro`) that appear overhead when the player is within a `Trigger2D` proximity of an object.
