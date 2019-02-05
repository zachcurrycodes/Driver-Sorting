using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class MyDataBind : MyGridView {
    public MyDataBind() {
        this.MyGVSource = new MyGridView();
        this.MyGVDestination = new MyGridView();
        this.CheckBoxVisible = false;
    }

    public MyGridView MyGVSource { private get; set; }
    public MyGridView MyGVDestination { private get; set; }
    public bool CheckBoxVisible { get; private set; }

    // On Row Bind, update passed row
    public void gvRowBinding() {
        // Must be a DataRow to proceed
        if (this.GridViewRow.RowType == DataControlRowType.DataRow) {
            this.MyFile = this.GridViewRow.DataItem as MyFile;
            if (
                //this.MyFile.IsFolder && 
                this.MyFile.Name != "..") {
                if (this.GridViewName == "gvSource") {
                    displayCheckBox();                                                                                    // TODO need to add more rules besides MSI
                } else if (this.GridViewName == "gvDestination") {
                    displayRadioButton();
                }
            }
            displayRowItem();
        }
    }

    private void displayRowItem() {
        // If directory has a directory child, display link as a linkButton, otherwise as literal
        displayItemAsLinkOrLiteral();
        if (this.GridViewName == "gvDestination") {
            // If Item's Name is ".." do not add an expanding ImageButton
            if (this.MyFile.Name != ".." && MyFile.IsFolder && this.MyFile.getChildren() > 0) {
                ImageButton imgbtnExpand = (ImageButton)GridViewRow.Cells[0].FindControl("expand");
                imgbtnExpand.Visible = true;
            }
        }
    }

    // If directory has a directory child, display link as a linkButton, otherwise as literal
    private void displayItemAsLinkOrLiteral() {
        if (this.MyFile.IsFolder) {
        //NetworkDrives.MapDrive("iboxx");
            if (Directory.GetDirectories(this.MyFile.FullName).Length != 0 
                || Directory.GetFiles(this.MyFile.FullName).Length != 0) {
                LinkButton lbFolderItem = GridViewRow.FindControl("lbFolderItem") as LinkButton;
                lbFolderItem.Text = string.Format(@"<img src=""{0}"" alt="""" />&nbsp;{1}", this.Page.ResolveClientUrl("./Images/folder.png"), this.MyFile.Name);
            } else {
                // Display Item as literal, add selection to item (type determined by GridView Name)
                Literal ltlFileItem = GridViewRow.FindControl("ltlFileItem") as Literal;
                ltlFileItem.Text = string.Format(@"<img src=""{0}"" alt="""" />&nbsp;{1}", this.Page.ResolveClientUrl("./Images/folder.png"), this.MyFile.Name);
            }
        //NetworkDrives.DisconnectDrive("iboxx");
        } else {
            Literal ltlFileItem = GridViewRow.FindControl("ltlFileItem") as Literal;
            ltlFileItem.Text = this.MyFile.Name;
        }
    }

    #region Source Display Checkbox Rules
    private void displayCheckBox() {
        if (determineToDisplayCheckBox()) {
            CheckBox chkSelect = GridViewRow.FindControl("chkSelect") as CheckBox;
            chkSelect.Visible = true;
            CheckBoxVisible = true;
        }
    }
    private bool determineToDisplayCheckBox() {
        return findInPathMSI();
        //|| findPathASRock();
    }

    private bool findPathASRock() {
        if (Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)))) != null) {
            return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)))).Contains("ASRock");
        } else {
            return false;
        }
    }

    private bool findInPathMSI() {
        if (Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)) != null) {
        return Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)).ToUpper().Contains("SYSTEM")
            || Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)).ToUpper().Contains("DRIVERS")
            || Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)).ToUpper().Contains("BIOS");
        } else {
            return false;
        }
        //return Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)))) == "Current";

    }
    private bool findPathClevo() {
        if (Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)))) != null) {
            return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)))).Contains("ASRock");
        } else {
            return false;
        }
    }
    #endregion

    #region Destination Display RadioButton Rules
    private void displayRadioButton() {
        if (this.MyFile.IsFolder && determineToDisplayRadioButton()) {
            RadioButton rbSelect = GridViewRow.FindControl("rbSelect") as RadioButton;
            rbSelect.Visible = true;
        }
    }
    private bool determineToDisplayRadioButton() {
        return findInPathBaseBuilds()
            || findInPathLaptopParts()
            || findInPathMiscPeripherals()
            || findInPathMotherboards()
            || findInPathNetworkControllers()
            || findInPathSoundCards()
            || findInPathStorageControllers()
            || findInPathVideoCards();
    }
    private string getDriverDirectoryDirectory() {
        return Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(this.MyFile.FullName)));
    }
    private bool findInPathBaseBuilds() {
        return getDriverDirectoryDirectory() == "Base Builds";
    }
    private bool findInPathLaptopParts() {
        return getDriverDirectoryDirectory() == "Laptop Parts";
    }
    private bool findInPathMiscPeripherals() {
        return getDriverDirectoryDirectory() == "Misc. Peripherals";
    }
    private bool findInPathMotherboards() {
        return getDriverDirectoryDirectory() == "Motherboards";
    }
    private bool findInPathNetworkControllers() {
        return getDriverDirectoryDirectory() == "Network Controllers";
    }
    private bool findInPathSoundCards() {
        return getDriverDirectoryDirectory() == "Sound Cards";
    }
    private bool findInPathStorageControllers() {
        return getDriverDirectoryDirectory() == "Storage Controllers";
    }
    private bool findInPathVideoCards() {
        return getDriverDirectoryDirectory() == "Video Cards";
    }
    #endregion
}