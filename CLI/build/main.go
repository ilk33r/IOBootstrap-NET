package main

import (
	"flag"
	"fmt"
	"iobootstrap-cli-docker/dockerhelper"
	"iobootstrap-cli-dotnet/dotnethelper"
	"iobootstrap-cli-react/reacthelper"
	"iobootstrap-cli-shared/core"
	"os"
	"path/filepath"
)

func usage() {
	fmt.Fprintf(os.Stderr, "usage: iobootstrap-cli-build [options] [output]\n")
	flag.PrintDefaults()
	os.Exit(2)
}

var (
	Version                = "dev"
	netEnvironment         = flag.String("netenv", "", "Net.Core environment")
	reactEnvironment       = flag.String("reactenv", "", "React environment")
	workingDirectory       = flag.String("sln", "", "Net.Core solution directory")
	reactUIDirectory       = flag.String("reactui", "", "React.UI directory")
	boDirectory            = flag.String("backoffice", "", "BackOffice directory")
	boOutputDirName        = flag.String("www-bo-output-dir-name", "", "BackOffice output directory name")
	dockerWorkingDirectory = flag.String("docker-working-dir", "", "Docker working directory")
	dockerBuildDirName     = flag.String("docker-build-dir-name", "", "Docker build directory name")
	dockerPort             = flag.String("docker-port", "", "Docker port")
	dockerImageName        = flag.String("docker-image-name", "", "Docker image name")
	dockerTemplateFile     = flag.String("docker-template-file", "", "Docker template file")
	createDocker           = flag.Bool("docker", false, "Create docker image")
	checkVersion           = flag.Bool("v", false, "Version")
)

func main() {
	logger := core.InitializeLogger()

	// Parse flags.
	flag.Usage = usage
	flag.Parse()

	if *checkVersion {
		fmt.Fprintf(os.Stderr, "Version: %s", Version)
		os.Exit(2)
	}

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

	boOutputPath := filepath.Join(outputPath, "wwwroot/"+*boOutputDirName)
	reactHelper := reacthelper.NewReactHelper(&logger, &cliStep, &executorInitializer, boDirectory, reactEnvironment, &boOutputPath)
	reactHelper.InstallDependencies()
	reactHelper.Clean()
	reactHelper.Build()
	reactHelper.PrepareOutput()
	reactHelper.CopyOutput()

	if *createDocker {
		dockerHelper := dockerhelper.NewDockerHelper(
			&logger,
			&cliStep,
			&executorInitializer,
			dockerWorkingDirectory,
			dockerBuildDirName,
			dockerPort,
			dockerImageName,
			dockerTemplateFile,
		)

		dockerHelper.CreateDockerFile()
		dockerHelper.BuildDockerFile()
	}

	cliStep.Summary()
}
