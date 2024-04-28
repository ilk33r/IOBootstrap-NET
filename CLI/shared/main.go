package main

import (
	"flag"
	"fmt"
	"iobootstrap-cli-shared/core"
	"os"
	"time"
)

func usage() {
	fmt.Fprintf(os.Stderr, "usage: iobootstrap-cli [options] [output]")
	flag.PrintDefaults()
	os.Exit(2)
}

var (
	workingDirectory = flag.String("w", "", "Working directory")
	applicationID    = flag.Int("a", 0, "Application ID")
	groupID          = flag.Int("g", 0, "Group ID")
	releaseNotes     = flag.String("r", "", "Release notes")
)

func main() {
	// Configure logging for a command-line program.
	core.InitializeLogger()

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

	if outputPath == "" {
		core.LogErrorf("Invalid output path %q", outputPath)
	}

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
}
