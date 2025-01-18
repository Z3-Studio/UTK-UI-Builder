using System;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.UIBuilder
{
    public class Joystick : VisualElement
    {
        public Vector2 Value { get; private set; }
        public Action<Vector2> onUpdate;

        // State
        private Vector2 moveValue;
        private Vector2 startDragPosition;
        private bool isDragging;

        // Initial variables
        private readonly VisualElement handle;
        private readonly float handleInitialLeft;
        private readonly float handleInitialTop;
        private readonly float backgroundRadius;
        private readonly float handleRadius;

        // Default Values
        private const float defaultBackgroundSize = 100f;
        private const float backgrounBorderSize = 2;
        private const float defaultHandleSize = 40f;
        private readonly Color joystickColor = new(0.7607843f, 0.07843138f, 0.1647059f);

        public Joystick(float backgroundSize = defaultBackgroundSize, float handleSize = defaultHandleSize)
        {
            backgroundRadius = backgroundSize / 2f;
            handleRadius = handleSize / 2f;
            // Initialize initial positions
            handleInitialLeft = (backgroundSize - handleSize) / 2;
            handleInitialTop = (backgroundSize - handleSize) / 2;

            // Create the background circle
            VisualElement backgroundCircle = new VisualElement();
            backgroundCircle.style.width = backgroundSize;
            backgroundCircle.style.height = backgroundSize;
            backgroundCircle.style.alignSelf = Align.Center;
            backgroundCircle.style.SetBorderRadius(backgroundRadius);
            backgroundCircle.style.SetMargin(4f);
            backgroundCircle.style.SetBorderColor(joystickColor);
            backgroundCircle.style.SetBorderWidth(backgrounBorderSize);

            // Create the handle
            handle = new VisualElement();
            handle.style.width = handleSize;
            handle.style.height = handleSize;
            handle.style.backgroundColor = joystickColor;

            handle.style.SetBorderRadius(handleRadius);
            handle.style.position = Position.Absolute;
            handle.style.left = handleInitialLeft;
            handle.style.top = handleInitialTop;
            handle.style.marginLeft = -backgrounBorderSize;
            handle.style.marginTop = -backgrounBorderSize;

            backgroundCircle.Add(handle);
            Add(backgroundCircle);

            // Register event handlers
            handle.RegisterCallback<PointerDownEvent>(OnPointerDown);
            handle.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            handle.RegisterCallback<PointerUpEvent>(OnPointerUp);

            // Schedule the UpdateMapScroll to run periodically
            schedule.Execute(UpdateMapScroll).Every(1); // Every 16ms (~60fps)
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            isDragging = true;
            startDragPosition = evt.position;
            handle.CapturePointer(evt.pointerId);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging)
                return;

            Vector2 delta = (Vector2)evt.position - startDragPosition;

            float maxDistance = backgroundRadius - handleRadius; // Maximum distance the handle can move from the center
            float distance = delta.magnitude;
            if (distance > maxDistance)
            {
                delta = delta.normalized * maxDistance;
            }

            handle.style.left = handleInitialLeft + delta.x;
            handle.style.top = handleInitialTop + delta.y;

            moveValue = delta; // Normalize value between -1 and 1
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            isDragging = false;
            handle.ReleasePointer(evt.pointerId);

            // Reset handle position
            handle.style.left = handleInitialLeft;
            handle.style.top = handleInitialTop;

            Value = Vector2.zero;
            onUpdate?.Invoke(Value);
        }

        private void UpdateMapScroll()
        {
            if (!isDragging)
                return;

            float maxDistance = backgroundRadius - handleRadius;
            Value = moveValue / maxDistance;

            onUpdate?.Invoke(Value);
        }
    }
}