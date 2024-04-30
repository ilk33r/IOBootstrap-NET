package reacthelper

import (
	"iobootstrap-cli-shared/core"
	"iobootstrap-cli-shared/utilities"
	"os"
	"path/filepath"
)

type ReactHelper struct {
	logger              *core.Logger
	cliStep             *core.CLIStep
	executorInitializer *core.ExecutorInitializer
	workingDirectory    *string
	environment         *string
	outputPath          *string
}

func NewReactHelper(logger *core.Logger, cliStep *core.CLIStep, executorInitializer *core.ExecutorInitializer, workingDirectory *string, environment *string, outputPath *string) ReactHelper {
	return ReactHelper{
		logger:              logger,
		cliStep:             cliStep,
		executorInitializer: executorInitializer,
		workingDirectory:    workingDirectory,
		environment:         environment,
		outputPath:          outputPath,
	}
}

func (helper *ReactHelper) InstallDependencies() {
	(*helper.cliStep).StartStep("npm install")

	executor := (*helper.executorInitializer).CreateExecutor("npm", "install")
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *ReactHelper) Clean(output string) {
	(*helper.cliStep).StartStep("Clean build")

	buildDirectory := filepath.Join(*helper.workingDirectory, "build")
	if _, err := os.Stat(buildDirectory); !os.IsExist(err) {
		removeError := os.RemoveAll(buildDirectory)
		if removeError != nil {
			(*helper.logger).LogErrorf("%v", removeError)
		}
	}

	if _, err := os.Stat(output); !os.IsExist(err) {
		removeError := os.RemoveAll(output)
		if removeError != nil {
			(*helper.logger).LogErrorf("%v", removeError)
		}
	}

	(*helper.cliStep).EndStep()
}

func (helper *ReactHelper) Build(environment string) {
	(*helper.cliStep).StartStep("Build application")

	executor := (*helper.executorInitializer).CreateExecutor("npm", "run", "build:"+environment)
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *ReactHelper) PrepareOutput(output string) {
	(*helper.cliStep).StartStep("Prepare output")

	if _, err := os.Stat(output); !os.IsExist(err) {
		dirError := os.Mkdir(output, 0755)
		if dirError != nil {
			(*helper.logger).LogErrorf("%v", dirError)
		}
	}

	(*helper.cliStep).EndStep()
}

func (helper *ReactHelper) CopyOutput(output string) {
	(*helper.cliStep).StartStep("Copy output")

	buildDirectory := filepath.Join(*helper.workingDirectory, "build")
	utilities.CopyDirectory(buildDirectory, output)

	(*helper.cliStep).EndStep()
}
