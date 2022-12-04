// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using ScreenFlipper;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("ScreenFlipper.json")
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection(nameof(Settings)).Get<Settings>();
int endRotateCount = 4 - settings.RotateCount;

PrimaryScreenResolution.ChangeResolution(settings.RotateCount);
var process = Process.Start(settings.Command, args);
process.WaitForExit();
PrimaryScreenResolution.ChangeResolution(endRotateCount);
