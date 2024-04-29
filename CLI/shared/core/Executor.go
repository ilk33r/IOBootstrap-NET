package core

import (
	"fmt"
	"os/exec"
)

type IExecutor interface {
	WorkingDirectory(dir string)
	Run()
}

type Executor struct {
	logger      *Logger
	command     *exec.Cmd
	commandName *string
}

func NewExecutor(logger *Logger, name string, args ...string) Executor {
	var commandString = name

	for argIndex := range args {
		commandString += fmt.Sprintf(" %v", args[argIndex])
	}

	logger.LogInfo("Starting command:\n")
	logger.LogSuccess(commandString)
	logger.LogInfoSeparator(len(commandString))

	return Executor{
		logger:      logger,
		command:     exec.Command(name, args...),
		commandName: &name,
	}
}

func (executor *Executor) WorkingDirectory(dir string) {
	executor.command.Dir = dir
}

func (executor *Executor) Run() {
	stdout, err := executor.command.StdoutPipe()
	if err != nil {
		executor.logger.LogErrorf("%v", err)
	}

	stderr, err := executor.command.StderrPipe()
	if err != nil {
		executor.logger.LogErrorf("%v", err)
	}

	if err = executor.command.Start(); err != nil {
		executor.logger.LogErrorf("%v", err)
	}

	for {
		stdOutBuffer := make([]byte, 1024)
		stdOutSize, stdOutErr := stdout.Read(stdOutBuffer)

		if stdOutSize > 0 {
			stdOutString := string(stdOutBuffer)
			executor.logger.LogMessage(stdOutString)
		}

		stdErrBuffer := make([]byte, 1024)
		stdErrSize, stdErrErr := stderr.Read(stdErrBuffer)

		if stdErrSize > 0 {
			stdErrString := string(stdErrBuffer)
			executor.logger.LogError(stdErrString)
		}

		if stdOutErr != nil && stdErrErr != nil {
			break
		}
	}

	executor.logger.LogInfof("Command %s", *executor.commandName)
	executor.logger.LogSuccess("Success")
}
