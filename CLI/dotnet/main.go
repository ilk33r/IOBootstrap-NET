package main

import (
	"dotnet/loader"
	"flag"
	"fmt"
	"os"
)

func usage() {
	fmt.Fprintf(os.Stderr, "usage: iobootstrap-cli [options] [output]")
	flag.PrintDefaults()
	os.Exit(2)
}

var (
	SharedLibraryPath = ""
	workingDirectory  = flag.String("w", "", "Working directory")
	environment       = flag.String("e", "", "Environment")
)

func main() {
	// Parse flags.
	flag.Usage = usage
	flag.Parse()

	// Parse and validate arguments.
	outputPath := ""
	args := flag.Args()
	if len(args) != 1 {
		usage()
	}

	if len(args) >= 1 {
		outputPath = args[0]
	}

	plugin := loader.LoadPlugin(SharedLibraryPath)
	logger := loader.InitializeLogger(plugin)
	cliStep := loader.InitializeCLIStep(plugin)

	if outputPath == "" {
		logger.LogErrorf("Invalid output path %q", outputPath)
	}

	fmt.Fprintf(os.Stdout, "%v", cliStep)
	/*
		cliStep := core.NewCliStep()

		cliStep.StartStep("dotnet publish")
		// executor := core.NewExecutor("dotnet", "publish", "--configuration", "Staging", "--output", outputPath)
		// executor.WorkingDirectory(*workingDirectory)
		// executor.Run()
		time.Sleep(1 * time.Second)
		cliStep.EndStep()

		cliStep.StartStep("dotnet restore")
		time.Sleep(2 * time.Second)
		cliStep.EndStep()

		cliStep.StartStep("dotnet clean")
		time.Sleep(1 * time.Second)
		cliStep.EndStep()

		cliStep.StartStep("dotnet build")
		time.Sleep(1 * time.Second)
		cliStep.EndStep()

		cliStep.Summary()
	*/
}
