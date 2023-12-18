using System;
using System.Collections.Generic;


public class Program
{
     public static void Main(string[] args)
          {
               ConsoleInterface interf = new ConsoleInterface();
               interf.LaunchInterface();

               /*
               TournamentTable tournament = new TournamentTable("KKKK");
               Game game1 = new Game(new Dictionary<string, int>{
                    {"team1", 4},
                    {"team2", 5}
               });
               Game game2 = new Game(new Dictionary<string, int>{
                    {"team1", 11},
                    {"team2", 7}
               });
               //Console.WriteLine(Game.GetInfoForTeams(new Game[] {game1}));
               foreach (KeyValuePair<string, Dictionary<string, int>> pair in Game.GetInfoForTeams(new Game[] {game1, game2})) {
                    Console.WriteLine("Team {0}", pair.Key);
                    foreach (KeyValuePair<string, int> values in pair.Value) {
                         Console.WriteLine("    {0}: {1}", values.Key, values.Value);
                    }
               }
               tournament.AddGame(game1);
               tournament.AddGame(game2);
               tournament.PrintSheets();
               */
          }
}

public class ConsoleInterface{

     List<TournamentTable> tournaments = new List<TournamentTable>();

     TournamentTable SelectTournament(string question) {
          // Provides terminal interface for user to pick one tournament out of existing ones.

          Console.WriteLine(question);

          for (int i = 0; i < tournaments.Count; i++) {
               Console.WriteLine("[{0}]: {1}", i, tournaments[i].name);
          }

          Console.Write("[selectTournament] >>> ");
          string choice = Console.ReadLine() ?? "";
          return tournaments[int.Parse(choice)];
     }

     void CreateTournament(){

          Console.Write("[mktour]Specify tournament name >>> ");
          string? tournamentName = Console.ReadLine();

          if (tournamentName == null){
               throw new ArgumentNullException("A tournament name must be specified.");
          }

          tournaments.Add(new TournamentTable(tournamentName));
     }

     void AddGame() {
          TournamentTable tournament = SelectTournament("[addgame]Which tournament should the game be added to?");
          Console.Write("[addgame]Name of the first team >>> ");
          string team1Name = Console.ReadLine() ?? "N/A";
          Console.Write("[addgame]Goals scored >>> ");
          int team1goals = int.Parse(Console.ReadLine() ?? "0");
          Console.Write("[addgame]Name of the second team >>> ");
          string team2Name = Console.ReadLine() ?? "N/A";
          Console.Write("[addgame]Goals scored >>> ");
          int team2goals = int.Parse(Console.ReadLine() ?? "0");

          tournament.AddGame(new Game(new Dictionary<string, int>{
                    {team1Name, team1goals},
                    {team2Name, team2goals}
          }));
     }

     void PrintSheet(){
          TournamentTable tournament = SelectTournament("[showtour]Choose target tournament:");
          tournament.PrintSheets();
     }

     void ListTours(){
          foreach (TournamentTable tournament in tournaments) {
               Console.Write(tournament.name + "  ");
          }
          Console.WriteLine();
     }

     void Help(){
          Console.WriteLine(
     @"Available commands:
     mktour - create new tournament table
     addgame - add game into the tournament
     tourtable - display tournament's spreadsheet
     lstours - display names of all existing tournaments
     exit - close console interface
     help - display this text"
          );
     }

     public void LaunchInterface() {

          Console.WriteLine("\nStarting BoSh v1.1 ...\n"); //BoSh - Bohdan Shell
          Console.WriteLine("Type \"help\" to list all available commands.");

          bool exit = false;
          while (!exit) {
               Console.Write(">>> ");
               string? command = Console.ReadLine();
               
               switch(command) {
                    case "mktour":
                         CreateTournament();
                         break;
                    case "addgame":
                         AddGame();
                         break;
                    case "help":
                         Help();
                         break;
                    case "tourtable":
                         PrintSheet();
                         break;
                    case "lstours":
                         ListTours();
                         break;
                    case "exit":
                         exit = true;
                         break;
                    case "":
                         break;
                    default:
                         Console.WriteLine("Unknown command \"{0}\"", command);
                         break;
               }
          }
          
     }
}

public class TournamentTable {

     public string name;
     public List<string> teamNames = new List<string>();
     public List<Game> games = new List<Game>();

     public TournamentTable(string name) {
          this.name = name;
     }

     public void AddGame(Game game) {
          // Adds team names from game to team names array of the tournament only if they don't exist already.
          foreach (string name in game.teams) {
               if (!teamNames.Contains(name)) {
                    teamNames.Add(name);
               }
          }

          games.Add(game);
     }

     public void PrintSheets(){

          Console.WriteLine("\nPrinting spreadsheet for tournament \"{0}\"...\n", name);

          Dictionary<string, Dictionary<string, int>> teamData = Game.GetInfoForTeams(games.ToArray());

          Console.Write(" ".PadLeft(15, '/'));
          foreach (string teamName in teamData.Keys) {
               Console.Write(teamName.PadRight(15));
          }
          Console.WriteLine("Score".PadRight(15) + "Points".PadRight(15) + "Positions".PadRight(15));

          foreach (string teamName in teamData.Keys) {
               Console.Write(teamName.PadRight(15));
               // Prints goals scored in each game
               foreach (string opponentName in teamData.Keys){
                    Game game = Game.SearchByTeams(games.ToArray(), teamName, opponentName);
                    if (game == null) {
                         Console.Write("N/A".PadRight(15));
                         continue;
                    }
                    Console.Write($"{game.goals[teamName]["goalsScored"]}:{game.goals[opponentName]["goalsScored"]}".PadRight(15));
               }
               // Prints total scored/mised goals of the team
               Console.Write($"{teamData[teamName]["goalsScored"]}:{teamData[teamName]["goalsMissed"]}".PadRight(15));
               // Prints total points of the team
               Console.Write($"{teamData[teamName]["points"]}".PadRight(15));
               // Print team's position
               Console.WriteLine($"{teamData[teamName]["position"]}".PadRight(15));
          }

     }
}

public class Game {
     public string[] teams = new string[2];

     public Dictionary<string, int> points = new Dictionary<string, int>();
     public Dictionary<string, Dictionary<string, int>> goals = new Dictionary<string, Dictionary<string, int>>();

     public Game(Dictionary<string, int> goalsInput) { // Dictionary<teamName, Dictionary<parameter [goalsScored/goalsMissed], value>>
          if (goalsInput.Count != 2) {
               throw new ArgumentException("Game object must have 2 teams.");
          }
          foreach (string teamName in goalsInput.Keys) {
               if (teamName.Length > 15) {
                    throw new ArgumentException("Team name can not be more than 15 characters long.");
               }
          }
          goalsInput.Keys.CopyTo(teams, 0);
          for (int i = 0; i < teams.Length; i++) {
               goals.Add(teams[i], new Dictionary<string, int> {
                    {"goalsScored", goalsInput[teams[i]]},
                    {"goalsMissed", goalsInput[teams[(i+1)%2]]}
               });
          }

          // Adds approporiate amount of points to each team
          foreach (string name in teams) {
               if(goals[name]["goalsScored"] > goals[name]["goalsMissed"]) {
                    points[name] = 2;
                    continue;
               }
               if(goals[name]["goalsScored"] < goals[name]["goalsMissed"]) {
                    points[name] = 0;
                    continue;
               }
               points[name] = 1;
          }
     }

     public static Game SearchByTeams(Game[] games, string team1, string team2) {
          if (team1 == team2) {
               return null;
          }
          for (int i = 0; i < games.Length; i++) {
               if (games[i].teams.Contains(team1) && games[i].teams.Contains(team2)) {
                    return games[i];
               }
          }
          return null;
     }

     public static Dictionary<string, Dictionary<string, int>> GetInfoForTeams(Game[] games) {
          // Return type interpretation: Dictionary<teamName, Dictionary<parameter [goals/points/position], value>>
          // Accepts array of games and returns data about teams who participated in them.

          Dictionary<string, Dictionary<string, int>> output = new Dictionary<string, Dictionary<string, int>>();

          //fill output dictionary with info about each team's total goals and points
          foreach (Game game in games) {
               foreach (string name in game.teams) {
                    if (!output.Keys.Contains(name)) {
                         // initialize new team entry
                         output.Add(name, new Dictionary<string, int> {
                              {"position", 0},
                              {"goalsScored", 0},
                              {"goalsMissed", 0},
                              {"points", 0}
                         });
                    }

                    output[name]["goalsScored"] += game.goals[name]["goalsScored"];
                    output[name]["goalsMissed"] += game.goals[name]["goalsMissed"];
                    output[name]["points"] += game.points[name];
               }
          }

          //make an array of team names sorted from best to worst team
          //implemented by shitty bubble sort
          string[] sortedTeams = output.Keys.ToArray();
          bool sorted = false;
          while(!sorted) {
               sorted = true;
               for (int i = 0; i < sortedTeams.Length; i++) {
                    if (i == sortedTeams.Length-1) {
                         break;
                    }
                    if (output[sortedTeams[i]]["points"] < output[sortedTeams[i+1]]["points"]) { // bad sorting logic
                         string temp = sortedTeams[i];
                         sortedTeams[i] = sortedTeams[i+1];
                         sortedTeams[i+1] = temp;
                         sorted = false;
                         continue;
                    }
                    if (
                         (output[sortedTeams[i]]["points"] == output[sortedTeams[i+1]]["points"]) // FUCKING AWFUL SORTING LOGIC
                         &&
                         (
                         (output[sortedTeams[i]]["goalsScored"]/output[sortedTeams[i]]["goalsMissed"])
                         <
                         (output[sortedTeams[i+1]]["goalsScored"]/output[sortedTeams[i+1]]["goalsMissed"])
                         )
                    ) {
                         string temp = sortedTeams[i];
                         sortedTeams[i] = sortedTeams[i+1];
                         sortedTeams[i+1] = temp;
                         sorted = false;
                    }
               }
          }

          //input leaderboard positions into output dictionary
          for (int i = 0; i < sortedTeams.Length; i++) {
               output[sortedTeams[i]]["position"] = i+1;
          }

          return output;
     }
}