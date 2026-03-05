using ChessBrowser.Components.Models;

namespace ChessBrowser.Components.Parser;
using ChessBrowser.Components;

public class PgnImport
{

    public void Import(string file)
    {
        PgnParser.ParseFile(file, out Dictionary<string, ChessPlayer>? players, 
            out Dictionary<(string Name, string Site, string Date), ChessEvent>? events, out List<ChessGame>? games);
        
    }
    
}