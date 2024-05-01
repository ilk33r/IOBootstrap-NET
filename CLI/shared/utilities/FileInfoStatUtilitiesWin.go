//go:build windows

package utilities

import (
	"fmt"
	"io/fs"
	"os"
	"syscall"
)

type FileInfoStatUtilities struct {
	stat *syscall.Win32FileAttributeData
}

func NewFileInfoStatUtilities(fileInfo *fs.FileInfo, sourcePath *string) FileInfoStatUtilities {
	stat, ok := (*fileInfo).Sys().(*syscall.Win32FileAttributeData)
	if !ok {
		fmt.Fprintf(os.Stderr, "failed to get raw syscall.Stat_t data for '%s'", *sourcePath)
		os.Exit(1)
	}

	return FileInfoStatUtilities{
		stat: stat,
	}
}

func (fileInfo FileInfoStatUtilities) CopyFilePermission(destination string) {
}
