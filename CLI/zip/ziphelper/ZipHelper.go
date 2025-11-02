package ziphelper

import (
	"iobootstrap-cli-shared/core"
	"os"
	"path/filepath"
)

type ZipHelper struct {
	logger              *core.Logger
	cliStep             *core.CLIStep
	executorInitializer *core.ExecutorInitializer
	environment         *string
	outputPath          *string
}

func NewZipHelper(logger *core.Logger, cliStep *core.CLIStep, executorInitializer *core.ExecutorInitializer, environment *string, outputPath *string) ZipHelper {
	return ZipHelper{
		logger:              logger,
		cliStep:             cliStep,
		executorInitializer: executorInitializer,
		environment:         environment,
		outputPath:          outputPath,
	}
}

func (helper *ZipHelper) PrepareZip() {
	(*helper.cliStep).StartStep("Prepare output")

	zipFile := helper.getZipFileName()
	if _, err := os.Stat(zipFile); !os.IsExist(err) {
		removeError := os.RemoveAll(zipFile)
		if removeError != nil {
			(*helper.logger).LogErrorf("%v", removeError)
		}
	}

	(*helper.cliStep).EndStep()
}

func (helper *ZipHelper) ZipOutput() {
	(*helper.cliStep).StartStep("Zip output")

	absDir, err := filepath.Abs(*helper.outputPath)
	if err != nil {
		helper.logger.LogErrorf("%v", err)
	}

	workingDirectory := filepath.Join(absDir, "../")
	helper.logger.LogInfof("Working dir: %s", workingDirectory)
	helper.logger.LogInfoSeparator(32)

	zipPath := filepath.Join(workingDirectory, *helper.environment+".zip")
	helper.logger.LogInfof("zipPath dir: %s", zipPath)
	helper.logger.LogInfoSeparator(32)

	executor := (*helper.executorInitializer).CreateExecutor("ditto", "-c", "-k", "--sequesterRsrc", "--keepParent", absDir, zipPath)
	executor.WorkingDirectory(workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
}

func (helper *ZipHelper) getZipFileName() string {
	return *helper.outputPath + ".zip"
}
