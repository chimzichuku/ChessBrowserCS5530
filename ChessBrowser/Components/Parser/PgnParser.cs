using ChessBrowser.Components.Models;

namespace ChessBrowser.Components.Parser;


public class PgnParser
{
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

    private static void CheckEventUniqueness(ChessEvent currentEvent, Dictionary<(string Name, string Site, String Date), ChessEvent> eventsByKey)
    {
        if(!eventsByKey.TryGetValue((currentEvent.GetEventName(), currentEvent.GetEventSite(), currentEvent.GetEventDate()), out ChessEvent? value))
            eventsByKey.Add((currentEvent.GetEventName(), currentEvent.GetEventSite(), currentEvent.GetEventDate()), currentEvent);
    }

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