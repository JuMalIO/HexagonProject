using log4net.Config;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace HexagonEvolution
{
    class Program
    {
        static NeatEvolutionAlgorithm<NeatGenome> _ea;
        const string CHAMPION_FILE = @"..\..\..\HexagonApp\bin\Debug\hexagon_champion.xml";

        static void Main(string[] args)
        {
            // Initialise log4net (log to console).
            XmlConfigurator.Configure(new FileInfo("log4net.properties"));

            // Experiment classes encapsulate much of the nuts and bolts of setting up a NEAT search.
            HexagonExperiment experiment = new HexagonExperiment();

            // Load config XML.
            //XmlDocument xmlConfig = new XmlDocument();
            //xmlConfig.Load("hexagon.config.xml");
            //experiment.Initialize("Hexagon", xmlConfig.DocumentElement);
            experiment.Initialize("Hexagon");

            // Create evolution algorithm and attach update event.
            _ea = experiment.CreateEvolutionAlgorithm();
            _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

            // Start algorithm (it will run on a background thread).
            _ea.StartContinue();

            // Hit return to quit.
            Console.ReadLine();
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("gen={0:N0} bestFitness={1:N6}", _ea.CurrentGeneration, _ea.Statistics._maxFitness));

            // Save the best genome to file
            var doc = NeatGenomeXmlIO.SaveComplete(new List<NeatGenome>() { _ea.CurrentChampGenome }, false);
            doc.Save(CHAMPION_FILE);
        }
    }
}
