using System;
using System.Web.UI.WebControls;

public class MyEventChangeFileOrder : MyEventRename {
    public MyEventChangeFileOrder() { }

    private string MovingUp { get; set; }
    private string MovingDown { get; set; }
    private string MovingUpAfter { get; set; }
    private string MovingDownAfter { get; set; }

    public void moveUp() {
        Literal movingUp = (this.GridViewRowSelected.FindControl("ltlFileItem") as Literal);
        Literal movingDown = (rowIndexMinusOne().FindControl("ltlFileItem") as Literal);
        assignForRename(movingUp.Text, movingDown.Text);
    }

    public void moveDown() {
        Literal movingDown = (this.GridViewRowSelected.FindControl("ltlFileItem") as Literal);
        Literal movingUp = (rowIndexPlusOne().FindControl("ltlFileItem") as Literal);
        assignForRename(movingUp.Text, movingDown.Text);
    }

    private void assignForRename(string movingUp, string movingDown) {
        this.MovingUp = movingUp;
        this.MovingDown = movingDown;
        swapFileName("up");
        rename(this.MovingUp, this.MovingUpAfter);
        rename(this.MovingDown, this.MovingDownAfter);
        refreshInnerGridView();
    }

    private void rename(string originalFileName, string newFileName) {
        this.OriginalFileName = originalFileName;
        this.NewFileName = newFileName;
        renameLocal(false);
        renameOnWebServer();
    }

    private void swapFileName(string argument) {
        string transferName = this.MovingUp;
        this.Prefix = this.MovingUp.Substring(0, 2);
        this.MovingDownAfter = Prefix + this.MovingDown.Substring(2);
        makeTwoDigitPrefix(argument);
        this.MovingUpAfter = Prefix + transferName.Substring(3);
    }

    private void makeTwoDigitPrefix(string argument) {
        int prefixInt = Convert.ToInt32(this.Prefix);
        if (argument == "up") {
            prefixInt--;
        }
        if (argument == "down") {
            prefixInt++;
        }
        Prefix = prefixInt.ToString();
        if (Prefix.Length < 2) {
            Prefix = "0" + Prefix;
        }
        Prefix = Prefix + "_";
    }

    private GridViewRow rowIndexPlusOne() {
        return this.GridViewInner.Rows[this.GridViewRowSelected.RowIndex + 1];
    }
    private GridViewRow rowIndexMinusOne() {
        return this.GridViewInner.Rows[this.GridViewRowSelected.RowIndex - 1];
    }
}