namespace Z3.UIBuilder
{
    public interface IHasIcon
    {
        public IconType IconType { get; }
    }

    /// <summary> <see cref="UIBuilder.Editor.EditorIcon"/> </summary>
    public enum IconType
    {
        None,
        Plus,
        Minus,
        Ok,
        Info,
        Warning,    
        Error,
        Invisible,
        Visible,
        Lamp,
        Box,
        Cog,
        Gamepad,
        VerticalLayoutGroup,
        ParticleSystemForceField,
        AudioMixerController,
        Eye,
        Globo,
        Avatar,
        Body,
        Cloud,
        Store,
        Pivot,
        Play
    }
}