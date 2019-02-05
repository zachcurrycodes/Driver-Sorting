using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

public class MyEventRename : MyEvent {
    public MyEventRename() {
        this.RenameMode = false;
    }

    private bool RenameMode { get; set; }

    public void Rename() {
        if (RenameMode != true) {
            setTextBoxText();
        } else {
            if (ValidateNewName()) {
                finshRename();
            } else {
                string invalidCharacters = @"\\ / : * ? "" < > |";
                this.Response.Write("<script>alert('Invalid Character in fileName " + invalidCharacters + "');</script>");
                (this.GridViewRowSelected.FindControl("txtFileItem") as TextBox).Focus();
            }
        }
    }

    private bool ValidateNewName() {
        string text = (this.GridViewRowSelected.FindControl("txtFileItem") as TextBox).Text;
        return !(text.Contains(@"\") ||
            text.Contains(@"/") ||
            text.Contains(@":") ||
            text.Contains(@"*") ||
            text.Contains(@"?") ||
            text.Contains(@"""") ||
            text.Contains(@"<") ||
            text.Contains(@">") ||
            text.Contains("|"));
    }

    private void setTextBoxText() {
        showColumnsBeginRename();
        trimPreviousName();
        (this.GridViewRowSelected.FindControl("txtFileItem") as TextBox).Text = this.NewFileName;
        (this.GridViewRowSelected.FindControl("txtFileItem") as TextBox).Focus();
    }

    private void finshRename() {
        NewFileName = (this.GridViewRowSelected.FindControl("txtFileItem") as TextBox).Text;
        hideColumnsFinishRename();
        addToNewName();
        if (NewFileName != OriginalFileName) {
            renameLocal(false);
            refreshInnerGridView();
            renameOnWebServer();
        }
    }

    private void addToNewName() {
        if (Prefix == null) {
            int rowCount = this.GridViewInner.Rows.Count;
            Prefix = rowCount + "_";
        }
        this.NewFileName = this.Prefix + this.NewFileName + ".exe";
    }

    private void showColumnsBeginRename() {
        this.RenameMode = true;
        this.GridViewInner.Columns[4].Visible = false;
        this.GridViewInner.Columns[5].Visible = false;
        this.GridViewInner.Columns[6].Visible = false;
        this.GridViewInner.Columns[7].Visible = true;
        this.GridViewInner.Columns[8].Visible = false;
        this.GridViewRowSelected.FindControl("Done").Visible = true;
        this.GridViewRowSelected.FindControl("ltlFileItem").Visible = false;
        this.GridViewRowSelected.FindControl("txtFileItem").Visible = true;
    }

    private void hideColumnsFinishRename() {
        RenameMode = false;
        this.GridViewInner.Columns[4].Visible = true;
        this.GridViewInner.Columns[5].Visible = true;
        this.GridViewInner.Columns[6].Visible = true;
        this.GridViewInner.Columns[7].Visible = false;
        this.GridViewInner.Columns[8].Visible = true;
        this.GridViewRowSelected.FindControl("Done").Visible = false;
        this.GridViewRowSelected.FindControl("txtFileItem").Visible = false;
        this.GridViewRowSelected.FindControl("ltlFileItem").Visible = true;
    }

    private void trimPreviousName() {
        this.OriginalFileName = (this.GridViewRowSelected.FindControl("ltlFileItem") as Literal).Text;
        this.Prefix = OriginalFileName.Substring(0, 3);
        this.NewFileName = OriginalFileName.Substring(0, OriginalFileName.Length - 4);
        int ingnoreMe = 0;
        if (int.TryParse(Prefix.Substring(0, 2), out ingnoreMe)) {
            this.NewFileName = NewFileName.Substring(3);
        }
    }
}