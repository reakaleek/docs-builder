using ConsoleAppFramework;
using ProcNet;

// ReSharper disable RedundantLambdaParameterType

ConsoleApp.Run(args, () => Proc.Exec("dotnet", "--help"));

