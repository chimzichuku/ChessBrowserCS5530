namespace ChessBrowser.Components.Models;

/// <summary>
/// Class for creating a Chess Player object
/// </summary>
public class ChessPlayer
{
    /// <summary>
    /// The name of the player
    /// </summary>
    private string _name;

    /// <summary>
    /// The Elo rating of the player
    /// </summary>
    private int _elo;

    /// <summary>
    /// Default constructor which will be used if no information is currently available
    /// </summary>
    public ChessPlayer()
    {
    }

    /// <summary>
    /// Constructor used for assigning the players name and Elo rating
    /// </summary>
    /// <param name="name"> The name of the player </param>
    /// <param name="elo"> The player's Elo rating </param>
    public ChessPlayer(string name, int elo)
    {
        this._name = name;
        this._elo = elo;
    }

    /// <summary>
    /// Get method for getting player's nanme
    /// </summary>
    /// <returns> Player's name </returns>
    public string GetPlayerName()
    {
        return _name;
    }

    /// <summary>
    /// Set method for setting the player's name
    /// </summary>
    /// <param name="name"> Name of Player </param>
    public void SetPlayerName(string name)
    {
        this._name = name;
    }

    /// <summary>
    /// Get method for getting the player's Elo rating
    /// </summary>
    /// <returns> Player's Elo rating </returns>
    public int GetEloRating()
    {
        return _elo;
    }

    /// <summary>
    /// Set method for setting the player's elo rating
    /// </summary>
    /// <param name="elo"> Value to replace player's Elo rating </param>
    public void SetEloRating(int elo)
    {
        this._elo = elo;
    }
}