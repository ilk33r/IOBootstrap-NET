package utilities

import (
	"fmt"
	"io"
	"os"
	"path/filepath"
	"syscall"
)

func CopyDirectory(scrDir, dest string) {
	entries, err := os.ReadDir(scrDir)
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}

	for _, entry := range entries {
		sourcePath := filepath.Join(scrDir, entry.Name())
		destPath := filepath.Join(dest, entry.Name())

		fileInfo, err := os.Stat(sourcePath)
		if err != nil {
			fmt.Fprintf(os.Stderr, "%v", err)
			os.Exit(1)
		}

		stat, ok := fileInfo.Sys().(*syscall.Stat_t)
		if !ok {
			fmt.Fprintf(os.Stderr, "failed to get raw syscall.Stat_t data for '%s'", sourcePath)
			os.Exit(1)
		}

		switch fileInfo.Mode() & os.ModeType {
		case os.ModeDir:
			CreateIfNotExists(destPath, 0755)
			CopyDirectory(sourcePath, destPath)

		case os.ModeSymlink:
			CopySymLink(sourcePath, destPath)

		default:
			Copy(sourcePath, destPath)
		}

		if err := os.Lchown(destPath, int(stat.Uid), int(stat.Gid)); err != nil {
			fmt.Fprintf(os.Stderr, "%v", err)
			os.Exit(1)
		}

		fInfo, err := entry.Info()
		if err != nil {
			fmt.Fprintf(os.Stderr, "%v", err)
			os.Exit(1)
		}

		isSymlink := fInfo.Mode()&os.ModeSymlink != 0
		if !isSymlink {
			if err := os.Chmod(destPath, fInfo.Mode()); err != nil {
				fmt.Fprintf(os.Stderr, "%v", err)
				os.Exit(1)
			}
		}
	}
}

func Copy(srcFile, dstFile string) {
	out, err := os.Create(dstFile)
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}

	defer out.Close()

	in, err := os.Open(srcFile)
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}

	defer in.Close()

	_, err = io.Copy(out, in)
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}
}

func Exists(filePath string) bool {
	if _, err := os.Stat(filePath); os.IsNotExist(err) {
		return false
	}

	return true
}

func CreateIfNotExists(dir string, perm os.FileMode) {
	if Exists(dir) {
		return
	}

	if err := os.MkdirAll(dir, perm); err != nil {
		fmt.Fprintf(os.Stderr, "failed to create directory: '%s', error: '%s'", dir, err.Error())
		os.Exit(1)
	}
}

func CopySymLink(source, dest string) {
	link, err := os.Readlink(source)
	if err != nil {
		fmt.Fprintf(os.Stderr, "%v", err)
		os.Exit(1)
	}

	linkError := os.Symlink(link, dest)
	if linkError != nil {
		fmt.Fprintf(os.Stderr, "%v", linkError)
		os.Exit(1)
	}
}
