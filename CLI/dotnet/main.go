package main

import (
	"flag"
	"fmt"
	"iobootstrap-cli-dotnet/helper"
	"iobootstrap-cli-shared/core"
	"os"
)

func usage() {
	fmt.Fprintf(os.Stderr, "usage: iobootstrap-dotnet [options] [output]")
	flag.PrintDefaults()
	os.Exit(2)
}

var (
	Version          = "dev"
	workingDirectory = flag.String("w", "", "Working directory")
	environment      = flag.String("e", "", "Environment")
	checkVersion     = flag.String("v", "set", "Version")
)

func main() {
	logger := core.InitializeLogger()

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

	if *checkVersion == "" {
		fmt.Fprintf(os.Stderr, "Version: %s", Version)
		flag.PrintDefaults()
		os.Exit(2)
	}

	if outputPath == "" {
		logger.LogErrorf("Invalid output path %q", outputPath)
	}

	cliStep := core.NewCliStep(&logger)
	executorInitializer := core.NewExecutorInitializer(&logger)
	dotnetHeler := helper.NewDotnetHelper(&logger, &cliStep, &executorInitializer, workingDirectory, environment, &outputPath)

	dotnetHeler.PrepareOutput()
	dotnetHeler.Restore()
	dotnetHeler.Clean()
	dotnetHeler.Publish(*environment, outputPath)
	dotnetHeler.CreateWWW(outputPath)

	cliStep.Summary()
}
