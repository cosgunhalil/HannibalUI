# HannibalUI C# Coding Style Guide

This document defines the coding conventions for the HannibalUI package. It's adapted from
[Unity's "Use a C# style guide for clean and scalable game code"](https://unity.com) e-book
(2nd edition, 2025) and tailored to this codebase's existing patterns.

These are team standards, not compiler-enforced rules. When in doubt, follow this guide over
personal preference — consistency across the codebase matters more than any individual
choice. If a rule here conflicts with the Microsoft Framework Design Guidelines, this document
takes precedence for HannibalUI code.

**Existing code note:** parts of `Runtime/` predate this guide and don't fully conform (e.g.
`VP_Canvas`/`VP_UIObject` use unprefixed PascalCase protected fields like `ObjectRectTransform`
instead of `_camelCase`). Don't do a drive-by rename of unrelated code just to match this guide
— bring a file into line when you're already touching it for another reason.

## Identifiers

- No special characters (Unicode, backslashes) in identifiers — they can break Unity's
  command-line tooling.
- Favor readability over brevity. `CanScrollHorizontally` beats `ScrollableX`.

### Casing

| Casing | Example | Used for |
|---|---|---|
| PascalCase | `MaxHealthPoints` | classes, interfaces, enums, public fields/properties/methods, namespaces |
| camelCase | `maxHealthPoints` | local variables, method parameters |
| `_camelCase` | `_maxHealthPoints` | private and protected fields |
| `k_PascalCase` | `k_MaxItems` | constants |
| `s_PascalCase` | `s_Instance` | static fields |

Hungarian notation (`int iCounter`, `string strName`) is not used.

## Fields and variables

- **Nouns for variables**, except `bool`s — those get a verb prefix that phrases them as a
  question: `isDead`, `isWalking`, `hasDamageMultiplier`.
- **Don't abbreviate** unless it's math or a loop counter (`i`, `j`). `movementSpeed`, not
  `mvmtSpeed`. Names should be searchable and pronounceable — this also helps AI-assisted
  tooling generate accurate completions.
- **One declaration per line.**
- **Avoid redundant names.** In `VP_Canvas`, a field doesn't need to be called
  `CanvasActivationTime` — `ActivationTime` already carries the context.
- **Drop redundant initializers.** Don't write `private bool _isDead = false;` — fields default
  to `0` / `null` / `false` already.
- **`private` is implicit and may be omitted** at field/method scope for brevity (as the
  existing code already does). Be consistent within a file: don't mix explicit and implicit
  `private` in the same class.
- **Use `var`** when the type is obvious from the right-hand side (`var pool = new
  ObjectPool<VP_Popup>(10, prefab);`). Avoid it when the return type isn't clear from context
  (`var result = SomeFactory.Create();`).

```csharp
// EXAMPLE
public class VP_ExampleUIObject : VP_UIObject
{
    public float DamageMultiplier = 1.5f;
    public bool IsInvincible;

    private bool _isDead;
    private float _currentHealth;

    private const int k_MaxStackSize = 99;
    private static int s_ActiveInstanceCount;
}
```

## Enums

- PascalCase for the enum name and its values.
- Singular noun for the enum name (`CanvasType`, not `CanvasTypes`) — it represents one value
  from a set.
- Exception: enums marked `[Flags]` are plural, since they represent a combination of values.
- No prefix or suffix on the name (`CanvasType`, not `ECanvasType` or `CanvasTypeEnum`).

```csharp
public enum CanvasType
{
    Main,
    Market,
    Characters
}

[Flags]
public enum PointerStates
{
    None = 0,
    Hovered = 1,
    Pressed = 2
}
```

## Classes and interfaces

- PascalCase noun or noun phrase for class names (`VP_Director`, not `VP_Direct`).
- One `MonoBehaviour` per file, and the file name must match the class name.
- Prefix interfaces with `I` followed by an adjective (`IAnimable`, `IPoolable`, `IObserver<T>`).
- **Keep classes small and single-responsibility.** If a `VP_Canvas` subclass starts handling
  input, animation timing, *and* network calls, split it — see `Runtime/Helpers/` for the
  existing pattern of pulling reusable, non-Unity-dependent logic into its own small assembly.
- Class member order:
  1. Fields
  2. Properties
  3. Events / delegates
  4. `MonoBehaviour` lifecycle methods (`Awake`, `Start`, `PreInit`/`Init`/`LateInit`, `OnDestroy`, ...)
  5. Public methods
  6. Private methods

```csharp
public class VP_ExampleCanvas : VP_Canvas
{
    public int PublicField;
    private int _packagePrivate;

    public void DoSomething() { }
}

public interface IKillable
{
    void Kill();
}
```

## Methods

- Start the name with a verb or verb phrase: `GetDirection`, `EnableCanvas`, `PlayForward`.
- Methods that return `bool` ask a question: `IsGameOver`, `HasStartedTurn`.
- camelCase for parameters, same as local variables.
- **Fewer arguments is better.** If a method needs many, consider a struct/config object.
- **Avoid boolean flag parameters that switch behavior.** Don't write `GetAngle(bool
  inRadians)` — write `GetAngleInDegrees()` and `GetAngleInRadians()` instead.
- **No side effects.** A method should do only what its name says. Prefer passing by value;
  if you use `out`/`ref`, that should be the method's one job.
- Avoid excessive overloading — implement only the overloads you actually call.

## Events

HannibalUI has its own typed observer system (`ISubject<T>`/`IObserver<T>`,
`VP_EventBroadcaster`, `VP_UIEvent`) rather than raw C# events — follow that pattern for
UI-level state changes. For plain C# events elsewhere in the codebase:

- Name the event with a verb phrase, present or past participle for before/after:
  `DoorOpening` / `DoorOpened`.
- Prefix the event-raising method with `On`: `OnDoorOpened()`.
- Use `System.Action` / `Action<T>` for the delegate type unless you need multiple named
  parameters, in which case define a small custom `EventArgs`-derived struct.

```csharp
public event Action DoorOpened;

public void OnDoorOpened()
{
    DoorOpened?.Invoke();
}
```

## Asynchronous code

HannibalUI uses [UniTask](https://github.com/Cysharp/UniTask) for anything that used to be a
`MonoBehaviour` coroutine (waiting, sequencing, delayed activation). **Don't add new
`StartCoroutine`/`IEnumerator` code** — `VP_Director.EnableRequestedCanvasAsync` and
`VP_Canvas.DeactivateWithAnimationAsync` are the reference examples for the pattern below.

- Fire-and-forget entry points (the direct replacement for what used to be a coroutine kicked
  off from a non-async method) are `private async UniTaskVoid MethodNameAsync(...)`, called
  without `await` from the sync method that starts them. Suffix these methods `Async`.
- **Every `UniTaskVoid` method that can be cancelled takes a `CancellationToken` parameter** and
  passes it into every `await` inside it (`UniTask.Delay(..., cancellationToken: token)`, etc.).
  Don't write an async method with no way to cancel it.
- The token comes from a `CancellationTokenSource` field owned by the class. Cancel *and*
  dispose the previous one before creating a new one, so re-triggering the same operation (e.g.
  calling `EnableCanvas` again mid-transition) cancels the in-flight run instead of letting two
  overlapping runs race:

  ```csharp
  private CancellationTokenSource _myOperationCts;

  public void StartMyOperation()
  {
      _myOperationCts?.Cancel();
      _myOperationCts?.Dispose();
      _myOperationCts = new CancellationTokenSource();
      MyOperationAsync(_myOperationCts.Token);
  }

  private async UniTaskVoid MyOperationAsync(CancellationToken cancellationToken)
  {
      try
      {
          await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
      }
      catch (OperationCanceledException)
      {
          return;
      }

      // ... continue only if not cancelled
  }
  ```
- **Cancel (and dispose) the `CancellationTokenSource` in `OnDestroy`/`OnDestroyCalled`.** An
  in-flight `UniTask` that outlives its owning `MonoBehaviour` is the async equivalent of a
  coroutine leak.
- Catch `OperationCanceledException` explicitly around the awaited call and `return` — don't let
  it propagate unhandled just because UniTask usually logs it quietly. Being explicit makes the
  cancel-and-bail-out path visible in the method body.

## Namespaces

- PascalCase, no underscores.
- All runtime code uses the `HannibalUI.Runtime.<Folder>` dot-delimited style, matching the
  `Runtime/` folder/assembly layout (`HannibalUI.Runtime.Base`, `.Animation`, `.Utilities`,
  `.UIElements`, `.Helpers.Observer`, `.Helpers.Memory`). The old reverse-domain
  `com.voxelpixel.hannibal_ui.*` C# namespaces were migrated away — **don't reintroduce them.**
  This is separate from the UPM package id `com.voxelpixel.hannibal-ui` in `package.json`, which
  stays as-is.

## Formatting

- **Allman brace style** (opening brace on its own line) — matches the existing codebase.
- 4-space indentation, no tabs.
- Don't omit braces, even for single-line `if`/`for` bodies.
- Single space after commas in argument lists; no space between a method name and its `(`;
  no spaces just inside parens or brackets.
- Single space around binary/comparison operators and before a flow-control `(`:
  `if (x == y)`, not `if(x==y)`.
- Keep lines under ~120 characters; break long statements up rather than letting them overflow.
- Don't use column alignment for field declarations — it creates diff noise as fields are
  added/removed.
- Indent `switch` cases; include a `default:` even when every enum case is already handled, so
  the code is defensive against new enum values.

### Properties

- Use expression-bodied properties for simple read-only accessors backed by a private field:

```csharp
private int _maxHealth;
public int MaxHealth => _maxHealth;
```

- Otherwise use `{ get; set; }` / `{ get; private set; }`. Reserve full methods (`GetMaxHealth()`)
  for accessors that do real computation, not simple field access.

### Serialization

- Prefer `[SerializeField] private T _fieldName;` over `public T FieldName;` when the field
  should be Inspector-editable but not part of the public API — this is already the pattern used
  throughout `Runtime/Base/` (e.g. `VP_Canvas.uIObjects`).
- Use `[Range(min, max)]` for bounded numeric fields.
- Group related Inspector fields into a `[Serializable]` struct/class rather than a flat list of
  loose fields.

## Comments

- Prefer self-explanatory names and small methods over comments explaining *what* code does.
  Comments should explain *why*, not *what* — a non-obvious constraint, a workaround, an
  invariant.
- `//` on its own line above the code it explains, not trailing at end-of-line.
- Use `[Tooltip("...")]` instead of a comment for serialized Inspector fields.
- `/// <summary>` XML doc comments are fine on public methods where the intent isn't obvious
  from the signature.
- Delete commented-out code — rely on git history instead.
- Don't leave stale `TODO`s. The codebase currently has a couple left (`//TODO: generate
  automatically!` in `VP_UIEvents.cs`, `//TODO: solve the order issue!` in `VP_Director.cs`) —
  these are legitimate as long as they still describe real, intended future work. Remove a
  `TODO` once it's done, or once you've decided it's not actually going to happen — the
  `//TODO: use Unitask instead of coroutine!` that used to be here is a recent example: it was
  deleted once `VP_Director`/`VP_Canvas` were migrated (see [Asynchronous code](#asynchronous-code)).
- No attribution comments (`// added by X`) — that's what `git blame` is for.

## Common pitfalls to watch for

- **Needless complexity / God objects** — e.g. don't let `VP_Director` grow beyond
  orchestrating canvases; push canvas-specific logic down into the `VP_Canvas` subclass.
- **Duplicate logic** — there is one `VP_Button` (`Runtime/UIElements/VP_Button.cs`); extend it
  or subclass it rather than copying a second button implementation. It was recently consolidated
  from two near-identical classes, so resist the urge to reintroduce a parallel one.
- **Fragility from broken single-responsibility** — if a small change to one class requires
  touching several unrelated classes, that's a signal responsibilities are tangled.

## References

- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Microsoft .NET Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- Unity, *Use a C# style guide for clean and scalable game code* (2nd ed., 2025) — source
  e-book this document was adapted from (`coding-style.pdf` in the project root).
