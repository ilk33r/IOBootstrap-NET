package loader

type ExecutorInitializer interface {
	CreateExecutor(name string, args ...string) any
}
