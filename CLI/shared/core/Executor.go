package core

import (
	"fmt"
	"io"
	"os/exec"
	"sync"
)

type IExecutor interface {
	WorkingDirectory(dir string)
	Run()
}

type Executor struct {
	logger        *Logger
	command       *exec.Cmd
	commandName   *string
	disableSTDErr bool
}

func NewExecutor(logger *Logger, disableSTDErr bool, name string, args ...string) Executor {
	var commandString = name

	for argIndex := range args {
		commandString += fmt.Sprintf(" %v", args[argIndex])
	}

	logger.LogInfo("Starting command:\n")
	logger.LogSuccess(commandString)
	logger.LogInfoSeparator(len(commandString))

	return Executor{
		logger:        logger,
		command:       exec.Command(name, args...),
		commandName:   &name,
		disableSTDErr: disableSTDErr,
	}
}

func (executor *Executor) WorkingDirectory(dir string) {
	executor.command.Dir = dir
}

func (executor *Executor) Run() {
	stdout, err := executor.command.StdoutPipe()
	if err != nil {
		executor.logger.LogErrorf("%v", err)
		return
	}
	stderr, err := executor.command.StderrPipe()
	if err != nil {
		executor.logger.LogErrorf("%v", err)
		return
	}

	if err := executor.command.Start(); err != nil {
		executor.logger.LogErrorf("%v", err)
		return
	}

	var wg sync.WaitGroup
	wg.Add(2)

	// stdout'u anlık kopyala
	go func() {
		defer wg.Done()
		_, _ = io.Copy(loggerWriter{write: func(p []byte) {
			executor.logger.LogMessage(string(p))
		}}, stdout)
	}()

	// stderr'i anlık kopyala (ister normal, ister error olarak logla)
	go func() {
		defer wg.Done()
		_, _ = io.Copy(loggerWriter{write: func(p []byte) {
			if executor.disableSTDErr {
				executor.logger.LogMessage(string(p))
			} else {
				executor.logger.LogError(string(p))
			}
		}}, stderr)
	}()

	// Süreç çıkışını bekle (pipes kapanacak, goroutine'ler EOF alıp bitecek)
	waitErr := executor.command.Wait()
	wg.Wait()

	if waitErr != nil {
		executor.logger.LogErrorf("Command %s failed: %v", *executor.commandName, waitErr)
		return
	}

	executor.logger.LogInfof("Command %s", *executor.commandName)
	executor.logger.LogSuccess("Success")
}

type loggerWriter struct {
	write func(p []byte)
}

func (lw loggerWriter) Write(p []byte) (int, error) {
	lw.write(p)
	return len(p), nil
}
