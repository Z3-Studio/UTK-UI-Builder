using UnityEngine.UIElements;

namespace Z3.UIBuilder.Editor
{
    public class JoystickPreview : VisualElement
    {
        public JoystickPreview()
        {
            Vector2Field result = new();
            result.SetEnabled(false);

            Joystick joystick = new();
            joystick.onUpdate += (v) => result.value = v;

            result.value = joystick.Value;

            Add(joystick);
            Add(result);
        }
    }
}