# Digital Clock

`DigitalClock` displays the current time in multiple time zones and works with Unity's built-in uGUI `Text` component, so it does not require TextMeshPro.

## Unity setup

1. Open or create a scene with a `Canvas`.
2. Create an empty UI object under the Canvas named `WorldClock`.
3. Add a `Vertical Layout Group` to `WorldClock` if you want generated rows stacked automatically. Recommended settings:
   - Child Alignment: Middle Center
   - Spacing: 8
   - Child Force Expand Width: enabled
   - Child Force Expand Height: disabled
4. Add `DigitalClock` from `Assets/Scripts/UI/DigitalClock/DigitalClock.cs` to `WorldClock`.
5. Leave **Rows Parent** empty to generate rows under `WorldClock`, or assign a child `RectTransform`.
6. Configure the `Clocks` list in the Inspector with IANA IDs such as:
   - `UTC`
   - `America/New_York`
   - `Europe/London`
   - `Asia/Tokyo`
   - `Australia/Sydney`
7. Choose 12/24-hour mode and whether seconds are shown.
8. Press Play. The clock refreshes four times per second without an `Update()` call.

## Notes

- Android and most Linux-based Unity targets use IANA time-zone IDs.
- The script includes fallbacks for common Windows time-zone IDs to support editor testing on Windows.
- Rows are generated at runtime and are destroyed/rebuilt when `Rebuild()` is called.
- Assign a custom `Font` in the Inspector for a branded digital-clock appearance. If none is assigned, Unity's built-in Arial font is used.

## Runtime configuration example

```csharp
var clock = FindObjectOfType<SurfaceRiseOfHumanity.UI.DigitalClock>();
clock.Rebuild();
```

For a production UI, place the clock behind a screen-safe-area panel and use a Canvas Scaler set to `Scale With Screen Size` for Android devices.
