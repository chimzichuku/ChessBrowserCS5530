namespace ChessBrowser.Components.Models;

/// <summary>
/// Class for creating a Chess Game object
/// </summary>
public class ChessGame
{
    /// <summary>
    /// Game's round
    /// </summary>
    private string _round;

    /// <summary>
    /// Result of game
    /// </summary>
    private string _result;

    /// <summary>
    /// Moves made in game
    /// </summary>
    private string _moves;

    /// <summary>
    /// White player in game
    /// </summary>
    private ChessPlayer _whitePlayer;

    /// <summary>
    /// Black player in game
    /// </summary>
    private ChessPlayer _blackPlayer;

    /// <summary>
    /// Event where game is taking place
    /// </summary>
    private ChessEvent _event;

    /// <summary>
    /// Default constructor which will be used if no information is currently available
    /// </summary>
    public ChessGame()
    {
    }

    /// <summary>
    /// Constructor used to set the variables of the Game object
    /// </summary>
    /// <param name="round"> Game round </param>
    /// <param name="result"> Game result </param>
    /// <param name="moves"> Moves made in game </param>
    /// <param name="whitePlayer"> Game's white player </param>
    /// <param name="blackPlayer"> Game's black player </param>
    /// <param name="@event"> Event holding the game </param>
    public ChessGame(string round, string result, string moves, ChessPlayer whitePlayer, ChessPlayer blackPlayer,
        ChessEvent @event)
    {
        this._round = round;
        this._result = result;
        this._moves = moves;
        this._whitePlayer = whitePlayer;
        this._blackPlayer = blackPlayer;
        this._event = @event;
    }

    /// <summary>
    /// Get method for getting game's round
    /// </summary>
    /// <returns> This game's round </returns>
    public string GetRound()
    {
        return _round;
    }

    /// <summary>
    /// Set method for setting game's round
    /// </summary>
    /// <param name="round"> Game's round </param>
    public void SetRound(string round)
    {
        this._round = round;
    }

    /// <summary>
    /// Get method for getting game's result
    /// </summary>
    /// <returns> This game's result </returns>
    public string GetResult()
    {
        return _result;
    }

    /// <summary>
    /// Set method for setting game's result
    /// </summary>
    /// <param name="result">Game's result </param>
    public void SetResult(string result)
    {
        this._result = result;
    }

    /// <summary>
    /// Get method for getting game's moves
    /// </summary>
    /// <returns> This game's moves</returns>
    public string GetMoves()
    {
        return _moves;
    }

    /// <summary>
    /// Set method for setting game's moves
    /// </summary>
    /// <param name="moves"> Game's moves </param>
    public void SetMoves(string moves)
    {
        this._moves = moves;
    }

    /// <summary>
    /// Get method for getting game's white player
    /// </summary>
    /// <returns> This game's white player </returns>
    public ChessPlayer GetWhitePlayer()
    {
        return _whitePlayer;
    }

    /// <summary>
    /// Set method for setting game's white player
    /// </summary>
    /// <param name="whitePlayer"> Game's white player </param>
    public void SetWhitePlayer(ChessPlayer whitePlayer)
    {
        this._whitePlayer = whitePlayer;
    }

    /// <summary>
    /// Get method for getting game's black player
    /// </summary>
    /// <returns> Game's black player </returns>
    public ChessPlayer GetBlackPlayer()
    {
        return _blackPlayer;
    }

    /// <summary>
    /// Set method for setting game's black player 
    /// </summary>
    /// <param name="blackPlayer"> Game's black player </param>
    public void SetBlackPlayer(ChessPlayer blackPlayer)
    {
        this._blackPlayer = blackPlayer;
    }

    /// <summary>
    /// Get method for getting the event holding the game
    /// </summary>
    /// <returns> Event holding the game </returns>
    public ChessEvent GetEvent()
    {
        return _event;
    }

    /// <summary>
    /// Set method for setting the event holding the game
    /// </summary>
    /// <param name="event"> Event holding the game </param>
    public void SetEvent(ChessEvent @event)
    {
        this._event = @event;
    }
}