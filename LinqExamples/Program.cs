using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            LINQExamples();
            TitanicExamples();
        }

        private static void TitanicExamples()
        {
            List<Passenger> passengers = Passenger.LoadPassengers();

            //  How many people were on the Titanic?
            Console.WriteLine($"Titanic passenger/crew count: {passengers.Count()}");

            //  we are going to use several High Order Functions
            //      FindAll - return EVERY attribute from a JSON array but only SOME of the rows
            //      MAP    - return a row for EVERY item in the JSON array but only SOME of the attributes
            //      REDUCE - return a ONE piece of information for the JSON array (total of a col, longest/shortest string)

            //------------------------------------------------------------------------------------------------
            //
            //  let"s start off with FindAll
            //
            //------------------------------------------------------------------------------------------------
            //      a list for survivors
            List<Passenger> survivors = passengers.FindAll(p => p.IsSurvivor);

            //  count is a TERMINAL operation which means we can no longer use this stream
            Console.WriteLine($"Titanic Survivors count: {survivors.Count()}");

            var crewMembers = passengers.FindAll(p => !p.IsPassenger);
            //  count is a TERMINAL operation which means we can no longer use this stream
            Console.WriteLine($"Titanic Crew count: %d%n", crewMembers.Count());

            //  let's get a list of surviving crew members in three different ways (NOT in Java we wont)
            //      ATTENTION we can not reuse the survivor or crewMembers list again.
            //          The count() method is a 'terminal' method that closes the stream
            //          Java does not permit the reuse of a stream once it has been clased
            //          JavaScript does not have this proscription
            var survivingCrew = passengers.FindAll(p => p.IsSurvivor && !p.IsPassenger);
            Console.WriteLine($"Number of the crew that survived: {survivingCrew.Count()}");

            //  one more FindAll request - get the number of children (under 18), that survived from 1st class
            var youngSurvivors = passengers.FindAll(p => p.Age > 0 && p.Age < 18 &&
                                                       p.PassengerClass.Equals("1st Class") && p.IsSurvivor);
            youngSurvivors.ForEach(p => Console.WriteLine(p));
            Console.WriteLine(youngSurvivors.Count());

            //------------------------------------------------------------------------------------------------
            //
            //  Moving on the MAP
            //      map will extract a one attribute of your object or multiple if you like
            //
            //------------------------------------------------------------------------------------------------
            //  get all first names. Notice the length of the array matches the length of the passengers array
            var allFirstNames = from p in passengers
                                select p.FirstName;
            Console.WriteLine($"We have a list of {allFirstNames.Count()} first names");

            //  get all titles
            //          using Set and map. map returns ALL of the titles, Set condenses down to the unique titles
            //          Set is a Java object that only holds UNIQUE values. All dups are tossed away
            //      step 1 - get the firstName. The first name attribute looks like this              Mrs Rhoda Mary 'Rosa'
            var firstNames = from p in passengers
                             select p.FirstName;
            //      step 2 - SPLIT the first name on space. This creates an array of the names       ['Mrs', 'Rhoda', 'Mary', '\'Rosa\'']
            //      step 3 - get the first token of the array [0]                                     'Mrs'
            var allTitles = from name in firstNames
                            select name.Split(" ")[0];                       //  this still has 2477 entries in it
            //      step 4 - create a new list of titles for just the unique values
            var titles = allTitles.Distinct();                  //  this is a stream NOT a Set
            Console.WriteLine($"What are the titles of the people on the Titanic? {string.Join(",", titles)}");

            //      Let's do it all in a single step. We wrap the LINQ statement with parens and treat in like a List
            titles = (from name in firstNames
                      select name.Split(" ")[0])
                     .Distinct();
            Console.WriteLine($"What are the titles of the people on the Titanic? {string.Join(",", titles)}");

            //------------------------------------------------------------------------------------------------
            //
            //  REDUCE
            //      Reduce is great for getting the total of an array or the longest string or largest value
            //
            //------------------------------------------------------------------------------------------------
            //  get total ages of all passengers and crew members
            //      method 1 - mapToDouble will produce a stream of age values and sum will do what it does
            double totalAges = passengers.Sum(p => p.Age);        //  there is also .average, .min, and .max
            Console.WriteLine($"Total of all ages {totalAges} and the Average {totalAges / passengers.Count()}");

            //
            //  what is the average age of the survivors?
            var ageOfSurvivors = passengers.FindAll(p => p.IsSurvivor).Average(p => p.Age);
            Console.WriteLine($"Average age of survivors {ageOfSurvivors}");

            // 3rd class passengers over 60 that survived
            var thirdClass = passengers.FindAll(p => p.IsSurvivor && (p.Age > 60) && p.PassengerClass.Equals("3rd Class")).Count();
            Console.WriteLine($"3rd class passengers over 60 that survived: {thirdClass}");

            //  find the captain
            //      hint firstName starts with Capt (and they were an officer)
            //      Surprise there were two Captains on board. One was a passenger
            var captains = passengers.FindAll(p => p.FirstName.StartsWith("Capt"));  //   && ! p.IsPassenger
            captains.ForEach(c => Console.WriteLine(c));

            // find the Musicians
            var musicians = passengers.FindAll(p => p.Role.Equals("Musician"));
            musicians.ForEach(m => Console.WriteLine(m));

            //  get list of married women
            var ladies = passengers.FindAll(p => p.FirstName.StartsWith("Mrs"));
            Console.WriteLine($"Number of Mrs's on board {ladies.Count()}");

            //-------------------------------------------------------------------------------------
            //
            //      Let's look at some of other array functions available to us
            //          Any     returns true or false if at least one record matches the conditional expression
            //          All     returns TRUE if EVERY record matches the conditional expression
            //          Find    returns the first record
            //          sort    sort the records in the array using the test 
            //                  (compare two records return <0 if record A is smaller >0 if A is bigger, 0 is the same)
            //-------------------------------------------------------------------------------------
            var oldSurvivors = passengers.Any(p => p.IsSurvivor && p.Age > 80);            //  did some octogenerians survice
            var allMusiciansDead = passengers.All(p => p.Role.Equals("Musician") && !p.IsSurvivor);  //  did every musician die?
            var first30Plus = passengers.FindAll(p => p.Age > 30 && p.IsSurvivor).First();       //  find first 30 something survivor

            //  Who was the youngest passenger? This is a little tricky.
            //      We have some missing age data so if we do not have the age of the passenger it has been set to 0;
            //      which we would not want to use as a valid lowest age. So we add a compare to 0 and skip that record
            var youngest = (from p in passengers
                            orderby p.Age
                            where p.Age > 0
                            select p)
                            .First();
            var oldest = (from p in passengers
                          orderby p.Age           //  or we could orderby descending and use First()
                          select p)
                            .Last();

            Console.WriteLine($"The youngest passenger on the Titanic was {youngest.FirstName} {youngest.LastName} age: {youngest.Age}");
            Console.WriteLine($"The oldest   passenger on the Titanic was {oldest.FirstName} {oldest.LastName} age: {oldest.Age}");

            //  alphabetize names by last name
            musicians = passengers.FindAll(p => p.Role.Equals("Musician"));
            var sorted = musicians.OrderBy(p => p.LastName);
            musicians.ForEach(m => Console.WriteLine(m));

            //  order names by length and find the shortest name
            //  version 1 - create a sorted stream based on length of last name
            var orderedNames = passengers.OrderBy(p => p.LastName.Length);

            //  order names by length and find the longest name
            var shortestName = orderedNames.First();
            var longestName = orderedNames.Last();

            Console.WriteLine($"The shortest last name is: {shortestName.LastName}");
            Console.WriteLine($"The lonest   last name is: {longestName.LastName}");

            List<Passenger> pax;
            HashSet<String> roles;

            //  get a List of Musicians, and by List I mean a true C# List
            pax = passengers.FindAll(p => p.Role.Equals("Musician")).ToList();

            //  get a unique Set of roles
            roles = passengers.Select(p => p.Role).ToHashSet();

            // Let"s see how we can group objects
            var classGroup = passengers.GroupBy(p => p.PassengerClass);
            classGroup.ToList().ForEach(g => Console.WriteLine($"Class Groups : {g.Key} #: {g.Count()}"));
            var personByClass = from p in passengers
                                group p by p.PassengerClass;
            personByClass.ToList().ForEach(p => Console.WriteLine($"Class: {p.Key} #: {p.Count()}"));

            var ageGroup = passengers.GroupBy(p => p.Age);
            ageGroup.ToList().ForEach(g => Console.WriteLine($"Age Groups : {g.Key} #: {g.Count()}"));
            var personByAge = from p in passengers
                              group p by p.Age;
            personByAge.ToList().ForEach(g => Console.WriteLine($"Age: {g.Key} #: {g.Count()}"));

            //  now just for some simple array functions with a simple array of numbers
            int[] ages = { 14, 17, 11, 32, 33, 16, 40, 15, 4, 18, 912, 543, 33 };

            Console.WriteLine($"reduce -    {ages.Aggregate(0, (tot, num) => tot - num)}");         //  subtract the numbers from 0
            Console.WriteLine($"reduce +    {ages.Aggregate(0, (tot, num) => tot + num)}");         //  add numbers together starting at 0
            Console.WriteLine($"every       {ages.All(age => age >= 18)}");                         //  is every number >= to 18
            Console.WriteLine($"includes    {ages.Any(age => age >= 18)}");                         //  does the array include any numbers >= 18?
            Console.WriteLine($"FindAll     {ages.Where(age => age >= 100).Count()}");              //  get array of numbers > 100
            Console.WriteLine($"find        {ages.Where(age => age >= 18).First()}");               //  find the first 18 in the array
            Console.WriteLine($"includes    {ages.Contains(16)}");                                  //  look for 16
            Console.WriteLine($"some        {ages.Any(age => age >= 18)}");                           //  are some of the numbers >= 18
            Console.WriteLine($"indexOf     {Array.IndexOf(ages, 33)}");                            //  where is the first 33
            Console.WriteLine($"lastIndexOf {Array.LastIndexOf(ages, 33)}");                        //  where is the last 33
            Console.WriteLine($"filter      {ages.Where(age => age >= 100).Count()}");              //  get array of numbers > 100
        }

        static void LINQExamples()
        {
            List<Passenger> paxList = Passenger.LoadPassengers();

            List<Passenger> survivor = paxList.FindAll(p => p.IsSurvivor);
            HashSet<string> classes = new HashSet<string>();
            HashSet<string> roles = new HashSet<string>();
            foreach (var pax in paxList)
            {
                classes.Add(pax.PassengerClass);
                roles.Add(pax.Role);
            }
            foreach (var role in roles)
            {
                Console.WriteLine(role);
            }

            /*      
                               LINQ and Collection methods

            List<Passenger> passengers = Passenger.LoadPassengers();
            var survivors = passengers.FindAll(p => p.Survivor);
            https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.add?view=netcore-3.1
                Add         AddRange    Clear       Contains    ConvertAll  Exists
                Find        FindAll     FindIndex   FindLast    FindLastIndex
                ForEach     GetRange    IndexOf     Insert      InsertRange
                LastIndexOf Remove      RemoveAll   RemoveAt    RemoveRange
                Reverse     Sort        ToArray     TrimExcess  TrueForAll

                https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any?view=netcore-3.1
            */
            int[] nums = { 3, 4, 2, 4, 5, 6, 32, 9, 4, 6, 76, 8, 432, 4, 2, 5 };
            int[] sub = { 32, 5, 6, 2 };

            int above10 = nums.Where(n => n > 10).Sum();

            var any = nums.Any(n => n > 5);
            var all = nums.All(n => n > 0);
            var average = nums.Average();
            var contains = nums.Contains(9);
            var count = nums.Count();
            var distinct = nums.Distinct();
            var elementAt = nums.ElementAt(6);
            var except = nums.Except(sub);
            var first = nums.First();
            var last = nums.Last();
            var max = nums.Max();
            var min = nums.Min();
            var orderBy = nums.OrderBy(n => n);
            var prepend = nums.Prepend(-1);
            var range = Enumerable.Range(-5, 5);
            var repeat = Enumerable.Repeat("Beetlejuice", 3);
            var reverse = nums.Reverse();
            //            var  single         = nums.Singlen => n > 32);
            var skip = nums.Skip(5);
            var skipLast = nums.SkipLast(5);
            var skipWhile = nums.SkipWhile(n => n != 9);
            var sum = nums.Sum();
            var take = nums.Take(5);
            var takeLast = nums.TakeLast(5);
            var takeWhile = nums.TakeWhile(n => n < 32);
            var where = nums.Where(n => n % 2 == 1);

            var toArray = nums.OrderBy(n => n).ToArray();
            var toHashSet = nums.OrderBy(n => n).ToHashSet();
            var toList = where.OrderBy(n => n).ToList();

            //List<NBAPlayer> players = NBAPlayer.LoadRecords();
            //var portland = players.Where(p => p.Team == "POR");
            //foreach (var player in portland)
            //{
            //    Console.WriteLine(player);
            //}
        }
        static void NumberExamples()
        {
            int[] numbers = { 7, 6, 3, 3, 6, 76, 3, 6, 8, 6, 3, 243, 5, 7 };
            //  Convert this string into an array of words
            string line = "Convert this string into an array of words";
            string[] arr = line.Split(" ");

            //Find the largest number and the largest word(alphabetically A comes before M comes before Z)
            int max = numbers.Max();
            string strMax = arr.Max();

            //Do any words end with g
            bool endsWithG = arr.Any(s => s.EndsWith("g"));

            //Are any numbers bigger than 20
            bool biggerThan20 = numbers.Any(n => n > 20);

            //What is the sum of all the numbers
            int total = numbers.Sum();

            //What is the sum of the numbers less than 50 ?
            total = numbers.Where(n => n > 50).Sum();

            //What is the middle number ?
            int first = numbers.ElementAt(0);
            int middle = numbers.ElementAt(numbers.Length / 2);

            //What is the middle word ?
            string middleWord = arr.ElementAt(arr.Length / 2);

            //Create a List of the first 4 numbers
            var first4 = numbers.Take(4);
            //Skip the first 3 words and create a list of the next 3 words
            var middle3Words = arr.Skip(3).Take(3);
            //What is the average of the numbers ?
            double ave = numbers.Average();

            int[] nums = { 3, 4, 2, 4, 5, 6, 432, 9, 4, 6, 76, 8, 432, 4, 2, 5 };
            int[] sub = { 32, 5, 6, 2 };

            var newList = nums.OrderBy(n => n).TakeLast(2).First();

            int[] list2 = new int[1];
            list2[0] = nums.Max();
            var exceptBiggest = nums.Except(list2).Max();

            var big = nums.Where(n => n > 10);
            var unique = nums.Distinct();
            int above10 = nums.Where(n => n > 10).Sum();
            //  create a list of numbers less than 16
            var below16 = nums.Where(n => n < 16);

            //  what is the average of that list
            ave = below16.Average();

            //  are there any odd numbers 
            bool anyOdds = below16.Any(n => n % 2 == 1);
            Console.WriteLine();

            var any = nums.Any(n => n > 5);
            var all = nums.All(n => n > 0);
            var average = nums.Average();
            var contains = nums.Contains(9);
            var count = nums.Count();
            var distinct = nums.Distinct();
            var elementAt = nums.ElementAt(6);
            var except = nums.Except(sub);
            first = nums.First();
            var last = nums.Last();
            max = nums.Max();
            var min = nums.Min();
            var orderBy = nums.OrderBy(n => n);
            var prepend = nums.Prepend(-1);
            var range = Enumerable.Range(-5, 5);
            var repeat = Enumerable.Repeat("Beetlejuice", 3);
            var reverse = nums.Reverse();
            var skip = nums.Skip(5);
            var skipLast = nums.SkipLast(5);
            var skipWhile = nums.SkipWhile(n => n != 9);
            var sum = nums.Sum();
            var take = nums.Take(5);
            var takeLast = nums.TakeLast(5);
            var takeWhile = nums.TakeWhile(n => n < 32);
            var where = nums.Where(n => n % 2 == 1);
        }
    }
}
