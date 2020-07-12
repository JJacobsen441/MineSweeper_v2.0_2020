using Accord.Neuro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MineSweeper.Ann
{
    public class BNet : Network
    {
        private BLayer in_layer { get; set; }
        private BLayer out_layer { get; set; }
        public double Gain { get; set; }
        private double Error { get; set; }
        private double Eta { get; set; }
        private double Alpha { get; set; }
        private int[] Neu { get; set; }

        //REAL Mean;
        public double TrainError { get; set; }
        public double TestError { get; set; }


        public BNet(int[] neurons, double gain, double eta, double alpha/*, int inputsCount, int layersCount*/) : base(neurons[0], neurons.Length)
        {
            this.inputsCount = neurons[0];
            this.layersCount = neurons.Length;

            this.Gain = gain;
            this.Eta = eta;
            this.Alpha = alpha;
            this.Neu = neurons;

            layers[0] = new BLayer(neurons[0], neurons[0], -1, true);
            for (int l = 1; l < layersCount; l++)
                layers[l] = new BLayer(neurons[l], neurons[l], neurons[l - 1], false);

            in_layer = (BLayer)layers[0];
            out_layer = (BLayer)layers[layersCount - 1];
        }

        public override double[] Compute(double[] input) 
        {
            return new double[] { 0.0 };
        }

        private void ComputeOutputError(double[] Target) 
        {
            double Out, Err;

            Error = 0;
            for (int i = 1; i <= ((BLayer)out_layer).Units; i++)
            {
                Out = out_layer.Output[i];
                Err = Target[i - 1] - Out;
                out_layer.Error[i] = Gain * Out * (1 - Out) * Err;
                Error += 0.5 * Math.Sqrt(Err);
            }
        }

        public override void Randomize()
        {
            for (int l = 1; l < layersCount; l++)
            {
                for (int i = 1; i <= ((BLayer)Layers[l]).Units; i++)
                {
                    for (int j = 0; j <= ((BLayer)Layers[l - 1]).Units; j++)
                        ((BLayer)Layers[l]).Weights[i][j] = GetRandomDouble(-0.5, 0.5);
                }
            }
        }

        private void SetInput(double[] input)
        {
            for (int i = 1; i <= ((BLayer)in_layer).Units; i++)
                in_layer.Output[i] = input[i - 1];
        }

        private void GetOutput(out double[] output)
        {
            output = new double[((BLayer)out_layer).Units];
            for (int i = 1; i <= ((BLayer)out_layer).Units; i++)
                output[i -1] = out_layer.Output[i];
        }

        public void SaveWeights()
        {
            for (int l = 1; l < layersCount; l++)
            {
                for (int i = 1; i <= ((BLayer)Layers[l]).Units; i++)
                {
                    for (int j = 0; j <= ((BLayer)Layers[l - 1]).Units; j++)
                        ((BLayer)Layers[l]).WeightSave[i][j] = ((BLayer)Layers[l]).Weights[i][j];
                }
            }
        }

        public void RestoreWeights()
        {
            for (int l = 1; l < layersCount; l++)
            {
                for (int i = 1; i <= ((BLayer)Layers[l]).Units; i++)
                {
                    for (int j = 0; j <= ((BLayer)Layers[l - 1]).Units; j++)
                        ((BLayer)Layers[l]).Weights[i][j] = ((BLayer)Layers[l]).WeightSave[i][j];
                }
            }
        }

        private void PropagateNet()
        {
            for (int l = 0; l < layersCount - 1; l++)
                ((BLayer)Layers[l]).PropagateLayer(this, (BLayer)Layers[l + 1]);
        }

        private void BackpropagateNet()
        {
            for (int l = layersCount - 1; l > 1; l--)
                ((BLayer)Layers[l]).BackpropagateLayer(this, (BLayer)Layers[l - 1]);
        }

        private void AdjustWeights()
        {
            double Out, Err, dWeight;
            //fprintf(f, "AdjustWeights\n");

            for (int l = 1; l < layersCount; l++)
            {
                for (int i = 1; i <= ((BLayer)Layers[l]).Units; i++)
                {
                    for (int j = 0; j <= ((BLayer)Layers[l - 1]).Units; j++)
                    {
                        Out = Layers[l - 1].Output[j];
                        Err = ((BLayer)Layers[l]).Error[i];
                        dWeight = ((BLayer)Layers[l]).dWeight[i][j];
                        ((BLayer)Layers[l]).Weights[i][j] += Eta * Err * Out + Alpha * dWeight;
                        ((BLayer)Layers[l]).dWeight[i][j] = Eta * Err * Out;
                        //if (j == 0)
                        //{
                        //    fprintf(f, "A: Out=%lf Err=%lf Net-Layer[%d]->dWeight[%d][%d]=%lf\n", l, i, j, Out, Err, dWeight);
                        //}
                    }
                }
            }
        }

        public void SimulateNet(double[] input, double[] target, out double[] _output, bool training)
        {
            SetInput(input);
            PropagateNet();
            GetOutput(out _output);

            ComputeOutputError(target);
            if (training)
            {
                BackpropagateNet();
                AdjustWeights();
            }
        }

        public void TrainNet(double[] input, double[] target, int epochs)
        {
            double[] _output;
            //int year;
            //double[] _output;

            for (int n = 0; n < epochs; n++)
                SimulateNet(input, target, out _output, true);
            //{
            //    year = GetRandomInt(1, 10);
            //    SimulateNet(new double[] { 45.0 }, new double[] { .1, .9 }, out _output, true);
            //}
        }

        //public void TestNet()
        //{
        //    int year;
        //    double[] Output = new double[Neu[layersCount - 1]];

        //    TrainError = 0;
        //    for (year = 10; year <= 10; year++)
        //    {
        //        SimulateNet(new double[] { 45.0 }, Output, new double[] { .1, .9 }, false);//remember false
        //        TrainError += Error;
        //    }
        //    TestError = 0;
        //    for (year = 10; year <= 10; year++)
        //    {
        //        SimulateNet(new double[] { 45.0 }, Output, new double[] { .1, .9 }, false);//remember false
        //        TestError += Error;
        //    }
        //    //fprintf(f, "\nNMSE is %0.3f on Training Set and %0.3f on Test Set",
        //    //           TrainError / TrainErrorPredictingMean,
        //    //           TestError / TestErrorPredictingMean);
        //}

        public void EvaluateNet(double[] input, double[] target, out double[] _output)
        {
        //    int year;
            //double[] _output;
            //double[] _Output = new double[Neu[layersCount - 1]];

        //    //fprintf(f, "\n\n\n");
        //    //fprintf(f, "Year    Sunspots    Open-Loop Prediction    Closed-Loop Prediction\n");
        //    //fprintf(f, "\n");
        //    for (year = 10; year <= 10; year++)
        //    {
                SimulateNet(input, target, out _output, false);//remember false
        //        SimulateNet(new double[] { 45.0 }, _Output, new double[] { .1, .9 }, false);//remember false
        //        //Sunspots_[Year] = Output_[0];
        //        //fprintf(f, "%d       %0.3f                   %0.3f                     %0.3f\n",
        //        //           FIRST_YEAR + Year,
        //        //           Sunspots[Year],
        //        //           Output[0],
        //        //           Output_[0]);
        //    }
        }

        public int GetRandomInt(int minimum, int maximum)
        {
            Random random = new Random();
            return random.Next(minimum, maximum);
        }
        public double GetRandomDouble(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
