package helper

import (
	"iobootstrap-cli-shared/core"
	"os"
	"path/filepath"
)

type IDotnetHelper interface {
}

type DotnetHelper struct {
	logger              *core.Logger
	cliStep             *core.CLIStep
	executorInitializer *core.ExecutorInitializer
	workingDirectory    *string
	environment         *string
	outputPath          *string
}

func NewDotnetHelper(logger *core.Logger, cliStep *core.CLIStep, executorInitializer *core.ExecutorInitializer, workingDirectory *string, environment *string, outputPath *string) DotnetHelper {
	return DotnetHelper{
		logger:              logger,
		cliStep:             cliStep,
		executorInitializer: executorInitializer,
		workingDirectory:    workingDirectory,
		environment:         environment,
		outputPath:          outputPath,
	}
}

func (helper *DotnetHelper) PrepareOutput() {
	(*helper.cliStep).StartStep("Prepare output")

	if _, err := os.Stat(*helper.outputPath); os.IsExist(err) {
		removeError := os.RemoveAll(*helper.outputPath)
		if removeError != nil {
			(*helper.logger).LogErrorf("%v", removeError)
		}
	}

	(*helper.cliStep).EndStep()
}

func (helper *DotnetHelper) Restore() {
	(*helper.cliStep).StartStep("Restore solution")

	executor := (*helper.executorInitializer).CreateExecutor("dotnet", "restore")
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *DotnetHelper) Clean() {
	(*helper.cliStep).StartStep("Clean solution")

	executor := (*helper.executorInitializer).CreateExecutor("dotnet", "clean")
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *DotnetHelper) Publish(environment string, output string) {
	(*helper.cliStep).StartStep("Publish solution")

	executor := (*helper.executorInitializer).CreateExecutor("dotnet", "publish", "--configuration", environment, "--output", output)
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *DotnetHelper) CreateWWW(output string) {
	(*helper.cliStep).StartStep("Create www dir")

	wwwPath := filepath.Join(output, "wwwroot")
	if _, err := os.Stat(wwwPath); os.IsNotExist(err) {
		dirError := os.Mkdir(wwwPath, os.ModeDir)
		if dirError != nil {
			(*helper.logger).LogErrorf("%v", dirError)
		}
	}

	(*helper.cliStep).EndStep()
}
