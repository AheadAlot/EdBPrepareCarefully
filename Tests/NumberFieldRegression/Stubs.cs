// Minimal GUI/game doubles: tests execute the production number-field and slider code.
using System;

namespace UnityEngine {
    public enum EventType { MouseDown, MouseUp, Repaint, Ignore, Used }
    public enum KeyCode { None, Return, Tab, KeypadEnter }
    public enum TextAnchor { MiddleCenter, UpperLeft }
    public struct Vector2 { public float x, y; public Vector2(float x, float y) { this.x = x; this.y = y; } }
    public struct Rect {
        public float x, y, width, height;
        public Rect(float x, float y, float width, float height) { this.x = x; this.y = y; this.width = width; this.height = height; }
        public Rect(Rect other) { this = other; }
        public bool Contains(Vector2 p) => p.x >= x && p.x < x + width && p.y >= y && p.y < y + height;
    }
    public struct Color { public static Color white => new Color(); }
    public class Texture { }
    public class GUIStyle {
        public TextAnchor alignment;
        public GUIStyle() { }
        public GUIStyle(GUIStyle other) { }
    }
    public class Event {
        public static Event current = new Event();
        public EventType type, rawType;
        public int button;
        public bool shift;
        public KeyCode keyCode;
        public Vector2 mousePosition;
    }
    public static class Input {
        public static bool LeftHeld;
        public static bool GetMouseButton(int button) => button == 0 && LeftHeld;
    }
    public static class Time {
        public static float deltaTime = 0.02f;
        public static float realtimeSinceStartup;
    }
    public static class GUI {
        public static Color color;
        public static string Focused = "", TypedText, LastText;
        public static string GetNameOfFocusedControl() => Focused;
        public static void SetNextControlName(string name) { }
        public static void FocusControl(string name) { Focused = name ?? ""; }
        public static string TextField(Rect rect, string text, GUIStyle style) {
            LastText = text;
            string result = TypedText ?? text;
            TypedText = null;
            return result;
        }
        public static void DrawTexture(Rect rect, Texture texture) { }
    }
}
namespace RimWorld { }
namespace Verse {
    using UnityEngine;
    public enum GameFont { Small }
    public static class Text {
        public static GameFont Font;
        public static TextAnchor Anchor;
        public static GUIStyle CurTextFieldStyle = new GUIStyle();
    }
    public static class Widgets {
        public static void Label(Rect rect, string label) { }
        public static void DrawAtlas(Rect rect, Texture texture) { }
        public static bool ButtonInvisible(Rect rect, bool sound) => false;
    }
    public static class RectExtensions { public static float MiddleY(this Rect rect) => rect.y + rect.height / 2; }
    public class AcceptanceReport {
        public bool Accepted;
        public static implicit operator AcceptanceReport(bool value) => new AcceptanceReport { Accepted = value };
    }
    public class SoundDef { public static SoundDef Named(string name) => new SoundDef(); }
    public static class SoundDefOf {
        public static SoundDef Tick_Tiny = new SoundDef(), Tick_High = new SoundDef(), Tick_Low = new SoundDef();
    }
    public delegate void DragSliderCallback(float offset, float rate);
    public static class DragSliderManager {
        private static bool dragging;
        private static DragSliderCallback update, complete;
        public static bool DragSlider(Rect rect, float rate, DragSliderCallback start, DragSliderCallback changed, DragSliderCallback end) {
            if (Event.current.type != EventType.MouseDown || Event.current.button != 0 || !rect.Contains(Event.current.mousePosition)) return false;
            start(0, rate);
            dragging = true;
            update = changed;
            complete = end;
            return true;
        }
        public static void ForceStop() { dragging = false; }
        public static void Tick(float offset) { if (dragging) update(offset, 1); }
        public static void MouseUp() {
            if (dragging && Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                dragging = false;
                complete(0, 1);
            }
        }
    }
}
namespace Verse.Sound {
    using Verse;
    public enum MaintenanceType { None }
    public class SoundInfo { public static SoundInfo OnCamera(MaintenanceType type) => new SoundInfo(); }
    public static class SoundStarter { public static void PlayOneShot(SoundDef def, SoundInfo info) { } }
    public static class SoundExtensions { public static void PlayOneShotOnCamera(this SoundDef def) { } }
}
namespace EdB.PrepareCarefully {
    using UnityEngine;
    public static class Style {
        public static Color ColorText, ColorButtonDisabled, ColorButton;
        public static void SetGUIColorForButton(Rect rect) { }
    }
    public static class Textures {
        public static Texture TextureFieldAtlas = new Texture(), TextureButtonPrevious = new Texture(), TextureButtonNext = new Texture();
    }
}
