using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project1Adams
{
    class Program
    {
        //file location where I store all TSPs
        static string tspDirectory = "../../TSPs/";
        static void Main(string[] args)
        {
            //loop through all files
            //order descending so it goes through most files before hitting Random12.tsp where I run out of memory
            foreach (var file in Directory.GetFiles(tspDirectory).OrderByDescending(x => x))
            {
                var lines = File.ReadAllLines(file);

                //get all lines after the 7th line since that's where coordinates start
                List<Coordinate> coordinates = new List<Coordinate>();
                for (var i = 7; i < lines.Count(); i++)
                {
                    //split out line into coordinate class I created
                    var coordsText = lines[i].Split(' ');
                    coordinates.Add(new Coordinate() { Id = Convert.ToInt32(coordsText[0]), Latitude = Convert.ToDouble(coordsText[1]), Longitude = Convert.ToDouble(coordsText[2]) });
                }

                //get all combos
                var combos = GeneratePermutations(coordinates);

                //preset lowest distance and the list that will be the shortest order
                double lowestDistance = -1;
                List<Coordinate> shortestOrder = new List<Coordinate>();
                foreach (var combo in combos)
                {
                    //calculate distance of this list
                    double distance = 0;
                    for (var i = 0; i < combo.Count - 1; i++)
                    {
                        distance += DistanceBetween(combo[i], combo[i + 1]);
                    }
                    distance += DistanceBetween(combo[combo.Count - 1], combo[0]);

                    //if it's less than the current shortest distance, replace variables
                    if (lowestDistance < 0 || distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        shortestOrder = combo;
                    }
                }

                //print out shortest distance and make the list readable and print out as well
                Console.WriteLine("Shortest distance for " + Path.GetFileName(file) + " is " + Math.Round(lowestDistance, 2).ToString());
                var coordString = "";
                foreach (var coord in shortestOrder)
                {
                    coordString += coord.Id.ToString() + ", ";
                }
                Console.WriteLine("In order of Id's: " + coordString.Substring(0, coordString.Length - 2));
            }
            //pause
            Console.ReadKey();
        }

        //Rather than using a library, I used permutation code that was reworked from http://csharphelper.com/blog/2014/08/generate-all-of-the-permutations-of-a-set-of-objects-in-c/
        private static List<List<Coordinate>> GeneratePermutations(List<Coordinate> items)
        {
            Coordinate[] current_permutation = new Coordinate[items.Count];

            // Make an array to tell whether a coordinate is in the current selection.
            bool[] in_selection = new bool[items.Count];
            List<List<Coordinate>> results = new List<List<Coordinate>>();
            PermuteCoords(items, in_selection, current_permutation, results, 0);

            return results;
        }

        // Recursively permute the coordinates that are not yet in the current selection.
        private static void PermuteCoords(List<Coordinate> coords, bool[] in_selection, Coordinate[] current_permutation, List<List<Coordinate>> results, int next_position)
        {
            // check if done with list
            if (next_position == coords.Count)
                results.Add(current_permutation.ToList());
            else
                // Try options for the next position.
                for (int i = 0; i < coords.Count; i++)
                    if (!in_selection[i])
                    {
                        // Add this coordinate to the current permutation.
                        in_selection[i] = true;
                        current_permutation[next_position] = coords[i];

                        // Recursively fill the remaining positions.
                        PermuteCoords(coords, in_selection, current_permutation, results, next_position + 1);

                        // Remove the coordinate from the current permutation.
                        in_selection[i] = false;
                    }
        }

        //use distance formula to find distance between two points
        private static double DistanceBetween(Coordinate coord1, Coordinate coord2)
        {
            return Math.Sqrt(Math.Pow((coord2.Latitude - coord1.Latitude), 2) + Math.Pow((coord2.Longitude - coord1.Longitude), 2));
        }
    }

    //class to hold id, lat & long
    class Coordinate
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
