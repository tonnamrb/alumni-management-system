namespace Domain.Enums;

/// <summary>
/// Defines the type of post content
/// </summary>
public enum PostType
{
    /// <summary>
    /// Text only post
    /// </summary>
    Text = 0,
    
    /// <summary>
    /// Single image post
    /// </summary>
    Image = 1,
    
    /// <summary>
    /// Multiple images (album)
    /// </summary>
    Album = 2,
    
    /// <summary>
    /// Video post (may include multiple images as well)
    /// </summary>
    Video = 3
}