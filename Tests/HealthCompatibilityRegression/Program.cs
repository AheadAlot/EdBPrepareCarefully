using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Reflection;
using RimWorld;
using EdB.PrepareCarefully;
using Verse;

class Program {
    static int checks;
    static void Main(string[] args) {
        if (args.Length != 1) throw new ArgumentException("Pass the RimWorld 1.7 installation directory.");
        string gameDir = Path.GetFullPath(args[0]);
        AssemblyLoadContext.Default.Resolving += (context, name) => {
            string path = Path.Combine(gameDir, name.Name + ".dll");
            return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
        };
        Run();
        Console.WriteLine($"PASS: {checks} regression checks");
    }
    static void Check(bool value, string name) {
        if (!value) throw new Exception("FAIL: " + name);
        checks++;
        Console.WriteLine("PASS: " + name);
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Run() {
        LanguageDatabase.activeLanguage = new LoadedLanguage("English");
        var options = new OptionsHealth();
        Check(options.FindBodyPartsForRecord(null) == null, "null body record lookup");
        Check(!options.FindBodyPartsForDef(null).Any(), "null body definition lookup");
        var brain = new BodyPartDef { defName = "TestBrain" };
        var condition = new HediffDef { defName = "TestWholeBody", label = "test condition" };
        var local = new ImplantOption { HediffDef = condition, BodyPartDefs = new HashSet<BodyPartDef> { brain } };
        var global = new ImplantOption { HediffDef = condition, BodyPartDefs = new HashSet<BodyPartDef> { null } };
        options.AddImplantOption(local);
        options.AddImplantOption(global);
        Check(!options.ImplantOptionTargetsBodyPart(local, null), "local option rejects whole body");
        Check(options.ImplantOptionTargetsBodyPart(global, null), "explicit whole-body target");
        Check(!options.ImplantOptionTargetsBodyPart(global, brain), "global option rejects brain");
        Check(options.FindImplantOptionThatAddsHediffDefToBodyPart(condition, null) == global, "find whole-body option");
        Check(options.FindImplantOptionThatAddsHediffDefToBodyPart(condition, brain) == local, "find local option");
        Check(options.FindImplantOptionsThatAddHediff(new Hediff { def = condition }).Single() == global, "whole-body hediff mapping");
        var recipe1 = new RecipeDef { defName = "TestRecipe1", addsHediff = condition, targetsBodyPart = true, appliedOnFixedBodyParts = new List<BodyPartDef> { brain } };
        var recipe2 = new RecipeDef { defName = "TestRecipe2", addsHediff = condition, targetsBodyPart = true, appliedOnFixedBodyParts = new List<BodyPartDef> { brain } };
        var first = new ImplantOption { RecipeDef = recipe1, HediffDef = condition };
        var second = new ImplantOption { RecipeDef = recipe2, HediffDef = condition };
        options.AddImplantOption(first);
        options.AddImplantOption(second);
        Check(options.FindImplantOptionThatAddsRecipeDefToBodyPart(recipe2, brain) == second, "recipe identity matching");
        Check(!options.RecipeTargetsBodyPart(recipe1, null), "targeted recipe rejects whole body");
        var wholeRecipe = new RecipeDef { defName = "TestWholeRecipe", targetsBodyPart = false, addsHediff = condition };
        Check(options.RecipeTargetsBodyPart(wholeRecipe, null), "whole-body recipe target");
        var implant = new Implant { HediffDef = condition, Severity = 2.5f };
        var saved = new SaveRecordImplantV5(implant);
        Check(saved.bodyPart == null && saved.hediffDef == condition.defName && saved.severity == 2.5f, "save whole-body state without runtime hediff");
        var part = new BodyPartRecord { def = brain };
        saved = new SaveRecordImplantV5(new Implant { BodyPartRecord = part, Recipe = recipe1 });
        Check(saved.bodyPart == brain.defName && saved.hediffDef == condition.defName && saved.recipe == recipe1.defName, "save local state with recipe fallback");
        // Exercise the dialog's actual conflict filter without initializing the game UI.
        part.groups = new List<BodyPartGroupDef>();
        options.AddBodyPart(new UniqueBodyPart { Record = part, Ancestors = new List<UniqueBodyPart>() });
        var pawnDef = new ThingDef { defName = "TestPawn" };
        var customizedPawn = new CustomizedPawn { Pawn = new Pawn { def = pawnDef } };
        var provider = new ProviderHealthOptions();
        var cache = (Dictionary<Tuple<ThingDef, MutantDef>, OptionsHealth>)typeof(ProviderHealthOptions)
            .GetField("optionsLookup", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider);
        cache.Add(Tuple.Create(pawnDef, (MutantDef)null), options);
        implant.Recipe = wholeRecipe;
        wholeRecipe.label = "whole-body condition";
        recipe1.label = "local condition";
        var unknownLocal = new Implant { HediffDef = condition, BodyPartRecord = part, Recipe = recipe1 };
        var dialog = (DialogManageImplants)RuntimeHelpers.GetUninitializedObject(typeof(DialogManageImplants));
        void Set(string name, object value) => typeof(DialogManageImplants).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dialog, value);
        Set("customizedPawn", customizedPawn);
        Set("providerHealth", provider);
        Set("implantList", new List<Implant> { implant, unknownLocal });
        Set("replacedParts", new Dictionary<BodyPartRecord, Implant>());
        var valid = new List<Implant>();
        Set("validImplants", valid);
        Set("options", new List<DialogManageImplants.DialogOption> {
            new DialogManageImplants.DialogOption { ImplantOption = global, Parts = new List<DialogManageImplants.ImplantBodyPart> {
                new DialogManageImplants.ImplantBodyPart { Selected = true, Implant = implant }
            }}
        });
        typeof(DialogManageImplants).GetMethod("ResetDisabledState", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(dialog, null);
        Check(valid.Contains(implant), "dialog preserves whole-body condition without option");
        Check(valid.Contains(unknownLocal), "dialog preserves unknown local condition");
    }
}
