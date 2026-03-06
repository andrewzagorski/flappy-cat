# AGENTS.md

## Code Style Guide

This project follows the [Godot C# Style Guide](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_style_guide.html). All contributors and AI agents must adhere to these conventions.

---

## Language

- Target **C# 13.0** (.NET 9.0). Do not use features from C# 14.0 or later.
- Ensure your `.csproj` has `<TargetFramework>net9.0</TargetFramework>`.

---

## Formatting

### General
- Line endings: **LF** (not CRLF or CR).
- End each file with a single newline (except `.csproj` files).
- Encoding: **UTF-8 without BOM**.
- Indentation: **4 spaces** (no tabs).
- Line length: aim to keep lines under **100 characters**.

---

### Naming Conventions

| Identifier | Convention | Example |
|---|---|---|
| Namespaces, types, members (methods, properties, constants, events) | `PascalCase` | `PlayerCharacter`, `DefaultSpeed` |
| Local variables, method arguments | `camelCase` | `attackStrength` |
| Private fields | `_camelCase` (underscore prefix) | `_aimingAt` |
| Interfaces | `IPascalCase` | `IDamageable` |
| Two-letter acronyms | All caps where PascalCase expected | `UI`, `ID` |

---

### Variables

- **Member variables**: only declare if used in more than one method; otherwise use local variables.
- **Local variables**: declare as close as possible to first use.
- **`var`**: use only when the type is obvious from the right-hand side.

---

## Godot-Specific Conventions

- Node scripts must use `partial class` to work with Godot's source generators.
- Prefer `[Export]` properties over public fields for inspector-exposed values.
- Use `GetNode<T>()` with a cached `[Export]`-assigned node reference rather than magic strings where possible.
- Signal callbacks should follow the naming pattern `On<NodeName><SignalName>` (e.g., `OnButtonPressed`).

---

## Scene & Node Architecture

- Every scene should have a **single, focused responsibility**. If a scene is doing two things, split it.
- Design scenes to be **self-contained with no external dependencies by default**. When a scene must interact with its context, use dependency injection (see below).
- **Do not use `GetParent()` or upward tree traversal** to find dependencies. This tightly couples a child to a specific parent structure.
- Prefer **sibling or root-level nodes** for things that logically outlive their context (e.g., a `Player` should not be a child of a `Room` if it persists across rooms).
- When a node needs to trigger behavior in its parent, **emit a signal** — never call a parent's method directly.
- When a parent needs to configure a child, **set properties or inject dependencies before adding the child to the scene tree** where possible. Exceptions exist for things that require tree membership, like global position.
- Implement `_GetConfigurationWarnings()` on any node that requires externally-set dependencies, so misconfiguration is visible in the editor.
- Before creating a custom node, check whether a built-in Godot node already solves the problem.

---

## Dependency Injection

Prefer these patterns (in order) when a node needs an external reference:

1. **`[Export]` property set in the editor** — simplest; use when the reference is fixed at design time.
2. **Public property or method set by the parent before `_Ready()`** — use when the parent instantiates the child at runtime.
3. **Signal** — use when the child only needs to *notify* and shouldn't care who's listening.
4. **`Callable` property** — use when the child needs to invoke a single callback provided by the parent.
5. **Autoload singleton** — last resort, only for truly global state (e.g., game settings, a global event bus). Do not use Autoloads for scene-local coordination.

**Never use:**
- `GetParent()` to access parent data or methods
- `GetNode("/root/...")` string paths to reach singletons when an `[Export]` or Autoload would suffice
- Static mutable state outside of deliberate singletons

---

## Object Types — Use the Right Base Class

Do not default to `Node` for everything. Choose the appropriate base:

| Base class   | When to use |
|---|---|
| `Node`       | Needs to live in the scene tree; has visual or interactive behavior; needs engine lifecycle notifications |
| `RefCounted` | Pure data or logic class; no scene tree needed; automatically garbage collected |
| `Object`     | Rare — only when you need manual memory control and `RefCounted` is unsuitable |
| `Resource`   | Data that should be serialized, shared between scenes, or exposed in the Inspector |

Prefer `RefCounted` for custom helper and data classes. Prefer `Resource` for configuration, stats, or reusable data assets. Replacing data-centric `Node`s with `RefCounted` where possible improves performance and keeps the scene tree clean.

---

## Signals

- Declare signals in PascalCase as C# events using `[Signal]`: `public delegate void PlayerDiedEventHandler();`
- Signal names should be **past-tense verbs** describing what happened: `Died`, `ItemCollected`, `RoomEntered`.
- Connect signals in code using the type-safe `SignalName` constant and `Connect()`; avoid raw magic-string `.Connect("signal_name", ...)`.
- Disconnect signals when the receiving object may be freed before the emitter.
- Do **not** use signals for synchronous data queries. Signals are fire-and-forget; use method calls or `Callable`s when you need a return value or guaranteed execution order.

---

## Process & Input

- Use `_PhysicsProcess(double delta)` for anything involving movement, collision, or physics-driven state.
- Use `_Process(double delta)` only for visual updates and things that should track render framerate.
- **Do not poll input in `_Process`**. Handle discrete input (button presses, actions) in `_Input(InputEvent @event)` or `_UnhandledInput(InputEvent @event)`.
- Disable processing on nodes that don't need it: `SetProcess(false)` / `SetPhysicsProcess(false)`. This is especially important as node count grows.

---

## Autoloads

- Only use Autoloads for objects that must be **truly global and always present** (e.g., a persistent settings manager or global event bus).
- Prefer **static methods or static variables** on a regular class for shared utilities that don't need lifecycle callbacks.
- Autoloads are not freed on scene changes — be deliberate about what state they hold.
- Never use an Autoload simply for convenience of access from anywhere. That is a coupling smell.

---

## Agent Guidelines

- **Read before writing.** Before modifying a scene or script, identify what signals it already emits, what `[Export]` properties it exposes, and what nodes it expects in its subtree.
- **Do not rename `[Export]` variables or signals** without noting that existing `.tscn` files will break — serialized property names are baked into the scene format.
- **Prefer small, targeted edits** over rewriting whole files. Godot `.tscn` files are fragile to wholesale regeneration.
- **Do not modify `.tscn` files directly**. Describe what changes are needed and let the developer apply them.
- **Do not add Autoloads unless explicitly asked.** Prefer scoped, injected solutions.
- C# in Godot requires a **build step** (`dotnet build`) before scene/script signal connections and source-generator output are live. Note in your response when a change requires recompilation before it will work.
- When uncertain whether a design fits Godot's architecture, bias toward the pattern that keeps nodes decoupled and scenes self-contained over the pattern that is merely convenient to write.