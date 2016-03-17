using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAF;
using GAF.Extensions;
using GAF.Operators;
namespace GezginSatıcıProblemi
{
    class Program
    {
         private static List<City> _cities;

        private static void Main(string[] args)
        {

          
            //get our cities
            _cities = CreateCities().ToList();

            //Each city can be identified by an integer within the range 0-15
            //our chromosome is a special case as it needs to contain each city 
            //only once. Therefore, our chromosome will contain all the integers
            //between 0 and 15 with no duplicates.

            //We can create an empty population as we will be creating the 
            //initial solutions manually.
            var population = new Population();
            
            //create the chromosomes
            for (var p = 0; p < 100; p++)
            {

                var chromosome = new Chromosome();
                for (var g = 0; g < 16; g++)
                {
                    chromosome.Genes.Add(new Gene(g));
                }
                chromosome.Genes.ShuffleFast();
                population.Solutions.Add(chromosome);
            }

            //create the elite operator
            var elite = new Elite(5);
            
            //create the crossover operator
            var crossover = new Crossover(0.8)
                {
                    CrossoverType = CrossoverType.DoublePointOrdered
                };

            //create the mutation operator
            var mutate = new SwapMutate(0.02);

            //create the GA
            var ga = new GeneticAlgorithm(population, CalculateFitness);

            //hook up to some useful events
            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;
            
            //add the operators
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutate);
            
            //run the GA
            ga.Run(Terminate);
            Console.ReadLine();
        }

        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            foreach (var gene in fittest.Genes)
            {
                Console.WriteLine(_cities[(int)gene.RealValue].Name);
            }
        }

        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            var distanceToTravel = CalculateDistance(fittest);
            Console.WriteLine("Generation: {0}, Fitness: {1}, Distance: {2}", e.Generation, fittest.Fitness, distanceToTravel);                
        }

        private static IEnumerable<City> CreateCities()
        {
            var cities = new List<City>
            {
                new City("Rize",40.3144, 41.020507),
                new City("Trabzon", 39.430852, 41.008139),
                new City("Giresun", 38.242161, 40.556215),
                new City("Ordu", 37.533895, 41.009931),
                new City("Samsun", 36.208239, 41.178549),
                new City("Sinop", 35.099532, 42.010126),
                new City("Ağrı", 43.031959, 39.445941),
                new City("İstanbul", 28.580766, 41.015849),
                new City("İzmir", 27.092406, 38.253222),
                new City("Muğla", 38.228426, 37.186893),
                new City("Kocaeli", 29.552982, 40.463554),
                new City("Sivas", 37.028224, 39.456893),
                new City("Amasya", 35.514837, 40.39495),
                new City("Erzurum", 41.176024, 39.553949),
                new City("Elazığ", 19.142266, 38.415556),
                new City("Diyarbakır", 40.149406, 37.555802)
            };

            return cities;
        }

        public static double CalculateFitness(Chromosome chromosome)
        {
            var distanceToTravel = CalculateDistance(chromosome);
            return 1 - distanceToTravel / 10000;
        }
    
        private static double CalculateDistance(Chromosome chromosome)
        {
            var distanceToTravel = 0.0;
            City previousCity = null;

            //run through each city in the order specified in the chromosome
            foreach (var gene in chromosome.Genes)
            {
                var currentCity = _cities[(int) gene.RealValue];

                if (previousCity != null)
                {
                    var distance = previousCity.GetDistanceFromPosition(currentCity.Latitude,
                                                                        currentCity.Longitude);

                    distanceToTravel += distance;
                }

                previousCity = currentCity;
            }

            return distanceToTravel;
        }

        public static bool Terminate(Population population, 
            int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 500;
        }
    }
}
