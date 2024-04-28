package main

import (
	"flag"
	"fmt"
	"iobootstrap-cli-shared/core"
	"os"
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

	executor := core.NewExecutor("dotnet", "publish", "--configuration", "Staging", "--output", outputPath)
	executor.WorkingDirectory(*workingDirectory)
	executor.Run()
}
