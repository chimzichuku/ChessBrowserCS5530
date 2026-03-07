using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using ChessBrowser.Components.Parser;
using ChessBrowser.Components.Models;

namespace ChessBrowser.Components.Pages
{
  public partial class ChessBrowser
  {
    /// <summary>
    /// Bound to the Unsername form input
    /// </summary>
    private string Username = "";

    /// <summary>
    /// Bound to the Password form input
    /// </summary>
    private string Password = "";

    /// <summary>
    /// Bound to the Database form input
    /// </summary>
    private string Database = "";

    /// <summary>
    /// Represents the progress percentage of the current
    /// upload operation. Update this value to update 
    /// the progress bar.
    /// </summary>
    private int Progress = 0;

    /// <summary>
    /// This method runs when a PGN file is selected for upload.
    /// Given a list of lines from the selected file, parses the 
    /// PGN data, and uploads each chess game to the user's database.
    /// </summary>
    /// <param name="PGNFileLines">The lines from the selected file</param>
    private async Task InsertGameData(string[] PGNFileLines)
    {
      // This will build a connection string to your user's database on atr,
      // assuimg you've filled in the credentials in the GUI
      string connection = GetConnectionString();

      // Parser expects file path and InsertGameData takes a string array
      // Create a temporary file to pass to parser
      string tempFile = Path.GetTempFileName();
      File.WriteAllLines(tempFile, PGNFileLines);
      PgnParser.ParseFile(tempFile, out Dictionary<string, ChessPlayer>? players, out Dictionary<(string Name, string Site, String Date), ChessEvent>? events, out List<ChessGame>? games);
      File.Delete(tempFile);

      using (MySqlConnection conn = new MySqlConnection(connection))
      {
        try
        {
          // Open a connection
          conn.Open();

          // Iterate through your data and generate appropriate insert commands
          Console.Write(games.Count + "\n");
          Console.Write(players.Count + "\n");
          Console.WriteLine(events.Count);

          int totalCommands = (events?.Count ?? 0) + (players?.Count ?? 0) + (games?.Count ?? 0);
          int completed = 0;
          if (events != null)
          {
            foreach (ChessEvent e in events.Values)
            {
              MySqlCommand cmd = GenerateEventInsertCommand(conn,e);
              cmd.ExecuteNonQuery();
              
              completed++;
              
              //this is essentially (completed / totalCommands) * 100, just ensures that the integer returned isnt
              //0 --> 50/200 == 0 for integers, (completed * 100 / totalCommands) ensures that this doesnt happen
              Progress = completed * 100 / totalCommands;
              
              await InvokeAsync(StateHasChanged);
            }
          }

          if (players != null)
          {
            foreach (ChessPlayer p in players.Values)
            {
              MySqlCommand cmd = GeneratePlayerInsertCommand(conn, p);
              cmd.ExecuteNonQuery();
              completed++;
              Progress = completed * 100 / totalCommands;
              await InvokeAsync(StateHasChanged);
            }
          }

          if (games != null)
          {
            foreach (ChessGame g in games)
            {
              MySqlCommand cmd = GenerateGameInsertCommand(conn, g);
              cmd.ExecuteNonQuery();
              completed++;
              Progress = completed * 100 / totalCommands;
              await InvokeAsync(StateHasChanged);
            }
          }

        }
        catch (Exception e)
        {
          System.Diagnostics.Debug.WriteLine(e.Message);
        }
      }

    }


    /// <summary>
    /// Queries the database for games that match all the given filters.
    /// The filters are taken from the various controls in the GUI.
    /// </summary>
    /// <param name="white">The white player, or "" if none</param>
    /// <param name="black">The black player, or "" if none</param>
    /// <param name="opening">The first move, e.g. "1.e4", or "" if none</param>
    /// <param name="winner">The winner as "W", "B", "D", or "" if none</param>
    /// <param name="useDate">true if the filter includes a date range, false otherwise</param>
    /// <param name="start">The start of the date range</param>
    /// <param name="end">The end of the date range</param>
    /// <param name="showMoves">true if the returned data should include the PGN moves</param>
    /// <returns>A string separated by newlines containing the filtered games</returns>
    private string PerformQuery(string white, string black, string opening,
      string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
    {
      // This will build a connection string to your user's database on atr,
      // assuimg you've typed a user and password in the GUI
      string connection = GetConnectionString();

      // Build up this string containing the results from your query
      string parsedResult = "";

      // Use this to count the number of rows returned by your query
      // (see below return statement)
      int numRows = 0;

      using (MySqlConnection conn = new MySqlConnection(connection))
      {
        try
        {
          // Open a connection
          conn.Open();

          // TODO:
          //   Generate and execute an SQL command,
          //   then parse the results into an appropriate string and return it.
        }
        catch (Exception e)
        {
          // System.Diagnostics.Debug.WriteLine(e.Message);
          Console.WriteLine(e.Message);
        }
      }

      return numRows + " results\n" + parsedResult;
    }


    private string GetConnectionString()
    {
      return "server=atr.eng.utah.edu;database=" + Database + ";uid=" + Username + ";password=" + Password;
    }


    /// <summary>
    /// This method will run when the file chooser is used.
    /// It loads the files contents as an array of strings,
    /// then invokes the InsertGameData method.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
      try
      {
        string fileContent = string.Empty;

        InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
        if (eventArgs.FileCount == 1)
        {
          var file = eventArgs.File;
          if (file is null)
          {
            return;
          }

          // load the chosen file and split it into an array of strings, one per line
          using var stream = file.OpenReadStream(1000000); // max 1MB
          using var reader = new StreamReader(stream);
          fileContent = await reader.ReadToEndAsync();
          string[] fileLines = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

          // insert the games, and don't wait for it to finish
          // _ = throws away the task result, since we aren't waiting for it
          _ = InsertGameData(fileLines);
        }
      }
      catch (Exception e)
      {
        Debug.WriteLine("an error occurred while loading the file..." + e);
      }
    }

    // Helper methods:

    private MySqlCommand GenerateEventInsertCommand(MySqlConnection conn, ChessEvent e)
    {
      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = "INSERT IGNORE INTO Events (Name, Site, Date) VALUES (@name, @site, @date);";
      cmd.Parameters.AddWithValue("@name", e.GetEventName());
      cmd.Parameters.AddWithValue("@site", e.GetEventSite());
      cmd.Parameters.AddWithValue("@date", e.GetEventDate());

      return cmd;
    }

    private MySqlCommand GeneratePlayerInsertCommand(MySqlConnection conn, ChessPlayer p)
    {
      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = "INSERT INTO Players (Name, Elo) VALUES (@name, @elo) ON DUPLICATE KEY UPDATE Elo = IF(Elo < @elo, @elo, Elo);";
      cmd.Parameters.AddWithValue("@name", p.GetPlayerName());
      cmd.Parameters.AddWithValue("@elo", p.GetEloRating());

      return cmd;
    }

    private MySqlCommand GenerateGameInsertCommand(MySqlConnection conn, ChessGame g)
    {
      // Get eID to add to Games table as a foreign key to Events table
      MySqlCommand getEID = conn.CreateCommand();
      getEID.CommandText = "SELECT eID FROM Events WHERE Name = @name and Site = @site and Date = @date;";
      getEID.Parameters.AddWithValue("@name", g.GetEvent().GetEventName());
      getEID.Parameters.AddWithValue("@site", g.GetEvent().GetEventSite());
      getEID.Parameters.AddWithValue("@date", g.GetEvent().GetEventDate());

      int eID = 0;
      int whiteID = 0;
      int blackID = 0;
      
      using (MySqlDataReader reader = getEID.ExecuteReader())
      {
        if(reader.Read())
        {
          eID = Convert.ToInt32(reader["eID"]);
        }
      }
      
      //Get White Player ID
      MySqlCommand getWhiteID = conn.CreateCommand();
      getWhiteID.CommandText = "Select pID From Players Where Name = @name";
      getWhiteID.Parameters.AddWithValue("@name", g.GetWhitePlayer().GetPlayerName());

      using (MySqlDataReader reader = getWhiteID.ExecuteReader())
      {
        if (reader.Read())
          whiteID = Convert.ToInt32(reader["pID"]);
      }
      
      //Get Black Player ID
      MySqlCommand getBlackID = conn.CreateCommand();
      getBlackID.CommandText = "Select pID From Players Where Name = @name";
      getBlackID.Parameters.AddWithValue("@name", g.GetBlackPlayer().GetPlayerName());

      using (MySqlDataReader reader = getBlackID.ExecuteReader())
      {
        if (reader.Read())
          blackID = Convert.ToInt32(reader["pID"]);
      }

      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = "INSERT INTO Games (Round, Result, Moves, WhitePlayer, BlackPlayer, eID) VALUES (@round, @result, @moves, @whiteplayer, @blackplayer, @eID);";
      cmd.Parameters.AddWithValue("@round", g.GetRound());
      cmd.Parameters.AddWithValue("@result", g.GetResult());
      cmd.Parameters.AddWithValue("@moves", g.GetMoves());
      cmd.Parameters.AddWithValue("@whiteplayer", whiteID);
      cmd.Parameters.AddWithValue("@blackplayer", blackID);
      cmd.Parameters.AddWithValue("@eID", eID);

      return cmd;
    }

  }
}


