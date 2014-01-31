using System;
using System.Collections.Generic;
using System.Text;
using ParamincsSNMPcontrol;

namespace ParamicsSNMPcontrol
{
    public class ArgumentsInterpreter
    {
        string[] Arguments;
        double
            Sigma,
            Fraction;
        public string ConfigFile;
        string[] JunctionWeightFiles;
        string[] JunctionNodeIDs;

        public ArgumentsInterpreter(string[] Args)
        {
            Arguments = Args;
            ParseStochasticElements();
            ParseConfigFile();
            ParseJuntionInfo();
        }

        private void ParseStochasticElements()
        {
            Sigma = Convert.ToDouble(Arguments[0]);
            Fraction = Convert.ToDouble(Arguments[1]);
        }

        private void ParseConfigFile()
        {
            ConfigFile = Arguments[2];
        }

        private void ParseJuntionInfo()
        {
            JunctionWeightFiles = SplitByColon(Arguments[3]);
            JunctionNodeIDs = SplitByColon(Arguments[4]);
        }

        private string[] SplitByColon(string InString)
        {
            return (InString.Split(';'));
        }

        public Coordinate BulidCoordinator(Strategies ST1, string IP, int port)
        {
            Coordinate Temp = new Coordinate(ConfigFile, ST1, IP, port);
            return (Temp);
        }
        public NeuralNet BuildNeuralNet()
        {
            NeuralNet Temp = new NeuralNet(JunctionWeightFiles, JunctionNodeIDs);
            return (Temp);
        }
        public Logit BuildLogit()
        {
            Logit Temp = new Logit(JunctionWeightFiles, JunctionNodeIDs);
            return (Temp);
        }

    }
}
