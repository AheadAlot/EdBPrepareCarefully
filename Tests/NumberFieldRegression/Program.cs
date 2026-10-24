using System;
using EdB.PrepareCarefully;
using UnityEngine;
using Verse;

class Program {
    static int checks;
    static readonly Rect Rect = new Rect(20, 20, 60, 22);
    static void Check(bool result, string name) {
        if (!result) throw new Exception("FAIL: " + name);
        checks++;
        Console.WriteLine("PASS: " + name);
    }
    static void SetEvent(EventType type, bool held, EventType? raw = null, int button = 0) {
        Input.LeftHeld = held;
        Event.current = new Event { type = type, rawType = raw ?? type, button = button, mousePosition = new Vector2(30, 30) };
    }
    static void Main() {
        foreach (bool age in new[] { true, false }) {
            string kind = age ? "age" : "equipment";
            var field = new WidgetNumberField { MaxValue = age ? 99 : 100000 };
            if (age) field.DragSlider = new DragSlider(0.4f, 20, 100) { maxValue = 99 };
            int value = 20, updates = 0;
            field.UpdateAction = next => { value = next; updates++; };
            SetEvent(EventType.MouseDown, true);
            field.Draw(Rect, value);
            Check(DragSlider.IsDragging(), kind + ": click starts drag");
            SetEvent(EventType.MouseUp, false);
            field.Draw(Rect, value);
            Check(!DragSlider.IsDragging() && field.DragSlider.valueUpdateCallback == null, kind + ": release clears drag and callback");
            // The outer 1.7 dispatcher hides MouseUp from the global manager.
            SetEvent(EventType.Used, false);
            DragSliderManager.MouseUp();
            DragSliderManager.Tick(100);
            Check(value == 20 && updates == 0, kind + ": movement after release cannot change value");
            SetEvent(EventType.Repaint, false);
            field.Draw(Rect, value);
            Check(GUI.GetNameOfFocusedControl() != "", kind + ": click focuses text field");
            GUI.TypedText = "42";
            field.Draw(Rect, value);
            DragSliderManager.Tick(100);
            field.Draw(Rect, value);
            Check(GUI.LastText == "42", kind + ": typed number survives later updates");
            Event.current.keyCode = KeyCode.Return;
            field.Draw(Rect, value);
            SetEvent(EventType.Repaint, false);
            field.Draw(Rect, value);
            Check(value == 42, kind + ": enter commits typed number");
        }
        int changed = 0;
        var slider = new DragSlider { maxValue = 100 };
        SetEvent(EventType.MouseDown, true);
        slider.OnGUI(Rect, 20, _ => changed++);
        SetEvent(EventType.Repaint, true);
        DragSliderManager.Tick(80);
        Check(changed > 0, "held drag still updates value");
        int before = changed;
        // No field drawing or MouseUp delivery, e.g. releasing outside the window.
        SetEvent(EventType.Used, false);
        DragSliderManager.Tick(80);
        Check(!DragSlider.IsDragging() && changed == before && slider.valueUpdateCallback == null,
            "button-state fallback stops consumed or missing release before updating");

        SetEvent(EventType.MouseDown, true);
        slider.OnGUI(Rect, 20, _ => changed++);
        SetEvent(EventType.Used, false, EventType.MouseUp);
        slider.OnGUI(Rect, 20, _ => changed++);
        Check(!DragSlider.IsDragging(), "raw mouse-up stops drag even when event type is used");

        SetEvent(EventType.MouseDown, true);
        slider.OnGUI(Rect, 20, _ => changed++);
        SetEvent(EventType.MouseUp, true, button: 1);
        slider.OnGUI(Rect, 20, _ => changed++);
        Check(DragSlider.IsDragging(), "right-button release does not stop left-button drag");
        DragSlider.StopDragging();
        Check(!DragSlider.IsDragging() && slider.valueUpdateCallback == null, "page cleanup stops owned drag");
        DragSlider.StopDragging();
        Check(!DragSlider.IsDragging(), "page cleanup is idempotent");

        SetEvent(EventType.MouseDown, true);
        slider.OnGUI(Rect, 20, _ => changed++);
        SetEvent(EventType.MouseUp, false);
        DragSliderManager.MouseUp();
        Check(!DragSlider.IsDragging() && slider.valueUpdateCallback == null, "normal manager completion remains supported");
        // Cleanup must not cancel a slider started by another mod or the game.
        SetEvent(EventType.MouseDown, true);
        DragSliderManager.DragSlider(Rect, 1, (_, _) => { }, (_, _) => { }, (_, _) => { });
        DragSlider.StopDragging();
        Check(DragSlider.IsDragging(), "cleanup does not stop unrelated global sliders");
        DragSliderManager.ForceStop();
        Console.WriteLine($"PASS: {checks} regression checks");
    }
}
