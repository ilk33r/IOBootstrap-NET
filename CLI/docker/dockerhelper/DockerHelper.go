package dockerhelper

import (
	"fmt"
	"iobootstrap-cli-shared/core"
	"os"
	"path/filepath"
)

type DockerHelper struct {
	logger              *core.Logger
	cliStep             *core.CLIStep
	executorInitializer *core.ExecutorInitializer
	workingDirectory    *string
	buildPath           *string
	dockerPort          *string
	dockerImageName     *string
	dockerTemplateFile  *string
}

func NewDockerHelper(
	logger *core.Logger,
	cliStep *core.CLIStep,
	executorInitializer *core.ExecutorInitializer,
	workingDirectory *string,
	buildPath *string,
	dockerPort *string,
	dockerImageName *string,
	dockerTemplateFile *string,
) DockerHelper {
	return DockerHelper{
		logger:              logger,
		cliStep:             cliStep,
		executorInitializer: executorInitializer,
		workingDirectory:    workingDirectory,
		buildPath:           buildPath,
		dockerPort:          dockerPort,
		dockerImageName:     dockerImageName,
		dockerTemplateFile:  dockerTemplateFile,
	}
}

func (helper *DockerHelper) CreateDockerFile() {
	(*helper.cliStep).StartStep("Creating Docker File")

	if _, err := os.Stat(*helper.workingDirectory); !os.IsExist(err) {
		removeError := os.RemoveAll(*helper.workingDirectory)
		if removeError != nil {
			(*helper.logger).LogErrorf("%v", removeError)
		}
	}

	dirError := os.Mkdir(*helper.workingDirectory, 0755)
	if dirError != nil {
		(*helper.logger).LogErrorf("%v", dirError)
	}

	dockerFileFormat := helper.DockerFileFormat()
	dockerFileContent := fmt.Sprintf(dockerFileFormat, *helper.dockerPort, *helper.dockerPort, *helper.buildPath)
	dockerFilePath := filepath.Join(*helper.workingDirectory, "DockerFile")
	fileError := os.WriteFile(dockerFilePath, []byte(dockerFileContent), 0755)
	if fileError != nil {
		(*helper.logger).LogErrorf("%v", fileError)
	}

	(*helper.cliStep).EndStep()
}

func (helper *DockerHelper) BuildDockerFile() {
	(*helper.cliStep).StartStep("Build Docker")

	buildRoot := filepath.Join(*helper.workingDirectory, "../")
	dockerFilePath := filepath.Join(*helper.workingDirectory, "DockerFile")
	executor := (*helper.executorInitializer).CreateExecutorWithoutSTDErr("docker", "build", "-t", *helper.dockerImageName, "-f", dockerFilePath, buildRoot)
	executor.WorkingDirectory(*helper.workingDirectory)
	executor.Run()

	(*helper.cliStep).EndStep()
	(*helper.logger).LogWarningf("Run command:\ndocker run -ti --rm --add-host %s.local:host-gateway -p %s:8000 -v ~/DockerShare:/data %s", *helper.dockerImageName, *helper.dockerPort, *helper.dockerImageName)
}

func (helper *DockerHelper) DockerFileFormat() string {
	data, fileError := os.ReadFile(*helper.dockerTemplateFile)
	if fileError != nil {
		(*helper.logger).LogErrorf("%v", fileError)
	}

	return string(data)
}
