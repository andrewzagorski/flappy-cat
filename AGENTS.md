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

### Braces — Allman Style
Opening braces go on their own line, at the same indentation level as the control statement:

```csharp
// Correct
if (x > 0)
{
    DoSomething();
}

// Wrong
if (x > 0) {
    DoSomething();
}
```

Single-line brackets are acceptable only for: simple property accessors, simple object/array/collection initializers, and abstract auto-property/indexer/event declarations.

### Blank Lines
**Insert** a blank line:
- After the `using` block.
- Between methods, properties, and inner type declarations.
- At the end of each file.

**Avoid** a blank line:
- After `{` or before `}`.
- After a comment block or single-line comment.
- Adjacent to another blank line.

### Spaces
**Insert** a space:
- Around binary and ternary operators.
- Between an opening parenthesis and `if`, `for`, `foreach`, `catch`, `while`, `lock`, `using`.
- Before/within single-line accessor blocks; between accessors.
- After commas (not at end of line) and semicolons in `for` statements.
- Around `:` in type declarations and `=>` lambda arrows.
- After `//`, and before it when inline at end of a line.
- Inside braces of single-line initializers.

**Do not** insert a space:
- After type-cast parentheses: `i += (int)MyProperty;`

---

## Naming Conventions

| Identifier | Convention | Example |
|---|---|---|
| Namespaces, types, members (methods, properties, constants, events) | `PascalCase` | `PlayerCharacter`, `DefaultSpeed` |
| Local variables, method arguments | `camelCase` | `attackStrength` |
| Private fields | `_camelCase` (underscore prefix) | `_aimingAt` |
| Interfaces | `IPascalCase` | `IDamageable` |
| Two-letter acronyms | All caps where PascalCase expected | `UI`, `ID` |

- `id` is **not** an acronym — treat it as a normal identifier: `public string Id { get; }`.
- Do **not** prefix identifiers with type names (e.g., avoid `strText`, `fPower`).
- Choose **descriptive names** over abbreviated ones.

```csharp
// Correct
FindNearbyEnemy()?.Damage(weaponDamage);

// Wrong
FindNode()?.Change(wpnDmg);
```

---

## Variables

- **Member variables**: only declare if used in more than one method; otherwise use local variables.
- **Local variables**: declare as close as possible to first use.
- **`var`**: use only when the type is obvious from the right-hand side.

```csharp
// OK
var direction = new Vector2(1, 0);
var text = "Some value";

// Not OK — type is not evident
var value = GetValue();
var velocity = direction * 1.5;
```

---

## Godot-Specific Notes

- Node scripts must use `partial class` to work with Godot's source generators.
- Prefer `[Export]` properties over public fields for inspector-exposed values.
- Use `GetNode<T>()` with a cached `[Export]`-assigned node reference rather than magic strings where possible.
- Signal callbacks should follow the naming pattern `On<NodeName><SignalName>` (e.g., `OnButtonPressed`).

---

## Other Conventions

- Always use **explicit access modifiers**.
- Use **properties** instead of non-private fields.
- Modifier order: `public` / `protected` / `private` / `internal` / `virtual` / `override` / `abstract` / `new` / `static` / `readonly`.
- Avoid unnecessary `this.` prefix or fully-qualified names.
- Remove unused `using` statements and unnecessary parentheses.
- Omit default initial values for types where reasonable.
- Prefer null-conditional operators and type initializers for compact code.
- Use **safe cast** (`as`) when the type may differ; use **direct cast** when it is known.