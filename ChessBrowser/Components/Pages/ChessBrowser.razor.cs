using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using ChessBrowser.Components.Parser;
using ChessBrowser.Components.Models;
using System.Runtime.InteropServices;

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
            PgnParser.ParseFile(tempFile, out Dictionary<string, ChessPlayer>? players,
                out Dictionary<(string Name, string Site, String Date), ChessEvent>? events,
                out List<ChessGame>? games);
            File.Delete(tempFile);

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Iterate through your data and generate appropriate insert commands
                    int totalCommands = (events?.Count ?? 0) + (players?.Count ?? 0) + (games?.Count ?? 0);
                    int completed = 0;
                    if (events != null)
                    {
                        foreach (ChessEvent e in events.Values)
                        {
                            MySqlCommand cmd = GenerateEventInsertCommand(conn, e);
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

                    // Generate and execute an SQL command,
                    // then parse the results into an appropriate string and return it.

                    MySqlCommand cmd = conn.CreateCommand();
                    string whereConditions =
                        BuildWhereCondition(white, black, opening, winner, useDate, start, end, cmd);
                    cmd.CommandText = BuildSelectQuery(showMoves) + whereConditions;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            numRows++;
                            parsedResult += BuildResultString(reader, showMoves);
                        }
                    }
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

                InputFileChangeEventArgs eventArgs =
                    args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
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
                    string[] fileLines =
                        fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

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

        /// <summary>
        /// Creates an SQL command that inserts an event into the Events table.
        /// INSERT IGNORE is used so duplicate events are skipped instead of causing errors.
        /// </summary>
        /// <param name="conn">The open MySQL connection.</param>
        /// <param name="e">The event to insert.</param>
        /// <returns>A parameterized MySqlCommand for inserting the event.</returns>
        private MySqlCommand GenerateEventInsertCommand(MySqlConnection conn, ChessEvent e)
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT IGNORE INTO Events (Name, Site, Date) VALUES (@name, @site, @date);";
            cmd.Parameters.AddWithValue("@name", e.GetEventName());
            cmd.Parameters.AddWithValue("@site", e.GetEventSite());
            cmd.Parameters.AddWithValue("@date", e.GetEventDate());

            return cmd;
        }

        /// <summary>
        /// Creates an SQL command that inserts a player into the Players table.
        /// For added protection, The "ON DUPLCATE KEY UPDATE" command was added in the event that a player already exists
        /// their Elo is updated only if the new Elo is higher. This was taken care of during parsing as well
        /// </summary>
        /// <param name="conn">The open MySQL connection.</param>
        /// <param name="p">The player to insert or update.</param>
        /// <returns>A parameterized MySqlCommand for inserting or updating the player.</returns>
        private MySqlCommand GeneratePlayerInsertCommand(MySqlConnection conn, ChessPlayer p)
        {
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO Players (Name, Elo) VALUES (@name, @elo) ON DUPLICATE KEY UPDATE Elo = IF(Elo < @elo, @elo, Elo);";
            cmd.Parameters.AddWithValue("@name", p.GetPlayerName());
            cmd.Parameters.AddWithValue("@elo", p.GetEloRating());

            return cmd;
        }

        /// <summary>
        /// Creates an SQL command that inserts a game into the Games table.
        /// This method first retrieves the foreign key IDs for the game's event, white player, and black player.
        /// </summary>
        /// <param name="conn">The open MySQL connection.</param>
        /// <param name="g">The game to insert.</param>
        /// <returns>A parameterized MySqlCommand for inserting the game.</returns>
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
                if (reader.Read())
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
            cmd.CommandText =
                "INSERT IGNORE INTO Games (Round, Result, Moves, WhitePlayer, BlackPlayer, eID) VALUES (@round, @result, @moves, @whiteplayer, @blackplayer, @eID);";
            cmd.Parameters.AddWithValue("@round", g.GetRound());
            cmd.Parameters.AddWithValue("@result", g.GetResult());
            cmd.Parameters.AddWithValue("@moves", g.GetMoves());
            cmd.Parameters.AddWithValue("@whiteplayer", whiteID);
            cmd.Parameters.AddWithValue("@blackplayer", blackID);
            cmd.Parameters.AddWithValue("@eID", eID);

            return cmd;
        }

        /// <summary>
        /// Builds the WHERE clause for the query based on whichever filters the user entered in the GUI.
        /// </summary>
        /// <param name="whitePlayer">The white player's name filter.</param>
        /// <param name="blackPlayer">The black player's name filter.</param>
        /// <param name="openingMove">The opening move filter, such as "1.e4".</param>
        /// <param name="winner">The result filter ("W", "B", or "D").</param>
        /// <param name="useDateFilter">True if a date range filter should be used.</param>
        /// <param name="startDate">The start date of the filter range.</param>
        /// <param name="endDate">The end date of the filter range.</param>
        /// <param name="cmd">The command receiving the matching SQL parameters.</param>
        /// <returns>A WHERE clause string, or an empty string if no filters were provided.</returns>
        private string BuildWhereCondition(string whitePlayer, string blackPlayer, string openingMove, string winner,
            bool useDateFilter, DateTime startDate, DateTime endDate, MySqlCommand cmd)
        {
            List<string> conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(whitePlayer))
            {
                conditions.Add("p1.Name = @whiteplayer");
                cmd.Parameters.AddWithValue("@whiteplayer", whitePlayer);
            }

            if (!string.IsNullOrWhiteSpace(blackPlayer))
            {
                conditions.Add("p2.Name = @blackplayer");
                cmd.Parameters.AddWithValue("@blackplayer", blackPlayer);
            }

            if (!string.IsNullOrWhiteSpace(openingMove))
            {
                conditions.Add("g.Moves LIKE @openingmove");
                cmd.Parameters.AddWithValue("@openingmove", openingMove + "%");
            }

            if (!string.IsNullOrWhiteSpace(winner))
            {
                conditions.Add("g.Result = @winner");
                cmd.Parameters.AddWithValue("@winner", winner);
            }

            if (useDateFilter)
            {
                conditions.Add("e.Date >= @startDate AND e.Date <= @endDate");
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
            }

            if (conditions.Count > 0)
            {
                return "WHERE " + string.Join(" AND ", conditions);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Builds the WHERE clause for the query based on whichever filters the user entered in the GUI.
        /// </summary>
        /// <param name="whitePlayer">The white player's name filter.</param>
        /// <param name="blackPlayer">The black player's name filter.</param>
        /// <param name="openingMove">The opening move filter, such as "1.e4".</param>
        /// <param name="winner">The result filter ("W", "B", or "D").</param>
        /// <param name="useDateFilter">True if a date range filter should be used.</param>
        /// <param name="startDate">The start date of the filter range.</param>
        /// <param name="endDate">The end date of the filter range.</param>
        /// <param name="cmd">The command receiving the matching SQL parameters.</param>
        /// <returns>A WHERE clause string, or an empty string if no filters were provided.</returns>
        private string BuildSelectQuery(bool showMoves)
        {
            string moves = "";

            if (showMoves)
            {
                moves = ", g.Moves";
            }

            return
                "SELECT e.Name as eName, e.Site, e.Date, p1.Name as wName, p1.Elo as wElo, p2.Name as bName, p2.Elo as bElo, g.Result" +
                moves + " " + "FROM Games g " + "JOIN Events e ON g.eID = e.eID " +
                "JOIN Players p1 ON g.WhitePlayer = p1.pID " + "JOIN Players p2 ON g.BlackPlayer = p2.pID ";
        }

        private string BuildResultString(MySqlDataReader reader, bool showMoves)
        {
            string result = "\nEvent: " + reader["eName"] + "\nSite: " + reader["Site"] + "\nDate: " + Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd") +
                            "\nWhite: " + reader["wName"] + " (" + reader["wElo"] + ")\nBlack: " + reader["bName"] +
                            " (" + reader["bElo"] + ")\nResult: " + reader["Result"];

            if (showMoves)
            {
                result += "\n" + reader["Moves"];
            }

            result += "\n";

            return result;
        }
    }
}