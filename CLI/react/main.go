package main

import (
	"flag"
	"fmt"
	"iobootstrap-cli-react/helper"
	"iobootstrap-cli-shared/core"
	"os"
)

func usage() {
	fmt.Fprintf(os.Stderr, "usage: iobootstrap-cli [options] [output]")
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
	reactHelper := helper.NewReactHelper(&logger, &cliStep, &executorInitializer, workingDirectory, environment, &outputPath)

	reactHelper.InstallDependencies()
	reactHelper.Clean(outputPath)
	reactHelper.Build(*environment)
	reactHelper.PrepareOutput(outputPath)
	reactHelper.CopyOutput(outputPath)

	cliStep.Summary()
}
