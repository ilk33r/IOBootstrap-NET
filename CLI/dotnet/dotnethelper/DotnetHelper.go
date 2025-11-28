package dotnethelper

import (
	"iobootstrap-cli-shared/core"
	"os"
	"path/filepath"
)

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

	if _, err := os.Stat(*helper.outputPath); !os.IsExist(err) {
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

func (helper *DotnetHelper) Publish() {
	(*helper.cliStep).StartStep("Publish solution")

	environment := *helper.environment
	output := *helper.outputPath
	executor := (*helper.executorInitializer).CreateExecutor("dotnet", "publish", "--configuration", environment, "--output", output)
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *DotnetHelper) CreateWWW() {
	(*helper.cliStep).StartStep("Create www dir")

	output := *helper.outputPath
	wwwPath := filepath.Join(output, "wwwroot")
	if _, err := os.Stat(wwwPath); !os.IsExist(err) {
		dirError := os.Mkdir(wwwPath, 0755)
		if dirError != nil {
			(*helper.logger).LogInfof("%v", dirError)
		}
	}

	(*helper.cliStep).EndStep()
}
