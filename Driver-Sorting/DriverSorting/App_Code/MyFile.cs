using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public class MyFile {
    public MyFile() { }

    public MyFile(FileInfo file) {
        this.Name = file.Name;
        this.DirectoryName = Path.GetDirectoryName(file.FullName);
        this.FullName = file.FullName;
        this.Size = file.Length;
        this.CreationTime = file.CreationTime;
        this.LastAccessTime = file.LastAccessTime;
        this.LastWriteTime = file.LastWriteTime;
        this.IsFolder = false;
    }

    public MyFile(DirectoryInfo folder) {
        this.Name = folder.Name;
        this.DirectoryName = Path.GetDirectoryName(folder.FullName);
        this.FullName = folder.FullName;
        this.CreationTime = folder.CreationTime;
        this.LastAccessTime = folder.LastAccessTime;
        this.LastWriteTime = folder.LastWriteTime;
        this.IsFolder = true;
    }

    public string Name { get; set; }
    public string DirectoryName { get; set; }
    public string FullName { get; set; }
    public string NewFullPath { get; private set; }
    public string From { get; set; }
    public string WithoutExtension { get; set; }
    public bool IsFolder { get; internal set; }

    // NEEDED FOR THE ASPX PAGE
    public long? Size { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastWriteTime { get; set; }


    public string FileSystemType {
        get {
            if (this.IsFolder)
                return "File folder";
            else {
                var extension = Path.GetExtension(this.Name);

                if (IsMatch(extension, ".txt"))
                    return "Text file";
                else if (IsMatch(extension, ".pdf"))
                    return "PDF file";
                else if (IsMatch(extension, ".doc", ".docx"))
                    return "Microsoft Word document";
                else if (IsMatch(extension, ".xls", ".xlsx"))
                    return "Microsoft Excel document";
                else if (IsMatch(extension, ".jpg", ".jpeg"))
                    return "JPEG image file";
                else if (IsMatch(extension, ".gif"))
                    return "GIF image file";
                else if (IsMatch(extension, ".png"))
                    return "PNG image file";


                // If we reach here, return the name of the extension
                if (string.IsNullOrEmpty(extension))
                    return "Unknown file type";
                else
                    return extension.Substring(1).ToUpper() + " file";
            }
        }
    }

    private bool IsMatch(string extension, params string[] extensionsToCheck) {
        foreach (var str in extensionsToCheck)
            if (string.CompareOrdinal(extension, str) == 0)
                return true;

        // If we reach here, no match
        return false;
    }

    internal int getChildren() {
        //NetworkDrives.MapDrive("iboxx");                                                                                                    //uncomment 3/23
        int numberOfChildDirectories = Directory.GetDirectories(this.FullName).Length;
        int numberofChildFiles = Directory.GetFiles(this.FullName).Length;
        //NetworkDrives.DisconnectDrive("iboxx");                                                                                                    //uncomment 3/23
        return numberOfChildDirectories + numberofChildFiles;
    }



    // TODO NOT CURRENTLY USED

    public void createNewPath() {
        //NewFullPath = DestinationHomeFolder + @"\Archived Drivers" +
        //  this.FullPath.Substring(DestinationHomeFolder.Length).Substring(0,
        //  this.FullPath.Substring(DestinationHomeFolder.Length).Length);
    }

    public string formatNewPath() {
        return string.Format("{0}-{1:ddMMMyyyy-HHmm.fff}.exe", NewFullPath.Substring(0, NewFullPath.Length - 4), DateTime.Now);
    }

    public void setDestinationPathEXE() {
        this.FullName = this.FullName.Substring(0, this.FullName.Length - 4) + ".exe";
    }
}