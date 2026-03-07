using ChessBrowser.Components.Models;

namespace ChessBrowser.Components.Parser;

/// <summary>
/// Class used to hold the methods needed for parsing PGN Files
/// </summary>
public class PgnParser
{
    /// <summary>
    /// Reads a PGN file and parses its contents into collections of unique players, unique events, and chess games.
    /// </summary>
    /// <param name="filename"> The path to the PGN file to parse</param>
    /// <param name="players"> Output dictionary containing unique players keyed by player name </param>
    /// <param name="events"> Output dictionary containing unique events keyed by the tuple (event name, event site, event date </param>
    /// <param name="games"> Output list containing all parsed chess games from the file </param>
    public static void ParseFile(string filename, out Dictionary<string, ChessPlayer>? players, out Dictionary<(string Name, string Site, String Date), ChessEvent>? events, out List<ChessGame>? games)
    {
        Dictionary<string, ChessPlayer> playerByName = new();
        Dictionary<(string Name, string Site, String Date), ChessEvent> eventsByKey = new();
        List<ChessGame> gamesList = new();
        
        try
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string? line;
                
                ChessGame currentGame = new ChessGame();
                ChessPlayer currentWhite = new ChessPlayer();
                ChessPlayer currentBlack = new ChessPlayer();
                ChessEvent currentEvent = new ChessEvent();
                
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        
                        if (!string.IsNullOrWhiteSpace(currentGame.GetMoves()))
                        {
                            CheckPlayerUniqueness(currentBlack, currentWhite, playerByName);
                            CheckEventUniqueness(currentEvent, eventsByKey);
                            currentGame.SetWhitePlayer(currentWhite);
                            currentGame.SetBlackPlayer(currentBlack);
                            // Add event to game to so eID can be retrieved for foreign key in DB
                            currentGame.SetEvent(currentEvent);
                            gamesList.Add(currentGame);
                            currentGame = new ChessGame();
                            currentWhite = new ChessPlayer();
                            currentBlack = new ChessPlayer();
                            currentEvent = new ChessEvent();
                        }
                        continue;
                    }
                    ParseEvent(line, currentEvent);
                    ParseGame(line, currentGame);
                    ParseBlackPlayer(line, currentBlack);
                    ParseWhitePlayer(line, currentWhite);
                }
                
                //in the event that the file uploaded does not end with an empty line, this will ensure the last
                //game is added
                if (!string.IsNullOrWhiteSpace(currentGame.GetMoves()))
                {
                    CheckPlayerUniqueness(currentBlack, currentWhite, playerByName);
                    CheckEventUniqueness(currentEvent, eventsByKey);
                    currentGame.SetWhitePlayer(currentWhite);
                    currentGame.SetBlackPlayer(currentBlack);
                    // Add event to game to so eID can be retrieved for foreign key in DB
                    currentGame.SetEvent(currentEvent);
                    gamesList.Add(currentGame);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"File could not be read with message: {e.Message}");
        }
        players = playerByName;
        games = gamesList;
        events = eventsByKey;
    }

    /// <summary>
    /// Helper method used to check if the Player has already been added. If so, the Elo is updated depending on if the
    /// new Elo is higher. If it isn't, then the previous Elo remains.
    /// </summary>
    /// <param name="currentBlack"> The current black player that is playing in the current game </param>
    /// <param name="currentWhite"> The current white player that is playing in the current game </param>
    /// <param name="playerByName"> Dictionary containing all unique players </param>
    private static void CheckPlayerUniqueness(ChessPlayer currentBlack, ChessPlayer currentWhite, Dictionary<string, ChessPlayer> playerByName)
    {
        if (playerByName.TryGetValue(currentWhite.GetPlayerName(), out ChessPlayer? whiteP))
        {
            if (whiteP.GetEloRating() < currentWhite.GetEloRating()) 
                whiteP.SetEloRating(currentWhite.GetEloRating());
        }
        else
            playerByName.Add(currentWhite.GetPlayerName(), currentWhite);

        if (playerByName.TryGetValue(currentBlack.GetPlayerName(), out ChessPlayer? blackP))
        {
            if (blackP.GetEloRating() < currentBlack.GetEloRating())
                blackP.SetEloRating(currentBlack.GetEloRating());
        }
        else
            playerByName.Add(currentBlack.GetPlayerName(), currentBlack);
    }

    /// <summary>
    /// Method used to check if an event is unique. If it is, it is added to teh events dictionary. If not, nothing happens.
    /// </summary>
    /// <param name="currentEvent"> The current event that is hosting the current game. </param>
    /// <param name="eventsByKey"> Dictionary containing all the unique events </param>
    private static void CheckEventUniqueness(ChessEvent currentEvent, Dictionary<(string Name, string Site, String Date), ChessEvent> eventsByKey)
    {
        if(!eventsByKey.TryGetValue((currentEvent.GetEventName(), currentEvent.GetEventSite(), currentEvent.GetEventDate()), out ChessEvent? value))
            eventsByKey.Add((currentEvent.GetEventName(), currentEvent.GetEventSite(), currentEvent.GetEventDate()), currentEvent);
    }

    /// <summary>
    /// This helper method is used to determine if the current line is containing information for the current event
    /// </summary>
    /// <param name="line"> The line of text extracted from the PGN file </param>
    /// <param name="currentEvent"> The current event taking place </param>
    private static void ParseEvent(string line, ChessEvent currentEvent)
    {
        if (line.StartsWith("[Event "))
        {
            string eventName = line.Split('"')[1];
            currentEvent.SetEventName(eventName);
        }
        
        if (line.StartsWith("[Site"))
        {
            string siteName = line.Split('"')[1];
            currentEvent.SetEventSite(siteName);
        }
        
        if (line.StartsWith("[EventDate"))
        {
            string date = line.Split('"')[1];
            string[] dateValues = date.Split(".");
            
            if ( dateValues.Length == 3 && !date.Contains("?") )
                currentEvent.SetEventDate($"{dateValues[0]}-{dateValues[1]}-{dateValues[2]}");
            else
                currentEvent.SetEventDate("0000-00-00");
        }
    }

    /// <summary>
    /// This helper method is used to determine if the current line is containing information for the current game
    /// </summary>
    /// <param name="line"> The line of text extracted from the PGN file </param>
    /// <param name="currentGame"> The current game taking place </param>
    private static void ParseGame(string line, ChessGame currentGame)
    {
        if (line.StartsWith("[Round"))
        {
            string round = line.Split('"')[1];
            currentGame.SetRound(round);
        }
        
        if (line.StartsWith("[Result"))
        {
            string result = line.Split('"')[1];
            if( result == "1-0")
                currentGame.SetResult("W");
            else if (result == "0-1")
                currentGame.SetResult("B");
            else if(result == "1/2-1/2")
                currentGame.SetResult("D");
        }
        
        if (!line.StartsWith("["))
        {
            if (line.StartsWith("1."))
                currentGame.SetMoves(line);
            else
                currentGame.SetMoves(currentGame.GetMoves() + line);
        }
    }

    /// <summary>
    /// This helper method is used to determine if the current line is containing information for the current white player
    /// </summary>
    /// <param name="line"> The line of text extracted from the PGN file </param>
    /// <param name="currentWhite"> The current white player in the current game taking place </param>
    private static void ParseWhitePlayer(string line, ChessPlayer currentWhite)
    {
        if (line.StartsWith("[White "))
        {
            string whitePName = line.Split('"')[1];
            currentWhite.SetPlayerName(whitePName);
        }
        
        if (line.StartsWith("[WhiteElo"))
        {
            string whiteElo = line.Split('"')[1];
            currentWhite.SetEloRating(int.Parse(whiteElo));
        }
    }

    /// <summary>
    /// This helper method is used to determine if the current line is containing information for the current black player
    /// </summary>
    /// <param name="line"> The line of text extracted from the PGN file </param>
    /// <param name="currentBlack"> The current black player in the current game taking place </param>
    private static void ParseBlackPlayer(string line, ChessPlayer currentBlack)
    {
        if (line.StartsWith("[Black "))
        {
            string blackPName = line.Split('"')[1];
            currentBlack.SetPlayerName(blackPName);
        }
        
        if (line.StartsWith("[BlackElo"))
        {
            string blackElo = line.Split('"')[1];
            currentBlack.SetEloRating(int.Parse(blackElo));
        }
    }
}