# Mutant Evolution Idle (Unity 2022+ Prototype)

A modular 2D idle game prototype where a strange organism generates **Biomass**, buys **Mutations**, and eventually performs **Evolution** (prestige) to gain permanent **DNA bonuses**.

## Implemented Systems

- Resource system (`currentBiomass`, auto production, spend/add APIs)
- Idle production loop (1-second ticks in `GameManager.Update`)
- Mutation upgrade system using `ScriptableObject` data
- Evolution/prestige reset with permanent DNA bonuses
- Unity UI management and mutation purchase buttons
- Visual feedback:
  - floating numbers for biomass gain
  - organism sprite override when mutation is purchased
- JSON save/load with autosave and save-on-quit

## Script Architecture

### `GameManager`
Bootstraps all systems and controls the core game loop.

Responsibilities:
- Load save data and initialize managers
- Run production tick every second
- Handle evolution reset flow
- Autosave loop + save on quit

### `ResourceManager`
Single source of truth for biomass.

Responsibilities:
- Store current biomass
- Compute effective biomass/sec
- Generate biomass from idle ticks
- Spend/add biomass safely
- Reset current run values on evolution

### `MutationData` (ScriptableObject)
Data-driven mutation definitions.

Fields:
- ID, name, description
- base cost
- additive production bonus
- multiplicative production bonus
- cost reduction
- optional organism sprite override

### `MutationManager`
Handles unlock/purchase state and applies mutation bonuses.

Responsibilities:
- Track purchased mutations
- Compute mutation cost (including DNA + mutation reductions)
- Purchase mutation and apply effects to `ResourceManager`
- Notify UI when mutation state changes

### `EvolutionManager`
Prestige logic + permanent bonus calculations.

Responsibilities:
- Validate evolution requirement
- Award DNA points
- Expose permanent production / idle speed / cost reduction bonuses

### `SaveSystem`
JSON persistence layer.

Saved data:
- current biomass
- purchased mutation IDs
- DNA points

### `UIManager`
Binds managers to Canvas UI.

Responsibilities:
- Update biomass, production, DNA labels
- Build mutation button list from ScriptableObjects
- Update evolution button state
- Handle floating text animation
- Apply organism sprite changes from mutation purchases

### `MutationButton`
Reusable button component for one mutation entry.

Responsibilities:
- Display name/cost/effect
- Trigger purchase
- Refresh interactable state

---

## Unity Scene Setup (Step-by-step)

1. **Create scene** (e.g., `Main.unity`).
2. Create an empty GameObject called **`GameRoot`** and add:
   - `GameManager`
   - `ResourceManager`
   - `MutationManager`
   - `EvolutionManager`
   - `SaveSystem`
   - `UIManager`
3. In `GameManager`, assign serialized references:
   - `ResourceManager`, `MutationManager`, `EvolutionManager`, `SaveSystem`, `UIManager`.
4. Create a **Canvas** (Screen Space - Overlay).
5. UI layout:
   - **TopBar**
     - `BiomassText` (TMP)
     - `BiomassPerSecondText` (TMP)
     - `DNAText` (TMP)
   - **CenterPanel**
     - `OrganismImage` (`UnityEngine.UI.Image`)
     - `FloatingTextRoot` (`RectTransform`)
   - **BottomPanel**
     - `MutationList` (`VerticalLayoutGroup` + `ContentSizeFitter`)
     - `EvolutionButton` (`Button`) + child TMP text
6. Create a prefab for **MutationButton**:
   - Root: `Button` + `MutationButton` script
   - Child TMPs for title/details
   - Assign refs in `MutationButton` component.
7. Create a prefab for **FloatingText**:
   - TMP text object
   - style as desired (e.g., green text)
8. Assign all UI references in `UIManager`:
   - biomass labels, DNA label
   - organism image + default sprite
   - mutation container + mutation button prefab
   - evolution button + label
   - floating text prefab + parent

---

## Example ScriptableObject Setup

Create 4 `MutationData` assets (`Assets/ScriptableObjects/Mutations/...`):

1. **Tentacles**
   - id: `tentacles`
   - baseCost: `25`
   - additiveBiomassPerSecond: `1`
   - multiplicativeProduction: `1`
   - costReduction: `0`
2. **Extra Eye**
   - id: `extra_eye`
   - baseCost: `60`
   - additiveBiomassPerSecond: `3`
   - multiplicativeProduction: `1`
   - costReduction: `0`
3. **Claws**
   - id: `claws`
   - baseCost: `120`
   - additiveBiomassPerSecond: `0`
   - multiplicativeProduction: `2`
   - costReduction: `0`
4. **Shell**
   - id: `shell`
   - baseCost: `200`
   - additiveBiomassPerSecond: `0`
   - multiplicativeProduction: `1`
   - costReduction: `0.1`

Drag these assets into `MutationManager.availableMutations` in your chosen order.

---

## Suggested UI Hierarchy

```text
Canvas
├── TopBar
│   ├── BiomassText (TMP)
│   ├── BiomassPerSecondText (TMP)
│   └── DNAText (TMP)
├── CenterPanel
│   ├── OrganismImage (Image)
│   └── FloatingTextRoot (RectTransform)
└── BottomPanel
    ├── MutationList (RectTransform)
    └── EvolutionButton (Button)
        └── Label (TMP)
```

---

## Future Expansion Ideas

- Multiple mutation tiers/branches (offense, defense, metabolism)
- Rare random mutation events (temporary buffs/debuffs)
- Offline progress simulation (timestamp-based catch-up)
- Animated organism layers per mutation part
- Unlockable biomes with different production modifiers
- DNA skill tree (choose one of multiple prestige bonuses)
- Achievements and long-term objectives
- Cloud save support
