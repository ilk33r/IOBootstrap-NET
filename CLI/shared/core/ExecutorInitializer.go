package core

type IExecutorInitializer interface {
	CreateExecutor(name string, args ...string) any
}

type ExecutorInitializer struct {
	logger *Logger
}

func NewExecutorInitializer(logger *Logger) ExecutorInitializer {
	return ExecutorInitializer{
		logger: logger,
	}
}

func (initializer *ExecutorInitializer) CreateExecutor(name string, args ...string) any {
	return NewExecutor(initializer.logger, name, args...)
}
