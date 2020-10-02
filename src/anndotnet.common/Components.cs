﻿using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnnDotNET.Common
{
    public enum ProgressType
    {
        Initialization,
        Training,
        Completed,
    }
    public enum EarlyStopping
    {
        None,
    }
    public enum TrainingType
    {
        TrainValidation,
        CrossValidation,
    }
    public class AnnLearner
    {
        public Tensor Eval { get; set; }
        public Tensor Loss { get; set; }
        public Operation Learner { get; set; }
    }

    public class TrainingParameters
    {
        public TrainingType TrainingType { get; set; }

        public EarlyStopping EarlyStopping { get; set; }

        public int Epoch { get; set; }

        public int ProgressStep { get; set; }

        public int MinibatchSize { get; set; }

        public int KFold { get; set; }

        public int SplitPercentage { get; set; }

        public Action<TrainingProgress> Progress { get; set; }

        public TrainingParameters()
        {
            TrainingType = TrainingType.TrainValidation;
            EarlyStopping = EarlyStopping.None;
            Epoch = 5000;
            ProgressStep = 10;
            MinibatchSize = 100;
            KFold = 5;
            SplitPercentage = 20;
        }
    }

    public class TrainingProgress
    {
        public ProgressType ProgressType { get; set; }
        public int Iteration { get; set; }
        public float Loss { get; set; }
        public float Eval { get; set; }
    }
   public  class Ann
    {
        //placeholders
        Tensor x { get; set; }
        Tensor y { get; set; }
        //model
        Tensor z { get; set; }
    }

}
