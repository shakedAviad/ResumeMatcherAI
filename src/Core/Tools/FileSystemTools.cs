using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Tools
{
    public class FileSystemTools
    {
        private readonly string _rootFolder;

        public FileSystemTools(string rootFolder)
        {
            _rootFolder = rootFolder;
            if (!Directory.Exists(_rootFolder))
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        public string GetRootFolder()
        {
            return _rootFolder;
        }
        
        public void AddFile(string filePath)
        {
            MoveFile(filePath, Path.Combine(_rootFolder, Path.GetFileName(filePath)));
        }

        public void CreateFolder(string folderPath)
        {
            Guard(folderPath);
            Directory.CreateDirectory(folderPath);
        }
       
        public void MoveFile(string sourceFilePath, string targetFilePath)
        {
            Guard(sourceFilePath);
            Guard(targetFilePath);
            File.Move(sourceFilePath, targetFilePath);
        }

        public void MoveFolder(string sourceFolderPath, string targetFolderPath)
        {
            Guard(sourceFolderPath);
            Guard(targetFolderPath);
            Directory.Move(sourceFolderPath, targetFolderPath);
        }

        public string[] GetFiles(string folderPath)
        {
            Guard(folderPath);
            return Directory.GetFiles(folderPath);
        }

        public string[] GetFolders(string folderPath)
        {
            Guard(folderPath);
            return Directory.GetDirectories(folderPath);
        }

        public void DeleteFolder(string folderPath)
        {
            if (folderPath == _rootFolder)
            {
                throw new Exception("You are not allowed to delete the Root Folder");
            }

            Guard(folderPath);
            Directory.Delete(folderPath);
        }

        public void DeleteFile(string filePath)
        {
            Guard(filePath);
            File.Delete(filePath);
        }

        private void Guard(string folderPath)
        {
            if (!folderPath.StartsWith(_rootFolder))
            {
                throw new Exception("No you don't!");
            }
        }
    }
}
