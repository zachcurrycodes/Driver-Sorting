using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

public class MyEventExpandCollapse : MyEvent {
    public MyEventExpandCollapse() {
        this.SearchedWords = new List<string>();
        this.ImgBtnExpand = new ImageButton();
        this.RadioButtonVisible = false;
    }

    public List<string> SearchedWords { private get; set; }
    public ImageButton ImgBtnExpand { private get; set; }
    private bool RadioButtonVisible { get; set; }

    // Expand the selected GridviewRow to display nested files/folders within selected Folder
    public void expandInnerGridView() {
        // Open insideGridView if not already visible
        // Change the minues to a plus
        openInsideGridView();
        orderDriverPrefixes();
        GridViewInner.Visible = true;
        ImgBtnExpand.ImageUrl = "./images/open.gif";
    }

    public void collapseAllInnerGridViews() {
        foreach (GridViewRow gvr in GridView.Rows) {
            GridView gridView = (GridView)gvr.FindControl("gvInsideFolder");
            if (gridView.Visible) {
                ImageButton imageButton = (ImageButton)gvr.FindControl("expand");
                imageButton.ImageUrl = "./images/closed.gif";
                gridView.Visible = false;
                gridView = null;
            }
        }
    }

    public void collapseInnerGridView() {
        ImgBtnExpand.ImageUrl = "./images/closed.gif";
        GridViewInner.Visible = false;
        GridViewInner = null;
    }

    private void openInsideGridView() {
        if (!GridViewInner.Visible) {
            // Get the list of files & folders in the lowerDirectory
            // Populate FSItems
            //NetworkDrives.MapDrive("iboxx");
            MyGridView.setCurrentDirectoryInfo(getLowerDirectory());
            //NetworkDrives.DisconnectDrive("iboxx");
            MyGridView.FSItems = populateFSItems();
            if (MyGridView.FSItems.Count != 0) {
                // Set gvInsideFolder DataSource equal to myGridview.FSItems and Bind Data
                // Change checkbox visibility to true for expanded details/folders
                // Return to current folder on myGridView
                GridViewInner.DataSource = MyGridView.FSItems;
                GridViewInner.DataBind();
                displayFileNameAsLiteral();
                displayRenameAndMoveColumns();
                changeRadioButtonVisibility();
                //NetworkDrives.MapDrive("iboxx");
                MyGridView.setCurrentDirectoryInfo(DestinationCurrentFolder);
                //NetworkDrives.DisconnectDrive("iboxx");
            }
        }
    }
    
    // Get name of Folder to expand
    private string getLowerDirectory() {
        string folderToExpand = (GridViewRowSelected.FindControl("lbFolderItem") as LinkButton).Text;
        if (folderToExpand == "") {
            folderToExpand = (GridViewRowSelected.FindControl("ltlFileItem") as Literal).Text;
        }
        //string folderToExpand = expandThis.Substring(expandThis.IndexOf(";") + 1);
        folderToExpand = cleanString(folderToExpand);
        return this.DestinationCurrentFolder + "\\" + folderToExpand;
    }

    // Change checkbox visibility to true for expanded details/folders
    private void changeRadioButtonVisibility() {
        // For each row in GvInsideFolder, determine if row is a directory
        // If so, check if directory has no directory children
        // If so, make checkbox visible for that folder
        foreach (GridViewRow gvr in GridViewInner.Rows) {
            string insideGVRPath = Path.Combine(MyGridView.CurrentFolder, (gvr.FindControl("ltlFileItem") as Literal).Text);
            string home = DestinationHomeFolder;
            if (Directory.Exists(insideGVRPath) && DestinationCurrentFolder != home) {
                RadioButton rbSelect = gvr.FindControl("rbSelect") as RadioButton;
                rbSelect.Visible = true;
                RadioButtonVisible = true;
            }
        }
    }

    // Return the FileSystemItemCS List to populate myGridview.FSItems
    private List<MyFile> populateFSItems() {
        // Search Mode will determine how FSItems List will be populated
        if (SearchMode) {
            // True - Find all Files and Folders that include at least one of the searched criteria
            // Sort the List and remove duplicates
            List<MyFile> foundItems = MyGridView.Folders.Select(folder => new MyFile(folder)).Concat(MyGridView.Files.Select(file => new MyFile(file))).ToList();
            return SearchedWords.SelectMany(word => foundItems.FindAll(y => y.Name.Contains(word)).Concat(foundItems.FindAll(file => file.FullName.Contains(word)))).Distinct().OrderBy(file2 => file2.Name).ToList();
        } else {
            // False - Find all Files and Folders inside the current Folder and return
            return MyGridView.Folders.Select(folder => new MyFile(folder)).Concat(MyGridView.Files.Select(file => new MyFile(file))).ToList();
        }
    }

    private void displayRenameAndMoveColumns() {
        if (Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(DestinationCurrentFolder))) == "Drivers") {
            GridViewInner.Columns[4].Visible = true;
            GridViewInner.Columns[5].Visible = true;
            GridViewInner.Columns[6].Visible = true;
            GridViewInner.Columns[8].Visible = true;
            GridViewInner.Columns[0].Visible = false;
            hideMoveArrowsForFirstAndLastRow();
        }
    }

    private void displayFileNameAsLiteral() {
        foreach (GridViewRow gvr in GridViewInner.Rows) {
            Literal ltlFileItem = gvr.FindControl("ltlFileItem") as Literal;
            ltlFileItem.Text = MyGridView.FSItems[gvr.RowIndex].Name;
        }
    }
}