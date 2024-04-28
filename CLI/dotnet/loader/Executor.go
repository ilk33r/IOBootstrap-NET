package loader

type Executor interface {
	WorkingDirectory(dir string)
	Run()
}
