namespace HannibalUI.Editor
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Read-only visualization of a <see cref="UIProjectConfig"/>: transition "event" nodes on the
    /// left, screen nodes on the right, with arrows from each event to its target screen. Transitions
    /// are global (event → target), so the layout is bipartite rather than screen-to-screen.
    /// </summary>
    public class NavigationFlowView : VisualElement
    {
        private const float NodeWidth = 150f;
        private const float NodeHeight = 34f;
        private const float RowGap = 14f;
        private const float ColumnGap = 120f;
        private const float Pad = 8f;

        private static readonly Color NodeColor = new Color(0.22f, 0.22f, 0.22f);
        private static readonly Color EventColor = new Color(0.18f, 0.24f, 0.30f);
        private static readonly Color BorderColor = new Color(0f, 0f, 0f, 0.4f);
        private static readonly Color StartColor = new Color(0.30f, 0.65f, 0.35f);
        private static readonly Color PopupColor = new Color(0.30f, 0.22f, 0.32f);
        private static readonly Color ArrowColor = new Color(0.72f, 0.72f, 0.72f);

        private readonly List<(Vector2 from, Vector2 to)> _edges = new List<(Vector2, Vector2)>();

        public NavigationFlowView()
        {
            style.flexGrow = 1f;
            style.minHeight = 160f;
            style.marginTop = 6f;
            generateVisualContent += OnGenerateVisualContent;
        }

        public void Rebuild(UIProjectConfig config)
        {
            Clear();
            _edges.Clear();

            if (config == null)
            {
                Add(Message("No config selected."));
                MarkDirtyRepaint();
                return;
            }

            var screenLeftAnchors = new Dictionary<string, Vector2>();
            int screenRow = 0;
            float screenX = Pad + NodeWidth + ColumnGap;

            foreach (var screen in config.Screens)
            {
                if (screen.IsPopup || string.IsNullOrWhiteSpace(screen.Name))
                {
                    continue;
                }

                float y = Pad + screenRow * (NodeHeight + RowGap);
                Add(MakeNode(screen.Name, screenX, y, screen.IsStart ? StartColor : NodeColor, screen.IsStart));
                screenLeftAnchors[screen.Name] = new Vector2(screenX, y + NodeHeight * 0.5f);
                screenRow++;
            }

            if (screenRow == 0)
            {
                Add(Message("Add a screen to see the flow."));
                MarkDirtyRepaint();
                return;
            }

            int eventRow = 0;
            foreach (var transition in config.Transitions)
            {
                string label = string.IsNullOrWhiteSpace(transition.EventName)
                    ? "(unnamed)"
                    : $"{transition.EventName}  [{transition.Action}]";

                float y = Pad + eventRow * (NodeHeight + RowGap);
                Add(MakeNode(label, Pad, y, EventColor, false));

                if (!string.IsNullOrWhiteSpace(transition.TargetScreen)
                    && screenLeftAnchors.TryGetValue(transition.TargetScreen, out var target))
                {
                    var from = new Vector2(Pad + NodeWidth, y + NodeHeight * 0.5f);
                    _edges.Add((from, target));
                }

                eventRow++;
            }

            float popupX = screenX + NodeWidth + ColumnGap;
            int popupRow = 0;
            foreach (var screen in config.Screens)
            {
                if (!screen.IsPopup || string.IsNullOrWhiteSpace(screen.Name))
                {
                    continue;
                }

                float y = Pad + popupRow * (NodeHeight + RowGap);
                Add(MakeNode("Popup: " + screen.Name, popupX, y, PopupColor, false));
                popupRow++;
            }

            int rows = Mathf.Max(Mathf.Max(screenRow, eventRow), popupRow);
            style.minHeight = Pad * 2f + rows * (NodeHeight + RowGap);

            MarkDirtyRepaint();
        }

        private static Label MakeNode(string text, float x, float y, Color background, bool isStart)
        {
            var label = new Label(text);
            var s = label.style;
            s.position = Position.Absolute;
            s.left = x;
            s.top = y;
            s.width = NodeWidth;
            s.height = NodeHeight;
            s.unityTextAlign = TextAnchor.MiddleCenter;
            s.color = Color.white;
            s.backgroundColor = background;
            s.overflow = Overflow.Hidden;

            float borderWidth = isStart ? 2f : 1f;
            Color borderColor = isStart ? StartColor : BorderColor;
            s.borderTopWidth = borderWidth;
            s.borderBottomWidth = borderWidth;
            s.borderLeftWidth = borderWidth;
            s.borderRightWidth = borderWidth;
            s.borderTopColor = borderColor;
            s.borderBottomColor = borderColor;
            s.borderLeftColor = borderColor;
            s.borderRightColor = borderColor;
            s.borderTopLeftRadius = 4f;
            s.borderTopRightRadius = 4f;
            s.borderBottomLeftRadius = 4f;
            s.borderBottomRightRadius = 4f;

            return label;
        }

        private static Label Message(string text)
        {
            var label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Italic;
            label.style.marginTop = 8f;
            return label;
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            if (_edges.Count == 0)
            {
                return;
            }

            var painter = context.painter2D;
            painter.strokeColor = ArrowColor;
            painter.lineWidth = 2f;

            foreach (var edge in _edges)
            {
                painter.BeginPath();
                painter.MoveTo(edge.from);
                painter.LineTo(edge.to);
                painter.Stroke();

                DrawArrowHead(painter, edge.from, edge.to);
            }
        }

        private static void DrawArrowHead(Painter2D painter, Vector2 from, Vector2 to)
        {
            Vector2 dir = (to - from).normalized;
            if (dir == Vector2.zero)
            {
                return;
            }

            Vector2 perp = new Vector2(-dir.y, dir.x);
            const float size = 8f;
            Vector2 baseLeft = to - dir * size + perp * (size * 0.5f);
            Vector2 baseRight = to - dir * size - perp * (size * 0.5f);

            painter.BeginPath();
            painter.MoveTo(baseLeft);
            painter.LineTo(to);
            painter.LineTo(baseRight);
            painter.Stroke();
        }
    }
}
