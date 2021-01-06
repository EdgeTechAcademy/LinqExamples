using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LinqExamples
{
    class NBAPlayer
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public decimal Salary { get; set; }
        public string Position { get; set; }
        public float GamesPlayed { get; set; }
        public float Minutes { get; set; }
        public float FieldGoalsMade { get; set; }
        public float FieldGoalsAttempted { get; set; }
        public float ThreePtsMade { get; set; }
        public float ThreePointsAttempted { get; set; }
        public float FreeThrowsMade { get; set; }
        public float FreeThrowAttemped { get; set; }
        public float Turnovers { get; set; }
        public float PersonalFouls { get; set; }
        public float OffensiveRebounds { get; set; }
        public float DefensiveRebounds { get; set; }
        public float Rebounds { get; set; }
        public float Assists { get; set; }
        public float Steals { get; set; }
        public float Blocks { get; set; }
        public float Points { get; set; }

        //  Player	Team    Salary	Position
        //      GamesPlayed	MinutesPerGame	FGMade	FGAttempts	
        //      3PMade	3pAttempts	FTMade	FTAttempts
        //      Turnovers	Fouls	OffensiveRebounds	DefensiveRebounds
        //      Rebounds	Assists	Steals	Blocks
        //      Points
        public NBAPlayer(string name, string team, decimal salary, string position,
                        float gamesPlayed, float minutes, float fieldGoalsMade, float fieldGoalsAttempted,
                        float threePtsMade, float threePointsAttempted, float freeThrowsMade, float freeThrowAttemped,
                        float turnovers, float personalFouls, float offensiveRebounds, float defensiveRebounds,
                        float rebounds, float assists, float steals, float blocks,
                        float points)
        {
            this.Name = name;
            this.Team = team;
            this.Salary = salary;
            this.Position = position;
            this.GamesPlayed = gamesPlayed;
            this.Minutes = minutes;
            this.FieldGoalsMade = fieldGoalsMade;
            this.FieldGoalsAttempted = fieldGoalsAttempted;
            this.ThreePtsMade = threePtsMade;
            this.ThreePointsAttempted = threePointsAttempted;
            this.FreeThrowsMade = freeThrowsMade;
            this.FreeThrowAttemped = freeThrowAttemped;
            this.Turnovers = turnovers;
            this.PersonalFouls = personalFouls;
            this.OffensiveRebounds = offensiveRebounds;
            this.DefensiveRebounds = defensiveRebounds;
            this.Rebounds = rebounds;
            this.Assists = assists;
            this.Steals = steals;
            this.Blocks = blocks;
            this.Points = points;
        }

        public override string ToString()
        {
            return $"{Name} plays for {Team} at {Position} for {Salary:C}";
        }

        /**
         *      LoadRecords 
         *          Reads a csv file
         *          parses the lines on comma
         *          creates an NBAPlayer with the fields
         *          adds the player to the List
         *          returns list to the calling application
         *
         */
        public static List<NBAPlayer> LoadRecords()
        {
            String fileName = @"C:\Projects\csv\Players.csv";   //  the CSV file with all players

            List<NBAPlayer> players = new List<NBAPlayer>();    //  list of all NBA players
            StreamReader textIn = null;                         //  this is file read
            string line = "";
            try
            {
                textIn = new StreamReader(fileName);            //  open the file for reading
                line = textIn.ReadLine();                       //  read and throw away the header line
                while (line != null)                            //  read each line from the file
                {
                    string[] prop = line.Split(",");            //  split the data fields apart

                    //  create the NBA Player
                    NBAPlayer player = new NBAPlayer(prop[1], prop[2], decimal.Parse(prop[3]), prop[4],
                        float.Parse(prop[5]), float.Parse(prop[6]), float.Parse(prop[7]), float.Parse(prop[8]),
                        float.Parse(prop[9]), float.Parse(prop[10]), float.Parse(prop[11]), float.Parse(prop[12]),
                        float.Parse(prop[13]), float.Parse(prop[14]), float.Parse(prop[15]), float.Parse(prop[16]),
                        float.Parse(prop[17]), float.Parse(prop[18]), float.Parse(prop[19]), float.Parse(prop[20]),
                        float.Parse(prop[21]));
                    players.Add(player);                        //  add player to the list of players
                    line = textIn.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Reading file: " + fileName);
                Console.WriteLine("\t" + line);
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                if (textIn != null) textIn.Close();
            }
            return players;
        }
    }
}
