using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public class MyEventArchive : MyEvent {
    public MyEventArchive() { }

    private static string ARCHIVEPATHHOME = @"\\boxx\Engineering\Drivers\Archived Drivers\";
    private bool ArchiveMode { get; set; }
    private string HoldExpandedFolderPath { get; set; }


    public void Archive() {
        showColumnsBeginArchive();
    }

    public void ArchiveCancel() {
        hideColumnsEndArchive();
    }

    public void ArchiveComfirm() {
        moveLocalFileToArchive();
        deleteFromWebServer();
        this.ExpandedFolder = this.HoldExpandedFolderPath;
        //refreshInnerGridView();
        orderDriverPrefixes2();
        hideColumnsEndArchive();
        refreshInnerGridView();
    }

    private void showColumnsBeginArchive() {
        this.ArchiveMode = true;
        this.GridViewInner.Columns[4].Visible = false;
        this.GridViewInner.Columns[5].Visible = false;
        this.GridViewInner.Columns[6].Visible = false;
        this.GridViewInner.Columns[7].Visible = false;
        this.GridViewInner.Columns[8].Visible = false;
        this.GridViewInner.Columns[9].Visible = true;
        this.GridViewInner.Columns[10].Visible = true;
        this.GridViewRowSelected.FindControl("ArchiveCancel").Visible = true;
        this.GridViewRowSelected.FindControl("ArchiveComfirm").Visible = true;
    }

    private void hideColumnsEndArchive() {
        this.ArchiveMode = false;
        this.GridViewInner.Columns[4].Visible = true;
        this.GridViewInner.Columns[5].Visible = true;
        this.GridViewInner.Columns[6].Visible = true;
        this.GridViewInner.Columns[7].Visible = true;
        this.GridViewInner.Columns[8].Visible = true;
        this.GridViewInner.Columns[9].Visible = false;
        this.GridViewInner.Columns[10].Visible = false;
        this.GridViewRowSelected.FindControl("ArchiveCancel").Visible = false;
        this.GridViewRowSelected.FindControl("ArchiveComfirm").Visible = false;
    }

    private void moveLocalFileToArchive() {
        GridViewRow newGVR = this.GridViewRowSelected.Parent.Parent.Parent.Parent as GridViewRow;
        FolderName = (newGVR.FindControl("lbFolderItem") as LinkButton).Text;
        FolderName = cleanString(FolderName);
        this.OriginalFileName = (this.GridViewRowSelected.FindControl("ltlFileItem") as Literal).Text;
        this.OriginalFileName = cleanString(this.OriginalFileName);
        this.ExpandedFolder = this.DestinationCurrentFolder + "\\" + FolderName + "\\";
        this.HoldExpandedFolderPath = this.ExpandedFolder;
        string originalPath = ExpandedFolder + OriginalFileName;
        string partialPath = originalPath.Substring(DestinationHomeFolder.Length + 1);
        partialPath = partialPath.Substring(0, partialPath.Length - 4);
        string newPath = ARCHIVEPATHHOME + partialPath + " " + DateTime.Now.ToString("MM-dd-yyy") + ".exe";
        try {
            string directory = Path.GetDirectoryName(newPath);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            try {
                File.Move(originalPath, newPath);
            } catch (Exception e) {
                this.Response.Write("<script>alert('Unable to archive file. Reason: " + e + "');</script>");
            }
        } catch (Exception e) {
            this.Response.Write("<script>alert('Unable to create directory. Reason: " + e + "');</script>");
        }
    }

    private void deleteFromWebServer() {
        string currentFolder = this.DestinationCurrentFolder.Substring(this.DestinationHomeFolder.Length + 1);
        this.ExpandedFolder = this.DestWebServerFolder + currentFolder + "\\" + FolderName + "\\";
        string filePathToDelete = this.ExpandedFolder + OriginalFileName;
        NetworkDrives.MapDrive("web");
        try {
            File.Delete(filePathToDelete);
        } catch (Exception e) {
            this.Response.Write("<script>alert('Unable to renameFile. Reason: " + e + "');</script>");
        }
        NetworkDrives.DisconnectDrive("web");
    }


    private void orderDriverPrefixes2() {
        refreshInnerGridView();
        if (DestinationCurrentFolder != this.DestinationHomeFolder && Path.GetDirectoryName(DestinationCurrentFolder) != this.DestinationHomeFolder) {
            bool nameChange = false;
            foreach (GridViewRow gvr in GridViewInner.Rows) {
                this.ExpandedFolder = this.HoldExpandedFolderPath;
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
                    renameFile();
                    //this.renameLocal(false);
                    this.renameOnWebServer(); //TODO when in test.boxx.com this can fail due to files not existing
                    nameChange = true;
                }
            }
            if (nameChange) {
                refreshInnerGridView();
            }
        }
    }
}