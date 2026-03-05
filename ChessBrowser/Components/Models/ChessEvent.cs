namespace ChessBrowser.Components.Models;

public class ChessEvent
{
    /// <summary>
    /// 
    /// </summary>
    private string _eventName;
    /// <summary>
    /// 
    /// </summary>
    private string _eventSite;
    /// <summary>
    /// 
    /// </summary>
    private string _eventDate;

    /// <summary>
    /// 
    /// </summary>
    public ChessEvent()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventSite"></param>
    /// <param name="eventDate"></param>
    /// <param name="eID"></param>
    public ChessEvent(string eventName, string eventSite, string eventDate)
    {
        this._eventName = eventName;
        this._eventSite = eventSite;
        this._eventDate = eventDate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetEventName()
    {
        return _eventName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventName"></param>
    public void SetEventName(string eventName)
    {
        this._eventName = eventName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetEventSite()
    {
        return _eventSite;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventSite"></param>
    public void SetEventSite(string eventSite)
    {
        this._eventSite = eventSite;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetEventDate()
    {
        return _eventDate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventDate"></param>
    public void SetEventDate(string eventDate)
    {
        this._eventDate = eventDate;
    }
}