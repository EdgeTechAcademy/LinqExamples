using System;
using System.Collections.Generic;
using System.Text;

namespace LinqExamples
{
    class Passenger
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public float Age { get; set; }
        public string PassengerClass { get; set; }
        public bool IsPassenger { get; set; }
        public string Role { get; set; }
        public bool IsSurvivor { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} was a {PassengerClass} and " + (IsSurvivor ? "survived" : "did not survive");
        }

        public Passenger(string lastName, string firstName, float age, string paxClass, bool isPassenger, string role, bool survivor)
        {
            this.LastName = lastName;
            this.FirstName = firstName;
            this.Age = age;
            this.PassengerClass = paxClass;
            this.IsPassenger = isPassenger;
            this.Role = role;
            this.IsSurvivor = survivor;
        }

        public static List<Passenger> LoadPassengers()
        {
            String fileName = @"C:\Projects\csv\Titanic.csv";

            // Read each line of the file into a string array. 
            // Each element of the array is one line of the file.
            var lines = new List<string>(System.IO.File.ReadAllLines(fileName));
            //  the first line is a header line, not data. Skip it
            lines.RemoveAt(0);

            List<Passenger> passengers = new List<Passenger>();

            foreach (string line in lines)
            {
                //  the data is comma separated. create an array of the properties from this line
                //  ABBING,Mr Anthony,41,3rd Class,Passenger,,false
                string[] props = line.Split(",");

                //  if the age field is empty set it to "0"
                if (props[2].Length == 0) props[2] = "0";

                //  create a passenger from the properties
                Passenger pax = new Passenger(props[0], props[1], float.Parse(props[2]), props[3], props[4].Equals("Passenger"), props[5], props[6].Equals("true"));

                //  add individual passenger to the list of passengers
                passengers.Add(pax);
            }
            return passengers;
        }
    }
}
