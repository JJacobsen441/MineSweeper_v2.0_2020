using Accord.Math.Random;
using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeper.Ann
{
    //public class BNeuron : Neuron
    //{
    //    //public double[] Weights { get { return weights; } set { weights = value; } }// 0=Weight, 1= WeightSave, 2=dWeight
    //    //public IRandomNumberGenerator<double> RandGenerator { get; set; }

    //    public BNeuron(int inputs) : base(inputs)
    //    {
    //        weights = new double[3];
    //    }

    //    public override double Compute(double[] input)
    //    {
    //        return 0.0;
    //    }

    //    public override void Randomize() 
    //    {
    //        //for (int w = 0; w < Weights.Length; w++)
    //            weights[0] = GetRandomDouble(-0.5, 0.5);
    //    }

    //    public double GetRandomDouble(double minimum, double maximum)
    //    {
    //        Random random = new Random();
    //        return random.NextDouble() * (maximum - minimum) + minimum;
    //    }
    //}
}
