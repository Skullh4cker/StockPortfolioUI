using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolioUI
{
    public class Population
    {
        public int generationNumber { get; private set; }
        public double avgFitness { get; private set; }
        public double bestFitness { get; private set; }
        public List<Individual> Individuals { get; set; }

        private int selectionMode = 0;
        private int counter = 0;
        private Random random = new Random();
        private int populationSize = GlobalParametrs.POPULATION_SIZE;
        public Population()
        {
            Individuals = new List<Individual>();
            generationNumber = 1;
        }

        public void InitializePopulation(int mode)
        {
            generationNumber = 1;
            avgFitness = 0;
            bestFitness = 0;
            counter = 0;
            selectionMode = mode;
            populationSize = GlobalParametrs.POPULATION_SIZE;
            Individuals.Clear();
            for (int i = 0; i < populationSize; i++)
            {
                byte Weight1 = GetRandomWeight();
                byte Weight2 = RandomWeightInRange(1, 100 - Weight1);
                byte Weight3 = (byte)(100 - Weight1 - Weight2);
                Individual individual = new Individual(Weight1, Weight2, Weight3);
                Individuals.Add(individual);
            }
        }
        public bool SimulateOneIteration()
        {
            var newPopulation = new List<Individual>();

            var parentPool = new List<Individual>();
            bestFitness = Individuals.Select(x => x.Fitness).Max();
            avgFitness = Individuals.Select(x => x.Fitness).Average();

            int count = GlobalParametrs.PARENTPOOL_SIZE;
            
            switch (selectionMode)
            {
                case 0:
                    parentPool = RouletteSelect(Individuals, count);
                    break;
                case 1:
                    parentPool = TournamentSelect(Individuals, count);
                    break;
            }

            while(newPopulation.Count < populationSize)
            {
                Individual newIndividual = Crossover(parentPool[random.Next(0, count)], parentPool[random.Next(0, count)]);
                newPopulation.Add(newIndividual);
            }

            Individuals = newPopulation;
            generationNumber++;

            double bestGrowth = Individuals.Select(x => x.Fitness).Max() - bestFitness;
            double avgGrowth = Individuals.Select(x => x.Fitness).Average() - avgFitness;
            if (bestGrowth <= 0.01 && bestGrowth >= -0.01) counter++;
            else counter = 0;

            if (counter > GlobalParametrs.ITERTIONS_UNCHANGED) return true;
            else return false;
        }
        public Individual DoubleCrossover(Individual parent1, Individual parent2)
        {
            int crossoverPoint1 = random.Next(1, 12);
            int crossoverPoint2 = random.Next(crossoverPoint1 + 1, 23);

            string parent1Chromosome = parent1.Chromosome.Genes;
            string parent2Chromosome = parent2.Chromosome.Genes;

            string childChromosome = parent1Chromosome.Substring(0, crossoverPoint1) +
                                     parent2Chromosome.Substring(crossoverPoint1, crossoverPoint2 - crossoverPoint1) +
                                     parent1Chromosome.Substring(crossoverPoint2);
            childChromosome = MutateGenes(childChromosome);

            byte weigth1 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(0, 8));
            byte weigth2 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(8, 8));
            byte weigth3 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(16));

            AdjustValues(ref weigth1, ref weigth2, ref weigth3);

            return new Individual(weigth1, weigth2, weigth3);
        }
        public Individual Crossover(Individual parent1, Individual parent2)
        {
            int crossoverPoint1 = random.Next(1, 23);

            string parent1Chromosome = parent1.Chromosome.Genes;
            string parent2Chromosome = parent2.Chromosome.Genes;

            string childChromosome = parent1Chromosome.Substring(0, crossoverPoint1) +
                                     parent2Chromosome.Substring(crossoverPoint1);
            childChromosome = MutateGenes(childChromosome);

            byte weigth1 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(0, 8));
            byte weigth2 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(8, 8));
            byte weigth3 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(16));

            AdjustValues(ref weigth1, ref weigth2, ref weigth3);

            return new Individual(weigth1, weigth2, weigth3);
        }
        private void AdjustValues(ref byte value1, ref byte value2, ref byte value3)
        {
            float sum = value1 + value2 + value3;
            value1 = Convert.ToByte((value1 * 100) / sum);
            value2 = Convert.ToByte((value2 * 100) / sum);
            value3 = Convert.ToByte((value3 * 100) / sum);
            if (value1 == 0 || value2 == 0 || value3 == 0)
            {
                byte maxValue = Math.Max(value1, Math.Max(value2, value3));
                if (value1 == 0)
                    value1++;
                else if (value2 == 0)
                    value2++;
                else if (value3 == 0)
                    value3++;

                if (maxValue == value1)
                    value1 -= 1;
                else if (maxValue == value2)
                    value2 -= 1;
                else
                    value3 -= 1;
            }
            byte difference = (byte)(100 - (value1 + value2 + value3));
            if (sum == 100) return;
            if (difference != 0)
            {
                byte maxValue = Math.Max(value1, Math.Max(value2, value3));

                if (maxValue == value1)
                {
                    value1 += difference;
                }
                else if (maxValue == value2)
                {
                    value2 += difference;
                }
                else
                {
                    value3 += difference;
                }
            }
            if (value1 == 99 || value2 == 99 || value3 == 99)
            {
                byte minValue = Math.Min(value1, Math.Min(value2, value3));
                if (value1 == 99)
                    value1--;
                else if (value2 == 99)
                    value2--;
                else if (value3 == 99)
                    value3--;

                if (minValue == value1)
                    value1 += 1;
                else if (minValue == value2)
                    value2 += 1;
                else
                    value3 += 1;
            }
        }
        private string MutateGenes(string genes)
        {
            char[] chromosome = genes.ToCharArray();
            for (int i = 0; i < chromosome.Length; i++)
            {
                if (random.NextDouble() <= GlobalParametrs.MUTATION_CHANCE)
                {
                    chromosome[i] = (chromosome[i] == '0') ? '1' : '0';
                }
            }

            return new string(chromosome);
        }
        private byte GetRandomWeight()
        {
            return (byte)random.Next(1, 98);
        }
        private byte RandomWeightInRange(int min, int max)
        {
            return (byte)random.Next(min, max);
        }
        public List<Individual> TournamentSelect(List<Individual> individuals, int count)
        {
            List<Individual> individualsForSelection = new List<Individual>(individuals);
            List<Individual> selectedParents = new List<Individual>();

            while (selectedParents.Count < count)
            {
                Individual participant1 = individualsForSelection[random.Next(individualsForSelection.Count)];
                Individual participant2 = individualsForSelection[random.Next(individualsForSelection.Count)];

                Individual winner = (participant1.Fitness > participant2.Fitness) ? participant1 : participant2;

                selectedParents.Add(winner);
            }

            return selectedParents;
        }
        public List<Individual> RouletteSelect(List<Individual> individuals, int count)
        {
            List<Individual> individualsForSelection = new List<Individual>(individuals);
            individualsForSelection = individualsForSelection.OrderByDescending(x => x.Fitness).ToList();
            List<Individual> selectedParents = new List<Individual>();
            double cumulativeProbability = 0;
            while (selectedParents.Count < count)
            {
                List<double> ratios = GetRatios(individualsForSelection);
                int index = RouletteWheelSelection(ratios.ToArray(), ref cumulativeProbability);
                selectedParents.Add(individualsForSelection[index]);
                individualsForSelection.RemoveAt(index);
            }
            return selectedParents;
        }
        public int RouletteWheelSelection(double[] probabilities, ref double cumulativeProbability)
        {
            double randomValue = random.NextDouble();       

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue <= cumulativeProbability)
                {
                    return i;
                }
            }

            return probabilities.Length - 1;
        }
        private List<double> GetRatios(List<Individual> individuals)
        {
            List<double> ratios = new List<double>();
            double totalFitness = individuals.Select(x => x.Fitness).Sum();
            foreach (var individual in individuals)
            {
                ratios.Add(individual.Fitness / totalFitness);
            }
            return ratios;
        }
    }
}
