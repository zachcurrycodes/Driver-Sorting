using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page {
    public static string _SOURCEHOME = System.Configuration.ConfigurationManager.AppSettings["MotherboardsPath"];
    public static string _DESTINATIONHOME = System.Configuration.ConfigurationManager.AppSettings["IntranetDriversPath"];
    public static string _DESTINATIONWEBSERVERFOLDER = System.Configuration.ConfigurationManager.AppSettings["WebDriversPath"];
    //public static string _DESTINATIONWEBSERVERFOLDER = @"\\boxx.com\c$\zTest\";
    //public static string _DESTINATIONWEBSERVERFOLDER = @"\\test.boxx.com\c$\Drivers\Drivers\";    

    #region Properties
    public string FocusedID {
        get {
            if (Session["FocusedID"] == null) { FocusedID = ""; }
            return (string)Session["FocusedID"];
        }
        set { Session["FocusedID"] = value; }
    }
    public string SourceHomeFolder {
        get { return _SOURCEHOME as string; }
        set { ViewState["SourceHomeFolder"] = value; }
    }
    public string SourceCurrentFolder {
        get { return ViewState["SourceCurrentFolder"] as string; }
        set { ViewState["SourceCurrentFolder"] = value; }
    }
    public string DestinationHomeFolder {
        get { return _DESTINATIONHOME as string; }
        set { ViewState["DestinationHomeFolder"] = value; }
    }
    public string DestinationWebServerFolder {
        get { return _DESTINATIONWEBSERVERFOLDER as string; }
        set { ViewState["DestinationHomeFolder"] = value; }
    }
    public string DestinationCurrentFolder {
        get { return ViewState["DestinationCurrentFolder"] as string; }
        set { ViewState["DestinationCurrentFolder"] = value; }
    }
    public MyGridView MyGVSource {
        get { return (MyGridView)Session["MyGVSource"]; }
        set { Session["MyGVSource"] = value; }
    }
    public MyGridView MyGVDestination {
        get { return (MyGridView)Session["MyGVDestination"]; }
        set { Session["MyGVDestination"] = value; }
    }
    public List<string> ListSearchResults {
        get { return (List<string>)Session["ListSearchResults"]; }
        set { Session["ListSearchResults"] = value; }
    }
    public List<string> ListSearchedWords {
        get { return (List<string>)Session["ListSearchedWords"]; }
        set { Session["ListSearchedWords"] = value; }
    }
    public bool SearchMode {
        get {
            if (Session["SearchMode"] == null) { SearchMode = false; }
            return (bool)Session["SearchMode"];
        }
        set {
            Session["SearchMode"] = value;
        }
    }
    public MyDataBind MyDataBind {
        get {
            if (Session["myEventDataBind"] == null) { MyDataBind = new MyDataBind(); }
            return (MyDataBind)Session["myEventDataBind"];
        }
        set { Session["myEventDataBind"] = value; }
    }
    public MyEventRename MyEventRename {
        get { return (MyEventRename)Session["MyEventRename"]; }
        set { Session["MyEventRename"] = value; }
    }
    public MyEventArchive MyEventArchive {
        get { return (MyEventArchive)Session["MyEventArchive"]; }
        set { Session["MyEventArchive"] = value; }
    }
    #endregion

    #region Page Load
    // First time loading the Session(Page)
    protected void Page_Load(object sender, EventArgs e) {
        // If page is updated and it's not the first time being loaded in a new browser
        if (!Page.IsPostBack) {
            firstPageLoad();
        }
    }
    private void firstPageLoad() {
        // Set the Current folders to the home folders
        this.SourceCurrentFolder = this.SourceHomeFolder;
        this.DestinationCurrentFolder = this.DestinationHomeFolder;

        // Reset the myGridview variables to the current gridviews
        setMyGVSource();
        setMyGVDestination();

        // Populate both Gridviews for first time loading the webpage
        bindGridview(gvSource, MyGVSource, lblSourceCurrentPath);
        bindGridview(gvDestination, MyGVDestination, lblDestinationCurrentPath);

        MyDataBind = new MyDataBind();
    }
    private void setMyGVSource() {
        this.MyGVSource = new MyGridView();
        MyGVSource.Gridview = gvSource;
        MyGVSource.LabelCurrentFolder = lblSourceCurrentPath;
        MyGVSource.CurrentFolder = this.SourceCurrentFolder;
        MyGVSource.HomeFolder = this.SourceHomeFolder;
        MyGVSource.GridViewName = "gvLocation";
    }
    private void setMyGVDestination() {
        this.MyGVDestination = new MyGridView();
        MyGVDestination.Gridview = gvDestination;
        MyGVDestination.LabelCurrentFolder = lblDestinationCurrentPath;
        MyGVDestination.CurrentFolder = this.DestinationCurrentFolder;
        MyGVDestination.HomeFolder = this.DestinationHomeFolder;
        MyGVDestination.GridViewName = "gvDestination";
    }
    private void setMyEventDataBind() {
        MyDataBind.MyGVSource = MyGVSource;
        MyDataBind.MyGVDestination = MyGVDestination;
    }
    #endregion

    #region PopulateGridViews
    private List<MyFile> populateGridView(MyGridView myGridview, Label lblCurrentFolder) {
        // Get Files & Folders in the CurrentFolder
        myGridview.setCurrentDirectoryInfo();
        if (!SearchMode || (SearchMode && myGridview.GridViewName != "gvDestination")) {
            myGridview.FSItems?.Clear();
            // Update Gridview
            if (!SearchMode) {
                lblCurrentFolder.Text = myGridview.getCurrentFolderLabelText();
            }
            myGridview.updateMyGridviewDataSource();
        }
        searchFunctionVisibility();
        return myGridview.FSItems;
    }

    // Hide or Show search function depending on current folder view
    private void searchFunctionVisibility() {
        bool visible = this.MyGVDestination.getSearchVisibility();
        txtDestSearch.Visible = visible;
        lblDestSearch.Visible = visible;
        btnDestSearch.Visible = visible;
        if (!visible) {
            txtDestSearch.Text = "";
        }
    }

    // Called by ASPX - get file size and determine how to display if item is file
    protected string displayFileSize(long? size) {
        if (size == null)
            return string.Empty;
        else {
            if (size < 1024)
                return string.Format("{0:N0} bytes", size.Value);
            else
                return String.Format("{0:N0} KB", size.Value / 1024);
        }
    }

    // Called by ASPX - when expanding a row in destination folder, this creates a lower level gridview within the current GridviewRow
    public String createNewRowSubGridView(object Name) {
        return String.Format(@"</td></tr><tr id ='tr{0}' style='collapsed-row'>
               <td colspan='100' style='padding:0px; margin:0px;'>", Name);
    }
    #endregion

    protected void gvFiles_RowCommand(object sender, GridViewCommandEventArgs e) {
        if (e.CommandName == "OpenSourceFolder" || e.CommandName == "OpenDestFolder") {
            // When a LinkButton is clicked, the page will refresh the Gridview to show the desired folder
            openLink(e);
        }
    }

    #region LinkButton - Open Folder
    private void openLink(GridViewCommandEventArgs e) {
        MyEventOpenFolder myOpenFolder = new MyEventOpenFolder();
        setMyOpenFolder(myOpenFolder, e);
        // If the Link was clicked within the Location Gridview, Update the Location Gridview
        if (e.CommandName == "OpenSourceFolder") {
            this.SourceCurrentFolder = myOpenFolder.openFolderViaLinkButton("Source");
            bindGridview(gvSource, MyGVSource, lblSourceCurrentPath);
        }
        // If the Link was clicked within the Destination Gridview, Update the Destination Gridview
        else if (e.CommandName == "OpenDestFolder") {
            this.DestinationCurrentFolder = myOpenFolder.openFolderViaLinkButton("Destination");
            // When a linkbutton is clicked, it will always exit the search mode // TODO, what about the back button??
            SearchMode = false;
            bindGridview(gvDestination, MyGVDestination, lblDestinationCurrentPath);
        }
    }

    private void setMyOpenFolder(MyEventOpenFolder open, GridViewCommandEventArgs e) {
        open.MyGVSource = this.MyGVSource;
        open.MyGVDestination = this.MyGVDestination;
        open.SourceCurrentFolder = this.SourceCurrentFolder;
        open.DestinationCurrentFolder = this.DestinationCurrentFolder;
        open.CommandArgument = e.CommandArgument.ToString();
    }
    #endregion

    #region Expand/Collapse
    // When a + sign is clicked, this will expand the Gridview to show the contents within a folder
    // When a - sign is clicked, this will collapse the Gridview to show the top level of the folder
    protected void expandCollapseFolders(object sender, EventArgs e) {
        MyEventExpandCollapse myEvent = new MyEventExpandCollapse();
        setMyEventExpandCollapse(myEvent, sender);
        if (myEvent.GridViewInner.Visible) {
            myEvent.collapseInnerGridView();
        } else {
            myEvent.collapseAllInnerGridViews();
            myEvent.expandInnerGridView();
        }
    }

    private void setMyEventExpandCollapse(MyEventExpandCollapse myEvent, object sender) {
        myEvent.MyGridView = this.MyGVDestination;
        myEvent.GridView = (GridView)(sender as Control).Parent.Parent.Parent.Parent;
        myEvent.GridViewInner = (GridView)(sender as Control).FindControl("gvInsideFolder");
        myEvent.ImgBtnExpand = (ImageButton)(sender as Control).FindControl("expand");
        myEvent.SearchedWords = this.ListSearchedWords;
        myEvent.GridViewRowSelected = (GridViewRow)(sender as Control).Parent.Parent;
        myEvent.SearchMode = this.SearchMode;
        myEvent.DestinationCurrentFolder = this.DestinationCurrentFolder;
        myEvent.DestWebServerFolder = this.DestinationWebServerFolder;
        myEvent.DestinationHomeFolder = this.DestinationHomeFolder;
        myEvent.Response = this.Response;
    }

    #endregion

    #region Search
    // Search Destination Folders by number or keyword
    protected void btnDestSearch_Click(object sender, EventArgs e) {
        // Search box must not be empty to search
        if (txtDestSearch.Text != "") {
            MyEventSearch myEvent = new MyEventSearch();
            setSearch(myEvent);
            myEvent.beginSearch();
            lblDestinationCurrentPath.Text = myEvent.CurrentPathLblText;
            this.SearchMode = myEvent.SearchMode;
            this.ListSearchedWords = myEvent.Words;
            txtDestSearch.Focus();
            bindGridview(gvDestination, MyGVDestination, lblDestinationCurrentPath); // Show results even if no results (blank GridView)
        } else {
            Response.Write("<script>alert('Please enter text in the search box to search');</script>");
        }
    }

    private void setSearch(MyEventSearch search) {
        search.SearchCriteria = txtDestSearch.Text;
        search.MyGridView = MyGVDestination;
        search.DestinationCurrentFolder = DestinationCurrentFolder;
        search.DestinationHomeFolder = DestinationHomeFolder;
        search.Words = ListSearchedWords;
        search.Results = ListSearchResults;
        search.SearchMode = SearchMode;
        search.Response = Response;
    }
    #endregion

    #region Copy
    // Copy selected file to one or more designated locations
    protected void btnCopy_Click(object sender, EventArgs e) {
        MyEventCopy myEvent = new MyEventCopy();
        setMyEventCopy(myEvent);
        myEvent.copy();
        if (myEvent.Error != null) {
            lblError.Text = myEvent.Error;
        }
        if (myEvent.Successful) {
            Response.Write("<script>alert('File(s) successfully copied to the selected location');</script>");
        }
    }

    private void setMyEventCopy(MyEventCopy copy) {
        //if (!lblSourceCurrentPath.Text.Contains(MyGVSource.CurrentFolder)) {
        //        MyGVSource.setCurrentDirectoryInfo(
        //            lblSourceCurrentPath.Text.Substring("Viewing the folder ".Length)
        //            );
        //}
        string lbl = lblDestinationCurrentPath.Text;
        if (!lbl.Contains(MyGVDestination.CurrentFolder)) {
            string newCurrentFolder = "";
            if (!SearchMode) {
                if (!lbl.Contains("search")) {
                    newCurrentFolder = lbl.Substring("Viewing the folder ".Length);
                    newCurrentFolder = Regex.Replace(newCurrentFolder, "<.*?>", String.Empty);
                } else { // not sure if it ever reaches this
                    newCurrentFolder = lbl.Substring(lbl.IndexOf(":"));
                }
            } else { // not sure if it ever reaches this
                newCurrentFolder = lbl.Substring(lbl.IndexOf(":"));
            }
            MyGVDestination.setCurrentDirectoryInfo(newCurrentFolder);
        }

        MyGVSource.Gridview = gvSource;
        MyGVDestination.Gridview = gvDestination;
        copy.MyGVSource = MyGVSource;
        copy.MyGVDestination = MyGVDestination;
        copy.Response = Response;
        copy.DestinationHomeFolder = DestinationHomeFolder;
        copy.DestWebServerFolder = DestinationWebServerFolder;
        copy.DestinationCurrentFolder = DestinationCurrentFolder;
        copy.Page = this.Page;
    }
    #endregion

    #region GridView Event Handlers
    private void bindGridview(GridView gridview, MyGridView myGridView, Label lblCurrentPath) {
        gridview.DataSource = populateGridView(myGridView, lblCurrentPath);
        gridview.DataBind();
        myGridView.Gridview = gridview;
    }

    // On DataBind, the Gridview shown on the interface is updated one row at a time
    protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e) {
        setMyGridView(e, sender);
        MyDataBind.gvRowBinding();
        selectAll.Visible = MyDataBind.CheckBoxVisible;
    }

    private void setMyGridView(GridViewRowEventArgs e, object sender) {
        MyDataBind.Page = Page;
        MyDataBind.GridViewRow = e.Row;
        MyDataBind.GridViewName = (sender as Control).ClientID;
    }

    // Determines which myGridview to utilize; More options can be added to this function if need be
    private MyGridView determineGridview(object sender) {
        GridView gvSender = (GridView)sender;
        if (gvSender.ClientID == "gvSource") {
            MyGVSource.Gridview = gvSource;
            return MyGVSource;
        } else {
            MyGVDestination.Gridview = gvDestination;
            return MyGVDestination;
        }
    }
    #endregion

    #region RadioButton Selection Change
    // Only one RadioButton allowed to be selected at a time (Needed because the RadioButtons aren't located in a central group)
    public void RadioButton_CheckedChanged(object sender, EventArgs e) {
        MyEvent myEvent = new MyEvent();
        myEvent.setMyEvent(sender, MyGVSource, MyGVDestination);
        myEvent.unselectAllRadioButtons(myEvent.MyGridView.Gridview);
        (sender as RadioButton).Checked = true;
    }
    #endregion

    protected void InnerRowCommand(object sender, GridViewCommandEventArgs e) {
        GridView gv = sender as GridView;
        int totalFiles = (sender as GridView).Rows.Count;
        var ee = (sender as GridView).Parent.Parent;
        string folder = (((sender as GridView).Parent.Parent as GridViewRow).FindControl("ltlFileItem") as Literal).Text;
        folder = cleanString(folder);
        string currentFolder = DestinationCurrentFolder + "\\" + folder;
        switch (e.CommandName) {
            case "MoveUp":
            case "MoveDown":
                MyEventChangeFileOrder myEvent = new MyEventChangeFileOrder();
                setMoveFile(myEvent, gv, e);
                if (e.CommandName == "MoveUp") {
                    myEvent.moveUp();
                } else {
                    myEvent.moveDown();
                }
                break;
            case "Rename":
                setRename(gv, Convert.ToInt32(e.CommandArgument));
                break;
            case "Archive":
            case "ArchiveCancel":
            case "ArchiveComfirm":
                archive(gv, e);
                break;
        }
    }

    private void archive(GridView gv, GridViewCommandEventArgs e) {
        MyEventArchive archive = new MyEventArchive();
        if (MyEventArchive == null) {
            setArchive(archive);
        } else archive = MyEventArchive;
        setArchiveGridView(archive, gv, Convert.ToInt32(e.CommandArgument), Response);
        switch (e.CommandName) {
            case "Archive":
                archive.Archive();
                break;
            case "ArchiveCancel":
                archive.ArchiveCancel();
                break;
            case "ArchiveComfirm":
                archive.ArchiveComfirm();
                break;
        }
        MyEventArchive = archive;
    }

    //void dataGridView1_PreviewKeyDown(object sender, GridViewPageEventArgs e) {
    //    if (e.KeyCode == Keys.Enter) {
    //        int i = dataGridView1.CurrentRow.Index;
    //        MessageBox.Show(i.ToString());
    //    }
    //}

    private void setMoveFile(MyEventChangeFileOrder myEvent, GridView gv, GridViewCommandEventArgs e) {
        myEvent.GridViewInner = gv;
        myEvent.GridViewRowSelected = gv.Rows[Convert.ToInt32(e.CommandArgument)];
        myEvent.DestinationCurrentFolder = this.DestinationCurrentFolder;
        myEvent.DestinationHomeFolder = this.DestinationHomeFolder;
        myEvent.DestWebServerFolder = this.DestinationWebServerFolder;
        myEvent.Response = Response;
    }

    private void setArchiveGridView(MyEventArchive archive, GridView gv, int rowIndex, HttpResponse response) {
        archive.GridViewInner = gv;
        archive.GridViewRowSelected = gv.Rows[rowIndex];
        archive.Response = response;
    }

    private void setRename(GridView gv, int rowIndex) {
        MyEventRename rename = new MyEventRename();
        if (MyEventRename != null) {
            rename = MyEventRename;
        }
        rename.GridViewInner = gv;
        rename.GridViewRowSelected = gv.Rows[rowIndex];
        rename.DestinationCurrentFolder = this.DestinationCurrentFolder;
        rename.DestinationHomeFolder = this.DestinationHomeFolder;
        rename.DestWebServerFolder = this.DestinationWebServerFolder;
        rename.Response = Response;
        rename.Rename();
        MyEventRename = rename;
        FocusedID = (gv.Rows[rowIndex].FindControl("txtFileItem") as TextBox).ClientID;
    }
    private void setArchive(MyEventArchive archive) {
        if (MyEventArchive != null) {
            archive = MyEventArchive;
        }
        archive.DestinationCurrentFolder = this.DestinationCurrentFolder;
        archive.DestinationHomeFolder = this.DestinationHomeFolder;
        archive.DestWebServerFolder = this.DestinationWebServerFolder;
        archive.Response = Response;
        MyEventArchive = archive;
    }

    private string cleanString(string s) {
        return s.Substring(s.IndexOf(";") + 1);
    }

    protected void SelectAll_Click(object sender, EventArgs e) {
        foreach (GridViewRow gvr in gvSource.Rows) {
            if (gvr.RowIndex != 0)
                (gvr.FindControl("chkSelect") as CheckBox).Checked = true;
        }
    }
}