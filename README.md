# VR Hand Interaction Assignment — Scissor Cutting & Syringe Injection

A Unity VR project demonstrating two core interaction mechanics built for Meta Quest using the XR Interaction Toolkit: a scissor-cutting simulation and a syringe injection simulation, both driven by the controller trigger ("Activate") button.

## Overview

This project implements two self-contained interaction mechanics as part of a VR development hiring assignment:

1. **Scissor Cutting** — pick up a pair of scissors, bring them near a sheet of paper, and press the trigger to cut it into two separate pieces.
2. **Syringe Injection** — pick up a syringe, press the trigger to load medicine into it, bring the needle to a foam block or any other body, and press the trigger again to inject — emptying the syringe with a plunger-push animation.

## Environment

- **Unity Version:** 2022.3.50f1
- **XR Framework:** XR Interaction Toolkit + OpenXR
- **Target Device:** Meta Quest (controller-based interaction)
- **Render Pipeline:** Built-in

## Controls

| Action | Input |
|---|---|
| Move | Standard XRI locomotion (thumbstick as per starter asset defaults) |
| Grab an object | Grip button |
| Cut / Load / Inject | Trigger button, while holding the relevant object |

## Interaction 1: Scissor Cutting

- Grab the scissors by the handle.
- Bring the blade tip close to the paper.
- Press the trigger — the blades play a quick snap animation and, if the paper is within range, it splits into two separate halves.
- Pressing the trigger away from the paper still plays the snap animation but does not cut anything.

**Script:** `SimpleScissorCutter.cs` (on the Scissor root) + `SimplePaperCut.cs` (on the Paper object).

**Detection:** a small `OverlapSphere` at the blade tip checks for nearby colliders on the `Cuttable` layer.

## Interaction 2: Syringe Injection

- Grab the syringe by the barrel.
- Press the trigger with the needle in open air — the syringe loads: the internal fluid mesh fills up and the plunger draws back.
- Bring the needle tip to the foam block.
- Press the trigger again — the syringe injects: the fluid mesh empties and the plunger pushes forward, simulating the injection.
- Pressing the trigger while loaded but not touching a valid target does nothing (prevents "injecting" into open air).

**Script:** `SimpleSyringe.cs` (on the Syringe root).

**Detection:** a small `OverlapSphere` at the needle tip checks for nearby colliders on the `Injectable` layer.

## Project Structure

```
Assets/
├── Scripts/
│   ├── SimpleScissorCutter.cs   — blade snap animation + cut detection
│   ├── SimplePaperCut.cs        — paper split/separation logic
│   ├── SimpleSyringe.cs         — load/inject state machine + plunger +  animation
├── Models/                      — scissor & syringe
├── Materials/
├── Prefabs/
└── Scenes/
    └── Main Interaction Scene   — primary scene, contains both mechanics
```

## Key Setup Notes

- Both interactable root objects (`Scissor`, `Syringe`) use a **kinematic** `Rigidbody` with the `XR Grab Interactable`'s Movement Type set to **Kinematic**, so they sit still on the table until grabbed rather than reacting to gravity/collision physics.
- The `Cuttable` layer is assigned to the paper object; the `Injectable` layer is assigned to the foam block — the interacting tool holds a *reference* to the layer it should search for, not the layer itself.
- Both mechanics are driven entirely by the hand controller's **Activate** (trigger) event via `XRGrabInteractable.activated`.