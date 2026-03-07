namespace ChessBrowser.Components.Models;

/// <summary>
/// Class for creating a Chess Event object
/// </summary>
public class ChessEvent
{
    /// <summary>
    /// Name of event
    /// </summary>
    private string _eventName;

    /// <summary>
    /// Site where event is taking place
    /// </summary>
    private string _eventSite;

    /// <summary>
    /// Date of the event
    /// </summary>
    private string _eventDate;

    /// <summary>
    /// Default constructor which will be used if no information is currently available
    /// </summary>
    public ChessEvent()
    {
    }

    /// <summary>
    /// Constructor used for setting the Event's name, site, and date
    /// </summary>
    /// <param name="eventName"> name of event </param>
    /// <param name="eventSite"> site of event </param>
    /// <param name="eventDate"> date of event </param>
    public ChessEvent(string eventName, string eventSite, string eventDate)
    {
        this._eventName = eventName;
        this._eventSite = eventSite;
        this._eventDate = eventDate;
    }

    /// <summary>
    /// Get method used for getting event name
    /// </summary>
    /// <returns> This event's name </returns>
    public string GetEventName()
    {
        return _eventName;
    }

    /// <summary>
    /// Set method used for setting the event's name
    /// </summary>
    /// <param name="eventName"> Name of event</param>
    public void SetEventName(string eventName)
    {
        this._eventName = eventName;
    }

    /// <summary>
    /// Get method used for getting the event's site
    /// </summary>
    /// <returns> This event's site </returns>
    public string GetEventSite()
    {
        return _eventSite;
    }

    /// <summary>
    /// Set method used for setting the event's site
    /// </summary>
    /// <param name="eventSite"> Site of event </param>
    public void SetEventSite(string eventSite)
    {
        this._eventSite = eventSite;
    }

    /// <summary>
    /// Get method used for getting the event's date
    /// </summary>
    /// <returns> This event's date </returns>
    public string GetEventDate()
    {
        return _eventDate;
    }

    /// <summary>
    /// Set method used for setting the Event's date
    /// </summary>
    /// <param name="eventDate"> Date of event </param>
    public void SetEventDate(string eventDate)
    {
        this._eventDate = eventDate;
    }
}