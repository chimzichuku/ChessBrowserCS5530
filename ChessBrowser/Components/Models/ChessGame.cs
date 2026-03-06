namespace ChessBrowser.Components.Models;

public class ChessGame
{
    /// <summary>
    /// 
    /// </summary>
    private string _round;
    /// <summary>
    /// 
    /// </summary>
    private string _result;
    /// <summary>
    /// 
    /// </summary>
    private string _moves;
    /// <summary>
    /// 
    /// </summary>
    private ChessPlayer _whitePlayer;
    /// <summary>
    /// 
    /// </summary>
    private ChessPlayer _blackPlayer;

    /// <summary>
    /// 
    /// </summary>
    private ChessEvent _event;

    /// <summary>
    /// 
    /// </summary>
    public ChessGame()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="round"></param>
    /// <param name="result"></param>
    /// <param name="moves"></param>
    /// <param name="whitePlayer"></param>
    /// <param name="blackPlayer"></param>
    /// <param name="eID"></param>
    public ChessGame(string round, string result, string moves, ChessPlayer whitePlayer, ChessPlayer blackPlayer)
    {
        this._round = round;
        this._result = result;
        this._moves = moves;
        this._whitePlayer = whitePlayer;
        this._blackPlayer = blackPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetRound()
    {
        return _round;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="round"></param>
    public void SetRound(string round)
    {
        this._round = round;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetResult()
    {
        return _result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    public void SetResult(string result)
    {
        this._result = result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetMoves()
    {
        return _moves;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moves"></param>
    public void SetMoves(string moves)
    {
        this._moves = moves;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ChessPlayer GetWhitePlayer()
    {
        return _whitePlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="whitePlayer"></param>
    public void SetWhitePlayer(ChessPlayer whitePlayer)
    {
        this._whitePlayer = whitePlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ChessPlayer GetBlackPlayer()
    {
        return _blackPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackPlayer"></param>
    public void SetBlackPlayer(ChessPlayer blackPlayer)
    {
        this._blackPlayer = blackPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ChessEvent GetEvent()
    {
        return _event;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="event"></param>
    public void SetEvent(ChessEvent @event)
    {
        this._event = @event;
    }
}