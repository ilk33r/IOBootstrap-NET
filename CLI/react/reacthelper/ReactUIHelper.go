package reacthelper

import (
	"iobootstrap-cli-shared/core"
)

type ReactUIHelper struct {
	logger              *core.Logger
	cliStep             *core.CLIStep
	executorInitializer *core.ExecutorInitializer
	workingDirectory    *string
	environment         *string
}

func NewReactUIHelper(logger *core.Logger, cliStep *core.CLIStep, executorInitializer *core.ExecutorInitializer, workingDirectory *string, environment *string) ReactUIHelper {
	return ReactUIHelper{
		logger:              logger,
		cliStep:             cliStep,
		executorInitializer: executorInitializer,
		workingDirectory:    workingDirectory,
		environment:         environment,
	}
}

func (helper *ReactUIHelper) InstallDependencies() {
	(*helper.cliStep).StartStep("npm install")

	executor := (*helper.executorInitializer).CreateExecutorWithoutSTDErr("npm", "install")
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *ReactUIHelper) Clean() {
	(*helper.cliStep).StartStep("Clean React.UI")

	executor := (*helper.executorInitializer).CreateExecutorWithoutSTDErr("npm", "run", "clean")
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *ReactUIHelper) Build() {
	(*helper.cliStep).StartStep("Build React.UI")

	environment := *helper.environment
	executor := (*helper.executorInitializer).CreateExecutorWithoutSTDErr("npm", "run", "build:"+environment)
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}
