﻿#load "PrepareData.fsx"
#load "CommonFunctions.fsx"

#r "../packages/Accord/lib/net40/Accord.dll"
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll"
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll"
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll"

open Accord
open Accord.Math
open CommonFunctions
open Accord.MachineLearning.Bayes
open Accord.Statistics.Distributions
open Accord.Statistics.Distributions.Fitting
open Accord.Statistics.Distributions.Univariate

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)

//i.WeekNumber; 3-9
//i.SalesDepotId; 1110-25759
//i.SalesChannelId; 1-11
//i.SalesRouteId; 1-9966
//i.ClientId; 26-2015152015
//i.ProductId; 41-49996

let input = 
    trainItems 
    |> Seq.map(fun i -> [|i.WeekNumber; i.SalesDepotId; i.SalesChannelId; i.SalesRouteId; i.ClientId; i.ProductId |] |> Array.map float)
    |> Seq.toArray

let output = 
    trainItems
    |> Seq.map(fun ti -> ti.AdjustedDemand)
    |> Seq.map(fun d -> if d > 20 then 21 else d)
    |> Seq.map(fun d -> if d < 0 then 0 else d)
    |> Seq.toArray

let numberOfClasses = output |> Array.distinct |> Array.length
let numberOfInputs = 6

let bayes = new NaiveBayes<NormalDistribution>(numberOfClasses, numberOfInputs, NormalDistribution.Standard)
let fittingOption = new NormalOptions()
fittingOption.Regularization <- 1e-5
let error = bayes.Estimate(input,output,true,fittingOption)