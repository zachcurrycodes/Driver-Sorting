using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Linq;


public class MyEvent {
    public MyEvent() {
        this.SearchMode = false;
        this.GridViewInner = new GridView();
        this.MyGridView = new MyGridView();
        //this.Response = new HttpResponse();
        this.Words = new List<string>();
        this.MyGVSource = new MyGridView();
        this.MyGVDestination = new MyGridView();
        this.GridView = new GridView();
        //this.GridViewRowSelected = new GridViewRow();
        this.MyFile = new MyFile();
        this.Error = "";
    }

    public bool SearchMode { get; set; }
    public GridView GridViewInner { get; set; }
    public MyGridView MyGridView { get; set; }
    public HttpResponse Response { protected get; set; }
    public List<string> Words { get; set; }
    public MyGridView MyGVSource { protected get; set; }
    public MyGridView MyGVDestination { protected get; set; }
    public GridView GridView { protected get; set; }
    public GridViewRow GridViewRowSelected { protected get; set; }
    public string DestinationHomeFolder { protected get; set; }
    public string DestinationCurrentFolder { protected get; set; }
    public string DestWebServerFolder { protected get; set; }
    protected MyFile MyFile { get; set; }
    protected string OriginalFileName { get; set; }
    protected string NewFileName { get; set; }
    protected string Prefix { get; set; }
    protected string FolderName { get; set; }
    protected string ExpandedFolder { get; set; }
    public string Error { get; set; }

    // Select only one file per GridView
    public void unselectAllRadioButtons(GridView gv) {
        // Unselect all radiobuttons on Gridview
        // Select the specified RadioButton
        //foreach (GridViewRow gvr in MyGridView.Gridview.Rows) {
        foreach (GridViewRow gvr in gv.Rows) {
            RadioButton rb = gvr.FindControl("rbSelect") as RadioButton;
            rb.Checked = false;
        }
    }

    public void setMyEvent(object sender, MyGridView myGVSource, MyGridView myGVDestination) {
        this.MyGVSource = myGVSource;
        this.MyGVDestination = myGVDestination;
        setMyGridView(sender);
    }

    private void setMyGridView(object sender) {
        if (sender.GetType() == typeof(RadioButton)) {
            // Cast sender as a Gridview
            sender = (GridView)(sender as RadioButton).Parent.Parent.Parent.Parent;
        }
        GridView gvSender = (GridView)sender;
        if (gvSender.ClientID == "gvSource") {
            this.MyGVSource.Gridview = gvSender;
            this.MyGridView = this.MyGVSource;
        } else if (gvSender.ClientID == "gvDestination") {
            this.MyGVDestination.Gridview = gvSender;
            this.MyGridView = this.MyGVDestination;
        } else {
            MyGridView newMyGridview = new MyGridView();
            newMyGridview.Gridview = gvSender;
            this.MyGridView = newMyGridview;
        }
    }

    internal void renameLocal(bool expandFolder) {
        GridViewRow newGVR;
        if (expandFolder) {
            newGVR = this.GridViewRowSelected;
        } else {
            newGVR = this.GridViewRowSelected.Parent.Parent.Parent.Parent as GridViewRow;
        }
        FolderName = (newGVR.FindControl("lbFolderItem") as LinkButton).Text;
        FolderName = cleanString(FolderName);
        ExpandedFolder = this.DestinationCurrentFolder + "\\" + FolderName + "\\";
        renameFile();
    }

    internal void renameOnWebServer() {
        NetworkDrives.MapDrive("web");
        string currentFolder = this.DestinationCurrentFolder.Substring(this.DestinationHomeFolder.Length + 1);
        ExpandedFolder = this.DestWebServerFolder + currentFolder + "\\" + FolderName + "\\";
        renameFile();
        NetworkDrives.DisconnectDrive("web");
    }

    protected void renameFile() {
        string originalPath = ExpandedFolder + OriginalFileName;
        string newPath = ExpandedFolder + NewFileName;
        try {
            //NetworkDrives.MapDrive("iboxx");                                                                                                          //comment for local testing                                     3/26
            File.Move(originalPath, newPath);
            //NetworkDrives.DisconnectDrive("iboxx");                                                                                                   //comment for local testing                                       3/26
        } catch (Exception e) {
            displayError(e);
            throw;
            //this.Response.Write("<script>alert('Unable to renameFile. Reason: " + e + "');</script>");
        }
    }

    protected void refreshInnerGridView() {
        try {
            //NetworkDrives.MapDrive("iboxx");                                                                                                              //comment for local testing IS THIS NEEDED?
            DirectoryInfo directoryInfo = new DirectoryInfo(ExpandedFolder);
            FileInfo[] files = directoryInfo.GetFiles();
            //NetworkDrives.DisconnectDrive("iboxx");                                                                                                       //comment for local testing
            List<MyFile> innerGridViewDataSource = files.Select(file => new MyFile(file)).ToList();
            GridViewInner.DataSource = innerGridViewDataSource;
            GridViewInner.DataBind();
            displayFileNameAsLiteral(innerGridViewDataSource);
            hideMoveArrowsForFirstAndLastRow();
        } catch (Exception e) {
            Error = "Error caught in refreshInnerGridView() " + e;
        }
    }

    private void displayFileNameAsLiteral(List<MyFile> ds) {
        foreach (GridViewRow gvr in GridViewInner.Rows) {
            Literal ltlFileItem = gvr.FindControl("ltlFileItem") as Literal;
            ltlFileItem.Text = ds[gvr.RowIndex].Name;
        }
    }

    protected void hideMoveArrowsForFirstAndLastRow() {
        try {
            GridViewInner.Rows[0].FindControl("Up").Visible = false;
            GridViewInner.Rows[GridViewInner.Rows.Count - 1].FindControl("Down").Visible = false;
        } catch (Exception e) {
            Error = "Error caught in hideMoveArrowsForFirstAndLastRow() \n\r" +
                "Expanded Folder: " + this.ExpandedFolder + "\n\r" + e;
        }
    }

    protected string cleanString(string s) {
        return s.Substring(s.IndexOf(";") + 1);
    }

    protected void displayError(Exception e) {
        Error = "Message: " + e.Message + "\n\r"
            + "Inner Exception: " + e.InnerException + "\n\r"
            + "Stack Trace: " + e.StackTrace;
    }

    internal void orderDriverPrefixes() { //  TODO VERY UGLY!!
        string home = this.DestinationHomeFolder;
        if (DestinationCurrentFolder != home && Path.GetDirectoryName(DestinationCurrentFolder) != home) {
            //setThisForPotentialFileReorder();
            bool nameChange = false;
            foreach (GridViewRow gvr in GridViewInner.Rows) {
                Literal file = (gvr.FindControl("ltlFileItem") as Literal);
                int fileIndex = 0;
                string text = file.Text;
                int ignorMe = 0;
                this.OriginalFileName = "";
                this.NewFileName = "";
                if (text.Substring(2, 1) != "_" || !int.TryParse(text.Substring(0, 2), out ignorMe)) {
                    nameChange = true;
                    this.OriginalFileName = text;
                    this.NewFileName = "00_" + this.OriginalFileName;
                } else {
                    fileIndex = int.Parse(text.Substring(0, 2));
                }
                int iRowIndex = gvr.RowIndex + 1;
                if (iRowIndex != fileIndex) {
                    nameChange = true;
                    this.OriginalFileName = text;
                    string sRowIndex = (iRowIndex).ToString();
                    if (sRowIndex.Length < 2) {
                        sRowIndex = "0" + sRowIndex;
                    }
                    if (NewFileName == "") {
                        this.NewFileName = sRowIndex + this.OriginalFileName.Substring(2);

                    } else {
                        this.NewFileName = sRowIndex + this.NewFileName.Substring(2);
                    }
                }
                if (this.OriginalFileName != this.NewFileName) {
                    this.renameLocal(true);
                    this.renameOnWebServer(); //TODO when in test.boxx.com this can fail due to files not existing
                    nameChange = true;
                }
            }
            if (nameChange) {
                this.refreshInnerGridView();
            }
        }
    }
}