using System.IO;

public class MyEventOpenFolder : MyEvent {
    public MyEventOpenFolder() { }

    public string SourceCurrentFolder { private get; set; }
    public string CommandArgument { private get; set; }
    private string CurrentFolder { get; set; }

    public string openFolderViaLinkButton(string currentFolder) {
        if (currentFolder == "Source") {
            this.CurrentFolder = this.SourceCurrentFolder;
            this.MyGridView = this.MyGVSource;
        } else if (currentFolder == "Destination") {
            this.CurrentFolder = this.DestinationCurrentFolder;
            this.MyGridView = this.MyGVDestination;
        }
        setMyFileOpenFolder(CurrentFolder);
        //NetworkDrives.MapDrive("iboxx");
        if (CommandArgument == "..") {
            MyGridView.setCurrentDirectoryInfo(Path.GetDirectoryName(MyFile.FullName));
        } else {

            //    //for each character
            //    //if value of 160
            //    //replace with 32
            //    char[] charArray = CommandArgument.ToCharArray();
            //    for (int i = 0; i < charArray.Length; i++) {
            //        if (charArray[i] == 160) {
            //            charArray[i] = (char)32;
            //        }
            //    }
            //    //CommandArgument = string.Join("", charArray);
            //    CommandArgument = new string(charArray);




            MyGridView.setCurrentDirectoryInfo(Path.Combine(MyFile.FullName, CommandArgument));
        }
        ////NetworkDrives.DisconnectDrive("iboxx");
        return MyGridView.CurrentFolder;
    }

    private void setMyFileOpenFolder(string gvCurrentFolder) {
        MyFile.Name = Path.GetFileName(gvCurrentFolder);
        MyFile.DirectoryName = Path.GetDirectoryName(gvCurrentFolder);
        MyFile.FullName = Path.GetFullPath(gvCurrentFolder);
        MyFile.WithoutExtension = Path.GetFileNameWithoutExtension(gvCurrentFolder);
    }
}