using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public class MyGridView {
    public MyGridView() {
        this.Gridview = new GridView();
        this.LabelCurrentFolder = new Label();
        this.FSItems = new List<MyFile>();
        this.Page = new Page();
        this.MyFile = new MyFile();
    }

    public GridView Gridview { get; set; }
    public Label LabelCurrentFolder { private get; set; }
    public string CurrentFolder { get; set; }
    public string HomeFolder { internal get; set; }
    public List<MyFile> FSItems { get; internal set; }
    public string GridViewName { get; set; }
    public Page Page { protected get; set; }
    public GridViewRow GridViewRow { get; set; }
    internal DirectoryInfo CurrentDirectoryInfo { get; set; }
    internal DirectoryInfo[] Folders { get; set; }
    internal FileInfo[] Files { get; set; }
    protected MyFile MyFile { get; set; }

    public void setCurrentDirectoryInfo(string currentFolder) {
        this.CurrentDirectoryInfo = new DirectoryInfo(currentFolder);
        this.CurrentFolder = currentFolder;
        this.Folders = this.CurrentDirectoryInfo.GetDirectories();
        this.Files = this.CurrentDirectoryInfo.GetFiles();
    }

    public void setCurrentDirectoryInfo() {
        setCurrentDirectoryInfo(this.CurrentFolder);
    }

    // Update the passed Gridview current folder Label
    public string getCurrentFolderLabelText() {
        return "Viewing the folder <b> " + this.CurrentFolder + " </b> ";
    }

    // Set Visibility for search fields
    public bool getSearchVisibility() {
        return (this.CurrentFolder != this.HomeFolder && Path.GetDirectoryName(this.CurrentFolder) == this.HomeFolder);
    }

    // Clear the FSItems list
    // Add ".." Parent Folder LinkButton option to FSItems
    public void updateMyGridviewDataSource() {
        // Current Folder must be atleast step down from the home folder
        if (notInHomeFolder()) {
            this.FSItems?.Clear();
            MyFile parentFolder = new MyFile(CurrentDirectoryInfo.Parent);
            parentFolder.Name = "..";
            this.FSItems.Add(parentFolder);
        }
        // Choose to either SET or ADD FSItems, depends if displaying home folder
        if (this.FSItems.Count > 0) {
            this.FSItems.AddRange(this.Folders.Select(folder => new MyFile(folder)).Concat(this.Files.Select(file => new MyFile(file))).ToList());
        } else {
            this.FSItems = this.Folders.Select(folder => new MyFile(folder)).Concat(this.Files.Select(file => new MyFile(file))).ToList();
        }
    }

    // Return whether the folders have the same path
    private bool notInHomeFolder() {
        return CurrentDirectoryInfo.FullName != this.HomeFolder;
    }
}