package main

import (
	"flag"
	"fmt"
	"iobootstrap-cli-dotnet/dotnethelper"
	"iobootstrap-cli-react/reacthelper"
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
	netEnvironment   = flag.String("netenv", "", "Net.Core environment")
	reactEnvironment = flag.String("reactenv", "", "React environment")
	workingDirectory = flag.String("sln", "", "Net.Core solution directory")
	reactUIDirectory = flag.String("reactui", "", "React.UI directory")
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

	dotnetHeler := dotnethelper.NewDotnetHelper(&logger, &cliStep, &executorInitializer, workingDirectory, netEnvironment, &outputPath)
	dotnetHeler.PrepareOutput()
	dotnetHeler.Restore()
	dotnetHeler.Clean()
	dotnetHeler.Publish()
	dotnetHeler.CreateWWW()

	reactUIHelper := reacthelper.NewReactUIHelper(&logger, &cliStep, &executorInitializer, reactUIDirectory, reactEnvironment)
	reactUIHelper.InstallDependencies()
	reactUIHelper.Clean()
	reactUIHelper.Build()

	// reactHelper := reacthelper.NewReactHelper(&logger, &cliStep, &executorInitializer, workingDirectory, reactEnvironment, &outputPath)
	// reactHelper.InstallDependencies()
	// reactHelper.Clean(outputPath)
	// reactHelper.Build(*environment)
	// reactHelper.PrepareOutput(outputPath)
	// reactHelper.CopyOutput(outputPath)

	cliStep.Summary()
}
