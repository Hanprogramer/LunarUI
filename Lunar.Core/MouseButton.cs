namespace Lunar.Native
{
    public enum MouseButton
    {
        /// <summary>
        /// Indicates the input backend was unable to determine a button name for the button in question, or it does not support it.
        /// </summary>
        Unknown = -1, // 0xFFFFFFFF
        /// <summary>The left mouse button.</summary>
        Left = 0,
        /// <summary>The right mouse button.</summary>
        Right = 1,
        /// <summary>The middle mouse button.</summary>
        Middle = 2,
        /// <summary>The fourth mouse button.</summary>
        Button4 = 3,
        /// <summary>The fifth mouse button.</summary>
        Button5 = 4,
        /// <summary>The sixth mouse button.</summary>
        Button6 = 5,
        /// <summary>The seventh mouse button.</summary>
        Button7 = 6,
        /// <summary>The eighth mouse button.</summary>
        Button8 = 7,
        /// <summary>The ninth mouse button.</summary>
        Button9 = 8,
        /// <summary>The tenth mouse button.</summary>
        Button10 = 9,
        /// <summary>The eleventh mouse button.</summary>
        Button11 = 10, // 0x0000000A
        /// <summary>The twelth mouse button.</summary>
        Button12 = 11, // 0x0000000B
    }
}
