using McMaster.Extensions.CommandLineUtils;
using System;

namespace TestGenerator
{
	public class Program
	{
    private CommandLineApplication CmdParser { get; }
    private CommandOption SourcePath { get; }
    private CommandOption DestPath { get; }
    private CommandOption SourceClassName { get; }

    public Program()
    {
      CmdParser = new CommandLineApplication();
      CmdParser.HelpOption();
      InitCommandLineParameters();

      CmdParser.OnExecute(() =>
      {
        Console.WriteLine($"{SourcePath}, {DestPath}, {SourceClassName}");
        return OnAppExecute();
      });
    }


    private int Run(string[] args)
    {
      try
      {
        CmdParser.Execute(args);
        return 0;
      }
      catch (Exception)
      {
        return 1;
      }
    }

    private static int OnAppExecute()
    {
      //var testGenerator = new TestGenerator(args);
      //testGenerator.Execute();

      return 0;
    }

    private void InitCommandLineParameters()
    {
      var sourcePath = CmdParser
        .Option(
          "-sourcePath <DIR>",
          "The source folder of the input interface level test class.",
          CommandOptionType.SingleValue)
        .IsRequired()
        .Accepts((dir) => dir.ExistingDirectory());

      var destPath = CmdParser
        .Option(
          "-destPath <DIR>",
          "The destination folder of class generation.",
          CommandOptionType.SingleValue)
        .IsRequired()
        .Accepts((dir) => dir.ExistingDirectory());

      var sourceClassName = CmdParser
        .Option(
          "-sourceClassName <TestFileName>",
          "The name of the source class of generation.",
          CommandOptionType.SingleValue)
        .IsRequired();
    }

    public static int Main(string[] args)
		{
      var program = new Program();
      return program.Run(args);
    }
  }
}