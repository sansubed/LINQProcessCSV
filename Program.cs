using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
namespace Cars
{
    public class Program
    {
        static void Main(string[] args)
        {
            //anonymous type
            //var keyword is used, no name that I can use to define a variable of that type
            //can create anonymous typed object using the 'new; keyword and then the curly braces
            var anonymous = new
            {
                Name = "Santona"
            };


            var cars = ProcessFile("fuel.csv");
            //then by is used for the secondary sorting 
            //here the lambda exp is king, it helps to pull one string from the list of car and helps you perform desired task!
            //here converted the manufacturer's name to lowercase in the Car Class

            //using the method syntax
            var query = cars.Where(car => car.Manufacturer == "bmw" && car.Year >= 2016)
                            .OrderByDescending(c => c.Combined)
                            .ThenBy(c => c.CName)
                            .Select(c => c); // this line is optional
                                             //.First(); //must immediately execute the query to produce the concrete result. 
                                             //Using extension and the query syntax to print the values
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine("Using extension method syntax meaning using the method syntax, you have to use Orderby, OrderbyDescending");
            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.CName} {car.Combined} {car.Manufacturer}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");


            //Using the query syntax
            var alternateToquery = from car in cars
                                   where car.Manufacturer == "bmw" && car.Year >= 2016
                                   orderby car.Combined descending, car.CName ascending
                                   select new
                                   {
                                       car.Manufacturer,
                                       car.CName,
                                       car.Combined

                                   };

            Console.WriteLine("Using query method, you only 'orderby' and then use the keywords: ascending and descending ");
            foreach (var car in alternateToquery.Take(10))
            {
                Console.WriteLine($"{car.CName} {car.Manufacturer} {car.Combined}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");

            //this is to check if the default value exists
            var defaultFirst = cars
                                   .OrderByDescending(c => c.Combined)
                                   .ThenBy(c => c.CName)
                                   .Select(c => c)
                                   .FirstOrDefault(car => car.Manufacturer == "aaa" && car.Year == 2016);
            if (defaultFirst == null)
            {
                Console.WriteLine("No value present for defaultFirst with Manufacturer name 'aaa' and Year= 2020 ");
            }
            else
                Console.WriteLine(defaultFirst.CName);

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");


            //demonstarting Any here, returns bool Func<T, bool>

            var anyOperator = cars.Any(car => car.Manufacturer == "ford");

            /* Alternative approach to create anonymous type
            var anyOperatorAlternate = cars.Select(c => new
            {
                c.Manufacturer, c.Name, c.Combined

            });
            */



            Console.WriteLine($"This any operator: Is there any manufacturer named 'ford' = {anyOperator}");
            //Console.WriteLine( anyOperator);

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");

            var allOperator = cars.All(car => car.Manufacturer == "ford");
            Console.WriteLine($"This all operator: Is all manufacturer named 'ford' = {allOperator}");

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");

            var selectUsage = from car in cars
                              where car.Manufacturer == "toyota" && car.Year >= 2000
                              orderby car.Combined descending, car.CName ascending
                              //creating anonymous 
                              select new
                              {
                                  car.Manufacturer,
                                  car.CName,
                                  car.Combined

                              };

            Console.WriteLine("Demonstrating Select and Anonymous (we only have car Manufacturer, Name and the Combined)");
            foreach (var car in selectUsage.Take(5))
            {
                Console.WriteLine($"{car.Manufacturer}: {car.CName} ");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");


            //usage of Select to print each character
            Console.WriteLine("Using select (iterating over collection/ array/strings) & nested 'foreach' loop to print characters in the Manufacturers: ");
            var selectEachChar = cars.Select(car => car.Manufacturer);
            foreach (var name in selectEachChar.Skip(1204)) //goes to line 1205 i.e volvo
            {
                foreach (var character in name.Take(5)) //only taking volv
                {
                    Console.WriteLine(character);
                }
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");

            Console.WriteLine("Alternative way to print the characters in a string by using SelectMany (iterating in collection of collection, eg: charcaters in string):\n");

            var selectManyUsage = cars.SelectMany(car => car.Manufacturer).OrderBy(c => c);
            foreach (var manufacturer in selectManyUsage.Skip(1000).Take(2))//??? take is taking the first 3 characters and skip is skipping 1 off it. eg hom, ho
            {
                Console.WriteLine(manufacturer);
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------\n");
            
            //After the Manufacturer.csv is added

            var manufacturers = ProcessManufacturers("manufacturers.csv");
            //here they want to see the headquarter which is the data in the Manufacturer.csv which car.csv doesn't have so join
           
            //this query is production IEnumerable of an anonymous type that has a string property 
            var carManuCombined = from car in cars  //datasource => cars and variable is car
                                  join manufacturer in manufacturers on car.Manufacturer equals manufacturer.MName
                                  orderby car.Combined descending, car.CName ascending
                                  select new
                                  {
                                      manufacturer.Headquarters,
                                      car.CName,
                                      car.Combined
                                  };
           

            //this car variable inside loop has no relationship with the car variable inside the LINQ query
            foreach (var car in carManuCombined.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.CName} {car.Combined}");
               
            }

            Console.WriteLine("Hello");

            var alternateTocarManu = cars.Join(manufacturers,
                                                c => c.Manufacturer,
                                                m => m.MName,
                                                (c, m) => new
                                                {
                                                    m.Headquarters,
                                                    c.CName,
                                                    c.Combined

                                                })
                                         .OrderByDescending(c => c.Combined)
                                          .ThenBy(c => c.CName);
             
            foreach(var car in alternateTocarManu.Take(5))
            {
                Console.WriteLine($"{car.CName}");
            }
           

        }


        /*File.ReadAllLines(path)
                .Skip(1) // this skips the first line and if you want to skip 10 line and take 40, use Take(40)
                .Where(line => line.Length > 1)
                .Select(Car.ParseFromCsv).ToList();
         * */
        //this is basically a function
        private static List<Car> ProcessFile(string path)
        {
            var query =
                 from line in File.ReadAllLines(path).Skip(1)
                 where line.Length > 1
                 select Car.ParseFromCsv(line);

            var query2 = File.ReadLines(path)
                             .Skip(1)
                             .Where(l => l.Length > 1)
                             .Select(l => Car.ParseFromCsv(l));

            //.ToCar();

            return query2.ToList();


        }
        //To read the manufacturer.csv file

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                            .Where(line => line.Length > 1) //empty line at the end
                            .Select(line =>
                            {
                                var columns = line.Split(',');
                                return new Manufacturer
                                {
                                    MName = columns[0],
                                    Headquarters = columns[1],
                                    Year = int.Parse(columns[2])
                                };
                            });
            return query.ToList();
        }


    }
}

/*This is used if you would like to use this method instead of select in the query above. 
 * Also you have to remove the function from the Car class here
 * 
 * public static class CarExtensions
 {
     public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
     {
         foreach (var line in source)
         {
             var columns = line.Split(',');

            yield return new Car
             {
                 Year = int.Parse(columns[0]),
                 Manufacturer = columns[1].ToLower(),
                 Name = columns[2],
                 Displacement = double.Parse(columns[3]),
                 Cylinders = int.Parse(columns[4]),
                 City = int.Parse(columns[5]),
                 Highway = int.Parse(columns[6]),
                 Combined = int.Parse(columns[7])

             };


         }
     }

 }
*/

