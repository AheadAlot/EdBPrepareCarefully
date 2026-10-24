# Health compatibility regression checks

Build the mod in Release first, then run against the RimWorld 1.7 assemblies:

```powershell
dotnet run --project Tests/HealthCompatibilityRegression -c Release -p:RimWorldDir="E:\Steam\steamapps\common\RimWorld" -- "E:\Steam\steamapps\common\RimWorld"
```

No test framework or running game is required. Checks use the real game and mod assemblies for null body-part lookups, whole-body/local option matching, recipe identity, save-record conversion, and the implant dialog's conflict filter. The dialog is allocated without its UI constructor; these are not in-game UI or full preset round-trip tests.

Manual checks still required on 1.6 and 1.7: add a Monolyn, confirm the implant dialog, edit a normal prosthesis, save/load a pawn and preset, and start a game. Verify that whole-body conditions remain and that no repeated exceptions or duplicate conditions appear.
