//go:build !windows

package utilities

import (
	"fmt"
	"io/fs"
	"os"
	"syscall"
)

type FileInfoStatUtilities struct {
	stat *syscall.Stat_t
}

func NewFileInfoStatUtilities(fileInfo *fs.FileInfo, sourcePath *string) FileInfoStatUtilities {
	stat, ok := (*fileInfo).Sys().(*syscall.Stat_t)
	if !ok {
		fmt.Fprintf(os.Stderr, "failed to get raw syscall.Stat_t data for '%s'", *sourcePath)
		os.Exit(1)
	}

	return FileInfoStatUtilities{
		stat: stat,
	}
}

func (fileInfo FileInfoStatUtilities) CopyFilePermission(destination string) {
	if err := os.Lchown(destination, int(fileInfo.stat.Uid), int(fileInfo.stat.Gid)); err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}
}
