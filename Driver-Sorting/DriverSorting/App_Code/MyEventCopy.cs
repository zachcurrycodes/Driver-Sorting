using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web.UI;

public class MyEventCopy : MyEvent {
    public MyEventCopy() {
        this.Successful = false;
        this.ValidSource = false;
        this.ValidDestination = false;
        this.UpperRadioButtonChecked = false;
        this.PathsToCopyToWebServer = new List<string[]>();
    }

    public bool Successful { get; private set; }
    private bool ValidSource { get; set; }
    private bool ValidDestination { get; set; }
    private bool UpperRadioButtonChecked { get; set; }
    private string DestinationFileName { get; set; }
    private List<string[]> PathsToCopyToWebServer { get; set; }
    public Page Page { get; set; }


    public void copy() {
        try {
            if (aFileIsSelectedToCopy()) {
                if (aDestinationIsSelected()) {
                    foreach (GridViewRow gvr in this.MyGVSource.Gridview.Rows) {
                        CheckBox chkSelect = gvr.FindControl("chkSelect") as CheckBox;
                        if (chkSelect.Checked == true) {
                            setCopyMyFile(gvr);
                            continueToFileDestination();
                        }
                    }
                    if (PathsToCopyToWebServer != null) {
                        this.GridViewInner.Visible = true;
                        (this.GridViewRowSelected.FindControl("expand") as ImageButton).ImageUrl = "./images/open.gif";
                        this.ExpandedFolder = Path.GetDirectoryName(this.DestinationFileName);
                        refreshInnerGridView();
                        refreshColumns();
                        showImgButton();
                        moveFilesToWebServer();
                    }
                    //this.Successful = true;
                    unselectAllRadioButtons(MyGVDestination.Gridview);
                    //unselectAllRadioButtons(GridViewInner);
                } else {
                    this.Response.Write("<script>alert('Please select a Destination for the selection');</script>");
                }
            } else {
                this.Response.Write("<script>alert('Please select a File to copy');</script>");
            }
        } catch (Exception e) {
            displayError(e);
            throw;
            //this.Response.Write("<script>alert('Error caught in continueToLowerLevelCheckBoxes() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in continueToFileDestination() " + e;
            //    throw;
            //}
        }
    }

    private void refreshColumns() {
        GridViewInner.Columns[4].Visible = true;
        GridViewInner.Columns[5].Visible = true;
        GridViewInner.Columns[6].Visible = true;
        GridViewInner.Columns[8].Visible = true;
        GridViewInner.Columns[0].Visible = false;
    }

    private void showImgButton() {
        if (UpperRadioButtonChecked) {
            ImageButton imageButton = (ImageButton)this.GridViewRowSelected.FindControl("expand");
            imageButton.Visible = true;
        }
    }

    private void setCopyMyFile(GridViewRow gvr) {
        this.MyFile = new MyFile();
        this.MyFile.Name = (gvr.FindControl("ltlFileItem") as Literal).Text;
        if (this.MyFile.Name == "") {
            this.MyFile.Name = cleanString((gvr.FindControl("lbFolderItem") as LinkButton).Text);
            this.MyFile.IsFolder = true;
        }
        this.MyFile.FullName = this.MyGVSource.CurrentFolder + "\\" + this.MyFile.Name;
        this.MyFile.From = this.MyFile.FullName;
    }

    // TODO optimize
    private void continueToFileDestination() {
        try {
            // Checks per row if visible checkboxes on top level gridview, if not, look for selected checkboxes on lowerlevel gridview
            foreach (GridViewRow gvrSelected in this.MyGVDestination.Gridview.Rows) {
                this.GridViewRowSelected = gvrSelected;
                if (UpperRadioButtonChecked) {
                    RadioButton rbSelect = this.GridViewRowSelected.FindControl("rbSelect") as RadioButton;
                    if (rbSelect.Checked == true) {
                        this.GridViewInner = this.GridViewRowSelected.FindControl("gvInsideFolder") as GridView;
                        setDestinationFileName();
                    }
                } else {
                    continueToLowerLevelRadioButtons();
                }
            }
            moveToIntranet();
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in continueToFileDestination() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in continueToFileDestination() " + e;
            //    throw;
            //}
        }
    }

    private void continueToLowerLevelRadioButtons() {
        try {
            //look for selected checkboxes on lowerlevel gridview
            this.GridViewInner = this.GridViewRowSelected.FindControl("gvInsideFolder") as GridView;
            if (this.GridViewInner.Visible == true) {
                continueIntoLowerGridview();
            }
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in continueToLowerLevelCheckBoxes() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in continueToLowerLevelCheckBoxes() " + e;
            //    throw;
            //}
        }
    }

    private void continueIntoLowerGridview() {
        foreach (GridViewRow gvrSelected in this.GridViewInner.Rows) {
            RadioButton rbSelect = gvrSelected.FindControl("rbSelect") as RadioButton;
            if (rbSelect.Visible == true && rbSelect.Checked == true) {
                string folderNameUpper = (this.GridViewRowSelected.FindControl("lbFolderItem") as LinkButton).Text;
                folderNameUpper = cleanString(folderNameUpper);
                string folderNameLower = (gvrSelected.FindControl("ltlFileItem") as Literal).Text;
                string firstPartOfPath = this.DestinationCurrentFolder + "\\" + folderNameUpper + "\\" + folderNameLower + "\\";
                this.DestinationFileName = firstPartOfPath + setPrefix(firstPartOfPath) + this.MyFile.Name;
            }
        }
    }

    private void setDestinationFileName() {
        try {
            string folderName = (this.GridViewRowSelected.FindControl("lbFolderItem") as LinkButton).Text;
            if (folderName == "") {
                folderName = (this.GridViewRowSelected.FindControl("ltlFileItem") as Literal).Text;
            }
            folderName = cleanString(folderName);
            string firstPartOfPath = this.MyGVDestination.CurrentFolder + "\\" + folderName + "\\";

            this.DestinationFileName = firstPartOfPath + setPrefix(firstPartOfPath) + Path.GetFileName(this.MyFile.Name);
            if (this.DestinationFileName == null) {
                throw new Exception("MyEventCopy.DestinationFileName is null");
            }
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in addToDestinationFiles() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in addToDestinationFiles() " + e;
            //    throw;
            //}
        }
    }

    private string setPrefix(string path) {
        //NetworkDrives.MapDrive("iboxx");                                                                                                          // comment for local testing
        int numberOfChildDirectories = Directory.GetDirectories(path).Length;
        int numberofChildFiles = Directory.GetFiles(path).Length;
        //NetworkDrives.DisconnectDrive("iboxx");                                                                                                   // comment for local testing
        int nextRowNumber = numberOfChildDirectories + numberofChildFiles + 1;
        string newRowNumber = nextRowNumber.ToString();
        if (newRowNumber.Length < 2) {
            newRowNumber = "0" + newRowNumber;
        }
        return newRowNumber + "_";
    }

    private bool aFileIsSelectedToCopy() {
        try {
            foreach (GridViewRow gvr in this.MyGVSource.Gridview.Rows) {
                CheckBox chkSelect = gvr.FindControl("chkSelect") as CheckBox;
                if (chkSelect.Checked == true) {
                    this.ValidSource = true;
                    break;
                }
            }
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in aFileIsSelectedToCopy() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in aFileIsSelectedToCopy() " + e;
            //    throw;
            //}
        }
        return this.ValidSource;
    }

    private bool aDestinationIsSelected() {
        try {
            // First, Check if any of the rows are checked, exit function and return true is checked
            foreach (GridViewRow gvrSelected in this.MyGVDestination.Gridview.Rows) {
                this.GridViewRowSelected = gvrSelected;
                RadioButton rbSelect = this.GridViewRowSelected.FindControl("rbSelect") as RadioButton;
                if (rbSelect.Visible == true && rbSelect.Checked == true) {
                    this.ValidDestination = true;
                    this.UpperRadioButtonChecked = true;
                    return this.ValidDestination;
                }
            }
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in aDestinationIsSelected() Part1 " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in aDestinationIsSelected() part 1 " + e;
            //    throw;
            //}
        }
        try {
            // If no rows are checked, look if any row that is expanded/check the inner gridview
            foreach (GridViewRow gvrSelected in this.MyGVDestination.Gridview.Rows) {
                GridView gvInnerGridview = gvrSelected.FindControl("gvInsideFolder") as GridView;
                if (gvInnerGridview.Visible == true) {
                    foreach (GridViewRow gvrSelected2 in gvInnerGridview.Rows) {
                        RadioButton rbSelect = gvrSelected2.FindControl("rbSelect") as RadioButton;
                        if (rbSelect.Visible == true && rbSelect.Checked == true) {
                            this.ValidDestination = true;
                            return this.ValidDestination;
                        }
                    }
                }
            }
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in aDestinationIsSelected() Part2 " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in aDestinationIsSelected() part 2 " + e;
            //    throw;
            //}
        }
        return this.ValidDestination;
    }

    private void moveToIntranet() { //TODO optimize
        try {
            MyFile mf = new MyFile();
            setMF(mf);
            moveFileToIntranet(mf);
            addFilePathsToListStrinArray(mf);
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in moveToIntranetAndWeb() " + e + "');</script>");
            //Error = "Error caught in moveToIntranetAndWeb() " + e;
        }
    }

    private void moveFileToIntranet(MyFile mf) {
        createDirectoryIfNotExist(mf.DirectoryName);
        createSelfExtractingZip(mf);
    }

    private void createDirectoryIfNotExist(string s) {
        if (!Directory.Exists(s)) {
            Directory.CreateDirectory(s);
        }
    }

    private void createSelfExtractingZip(MyFile mf) {
        try {
            NetworkDrives.MapDrive("iboxx");                                                                          //comment for local testing 3/26
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile()) {
                if (mf.IsFolder) {
                    zip.AddDirectory(mf.From);
                } else {
                    zip.AddFile(mf.From);
                }
                Ionic.Zip.SelfExtractorSaveOptions options = new Ionic.Zip.SelfExtractorSaveOptions();
                options.Flavor = Ionic.Zip.SelfExtractorFlavor.ConsoleApplication;
                zip.SaveSelfExtractor(mf.FullName, options);
            }
            NetworkDrives.DisconnectDrive("iboxx");                                                                   //comment for local testing
        } catch (Exception e) {
            displayError(e);
            throw;
            //if (Error == "") {
            //    Error = "Error caught in createSelfExtractingZip() " + e;
            //    throw;
            //}
            ////ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Message", "<script>alert('Error caught in createSelfExtractingZip() " + e + "');</script>", true);
            ////this.Response.Write("<script>alert('Error caught in createSelfExtractingZip() " + e + "');</script>");
        }
    }

    private void setMF(MyFile mf) {
        try {
            //string partOfPath = cleanString((GridViewRowSelected.FindControl("lbFolderItem") as LinkButton).Text);
            //if (partOfPath == "") {
            //    partOfPath = cleanString((GridViewRowSelected.FindControl("ltlFileItem") as Literal).Text);
            //}
            //mf.FullName = this.DestinationCurrentFolder + "\\" + partOfPath + "\\" + returnInnerRowCountPlusOne() + this.MyFile.Name + ".exe";
            mf.FullName = this.DestinationFileName + ".exe";
            mf.From = this.MyFile.From;
            mf.DirectoryName = Path.GetDirectoryName(mf.FullName);
            mf.WithoutExtension = Path.GetFileNameWithoutExtension(this.MyFile.FullName);
            mf.IsFolder = this.MyFile.IsFolder;
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in setMF() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in setMF() " + e;
            //    throw;
            //}
        }
    }

    private void addFilePathsToListStrinArray(MyFile mf) {
        try {
            string[] twoPaths = new string[2];
            string intranetLocation = mf.FullName;
            setFullPathForWebServer(mf, this.DestWebServerFolder);
            string webServerLocation = mf.FullName;
            twoPaths[0] = intranetLocation;
            twoPaths[1] = webServerLocation;
            this.PathsToCopyToWebServer.Add(twoPaths);
        } catch (Exception e) {
            displayError(e);
            throw;
            ////this.Response.Write("<script>alert('Error caught in moveFileToWebServer() " + e + "');</script>");
            //if (Error == "") {
            //    Error = "Error caught in moveFileToWebServer() " + e;
            //    throw;
            //}
        }
    }

    private void setFullPathForWebServer(MyFile mf, string webServerFolder) {
        mf.FullName = webServerFolder + pruneFullPathForWebServer(mf.FullName);
        mf.DirectoryName = Path.GetDirectoryName(mf.FullName);
        mf.WithoutExtension = Path.GetFileNameWithoutExtension(mf.FullName);
    }

    private string pruneFullPathForWebServer(string fullName) {
        return fullName.Substring(this.DestinationHomeFolder.Length + 1);
    }

    private void moveFileToWebServer2(MyFile mf, string intranetLocation) {
        createDirectoryIfNotExist(mf.DirectoryName);
        if (File.Exists(mf.FullName)) {
            File.Delete(mf.FullName);
        }
        File.Copy(intranetLocation, mf.FullName);
    }

    private void moveFilesToWebServer() {
        try {
            NetworkDrives.MapDrive("web");
            foreach (string[] twoPaths in PathsToCopyToWebServer) {
                string intranetLocation = twoPaths[0];
                string webServerLocation = twoPaths[1];
                string directoryName = Path.GetDirectoryName(webServerLocation);
                createDirectoryIfNotExist(directoryName);
                File.Copy(intranetLocation, webServerLocation, true);
            }
            NetworkDrives.DisconnectDrive("web");
            this.Successful = true;
        } catch (Exception e) {
            displayError(e);
            throw;
            //if (Error == "") {
            //    Error = Error = "Error caught in moveFileToWebServer() " + e;
            //    throw;
            //}
        }
    }
}