using System.ComponentModel.DataAnnotations;

namespace MO.LiveChat.Configs;

public class UserUpdaterConfiguration
{
    public bool Activated { get; set; }
    /// <summary>
    /// Run every given seconds.
    /// </summary>
    [Range(5, int.MaxValue)]
    public int UpdaterTimeSpanInSec { get; set; }
}