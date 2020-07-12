using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeper.Ann
{
    public class BLayer : Layer
    {
        public double[] Error { get; set; }
        public int Units { get; set; }
        public double[][] Weights { get; set; }
        public double[][] WeightSave { get; set; }
        public double[][] dWeight { get; set; }

        public BLayer(int neuronsCount, int inputsCount, int neuronsCount_lower, bool isInput) : base(neuronsCount, inputsCount)
        {
            Units = neuronsCount;
            //neurons = new Neuron[neuronsCount + 1];
            //for (int n = 0; n < neuronsCount + 1; n++)
            //    neurons[n] = new BNeuron(inputsCount);
            Weights = new double[neuronsCount + 1][];
            WeightSave = new double[neuronsCount + 1][];
            dWeight = new double[neuronsCount + 1][];
            output = new double[neuronsCount + 1];
            Error = new double[neuronsCount + 1];
            Output[0] = 1;

            if (!isInput)
            {
                for (int i = 1; i <= Units; i++)
                {
                    Weights[i] = new double[neuronsCount_lower + 1];
                    WeightSave[i] = new double[neuronsCount_lower + 1];
                    dWeight[i] = new double[neuronsCount_lower + 1];
                }
            }
        }

        //public override double[] Compute(double[] input) 
        //{ 
        //    return new double[] { 0.0}; 
        //}

        public override void Randomize() 
        {
            //foreach (Neuron n in Neurons)
            //    n.Randomize();
        }

        public void PropagateLayer(BNet net, BLayer upper)
        {
            double sum;

            for (int i = 1; i <= upper.Units; i++)
            {
                sum = 0;
                for (int j = 0; j <= Units; j++)
                    sum += upper.Weights[i][j] * Output[j];
                upper.Output[i] = 1 / (1 + Math.Exp(-net.Gain * sum));//sigmoid
            }
        }
        public void BackpropagateLayer(BNet net, BLayer lower)
        {
            double Out, Err;

            for (int i = 1; i <= lower.Units; i++)
            {
                Out = lower.Output[i];
                Err = 0;
                for (int j = 1; j <= Units; j++)
                    Err += Weights[j][i] * Error[j];
                lower.Error[i] = net.Gain * Out * (1 - Out) * Err;
            }
        }
    }
}
