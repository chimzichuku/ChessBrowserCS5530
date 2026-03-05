namespace ChessBrowser.Components.Models;

public class ChessPlayer
{
    /// <summary>
    /// 
    /// </summary>
    private string _name;
    /// <summary>
    /// 
    /// </summary>
    private int _elo;

    /// <summary>
    /// 
    /// </summary>
    public ChessPlayer()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="elo"></param>
    /// <param name="pID"></param>
    public ChessPlayer(string name, int elo)
    {
        this._name = name;
        this._elo = elo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return _name;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public void SetPlayerName(string name)
    {
        this._name = name;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetEloRating()
    {
        return _elo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="elo"></param>
    public void SetEloRating(int elo)
    {
        this._elo = elo;
    }
}