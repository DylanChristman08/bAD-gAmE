# Copilot Instructions for bAD gAmE

## Project Overview
**bAD gAmE** is a 2D platformer game built in **Unity 6.0.3.4** using the Universal Render Pipeline (URP). The game features a stick man character with movement, combat, and camera mechanics. The codebase uses C# with a custom `memstow` namespace.

## Architecture & Key Components

### Character System (`Assets/Charcters/2D Stick Man/`)
- **PlayerMove2D.cs** - Handles 2D character locomotion (walk/run), jumping, and attack state management
  - Uses **Rigidbody2D** physics with velocity-based movement
  - Integrates with **Animator** for state-driven animations
  - Core mechanics: `permitRunWalk` and `permitJump` flags prevent movement during special actions
  - Animation triggers: `doWalk`, `doRun`, `doJumpUp`, `doJumpDown`, `doAttack`, `doSpecial`, `doFlex`
  
- **CameraMove.cs** - Implements dynamic camera follow with look-ahead
  - Uses **SmoothDamp** for smooth camera movement
  - Offsets adjust based on player velocity (look-ahead) with clamped limits
  - Bounded by `moveLeftLimit` and `moveRightLimit`

### Animation Controller
- Uses **CHG2D-001.controller** (Animator state machine)
- Animations: Idle, Walk, TwinkleToes (run), JumpUp, JumpDown, ShowOff, Flex
- Clips sync with animator parameters (bool-based triggering)

### Project Structure
```
Assets/
├── SCRIPTS/              (Currently empty - for shared/global systems)
├── Charcters/            (Character-specific logic and assets)
│   └── 2D Stick Man/
│       ├── Scripts/      (PlayerMove2D, CameraMove)
│       ├── Animations/   (CHG2D-001.controller, .anim files)
│       ├── Prefab/
│       ├── Sprites/
│       └── Materials/
└── Scenes/               (Game scenes)
```

## Development Patterns & Conventions

### Namespace
All gameplay scripts must use `namespace memstow { }` wrapping.

### Input Handling
- Uses **new InputSystem** (v1.17.0)
- Axis input: `Input.GetAxisRaw("Horizontal")` returns -1/0/1
- Button input: "Jump" (spacebar), "Fire1" (left mouse)
- Shift modifier for running: `Input.GetKey(KeyCode.LeftShift)`

### Physics & Colliders
- **Physics2D** for 2D platformer collision
- **OverlapCircle** checks for grounding with optional layer-mask filtering
- Default radius: 0.2f for ground detection
- Layer system: Scripts reference `groundLayer` LayerMask for selective collision

### Animator Integration
**Critical Pattern**: Animation state is queried via `animator.GetCurrentAnimatorClipInfo(0)` to extract clip name and prevent unintended overlapping actions:
```csharp
animator = gameObject.GetComponent<Animator>();
currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
clipNameL0 = currentClipInfo[0].clip.name;
permitRunWalk = !(clipNameL0 == "Flex");
```

## Important Workflows & Commands

### Building & Running
- Open in **Unity Editor 6.0.3.4**
- Build for target platform via File > Build Settings
- Test scenes are in `Assets/Scenes/`

### Adding New Character Abilities
1. Add new .anim clip in `Charcters/2D Stick Man/Animations/`
2. Add state to CHG2D-001.controller
3. Add bool animator parameter
4. Update PlayerMove2D.cs to manage new state (check `permitRunWalk`/`permitJump`)
5. Add clip name check if blocking other actions

### Extending Movement
PlayerMove2D modifies velocity in two phases:
- **Update()** - Input processing and animator state changes
- **FixedUpdate()** - Rigidbody velocity application (respects `permitRunWalk`)

## Critical Constraints & Gotchas

1. **Movement Blocking** - Actions like "Flex" block walking; check animator state before allowing movement
2. **Velocity Clamping** - Camera offsets are clamped to max values; modify `xOffsetMax`/`yOffsetMax` if extending
3. **Frame Timing** - Jump velocity halving happens on button release; falling detection uses 0.1s delay
4. **Ground Detection Fallback** - If `groundLayer == 0`, uses any collider; set specific layer for proper platforming
5. **Animator Get Called Every Frame** - Currently refetches animator each Update(); could be cached for performance

## Dependencies
- **com.unity.inputsystem** (1.17.0) - Input management
- **com.unity.render-pipelines.universal** (17.3.0) - Graphics pipeline
- **com.unity.2d.*** packages - 2D sprites, animation, tilemap support

## Testing & Debugging
- Use Unity's Animator window to verify state transitions match bool parameters
- Velocity visualization in Scene view helps debug movement logic
- Camera follow limits can be adjusted in Inspector per level
