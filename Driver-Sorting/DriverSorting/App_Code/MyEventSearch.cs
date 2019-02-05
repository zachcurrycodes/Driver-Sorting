using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

public class MyEventSearch : MyEvent {
    public MyEventSearch() {
        this.Results = new List<string>();
    }

    public List<string> Results { private get; set; }
    public string SearchCriteria { private get; set; }
    public string CurrentPathLblText { get; private set; }

    public void beginSearch() {
        // Split the search string into an array using a " "(space) as a delimiter
        // Convert the array to a list and assign to List<string>listSearchedWords
        // Search for all items contained within the DesinationCurrentFolder and subFolders and append to the List<string>allDirectories
        // Search each path(item) found in allDirectories for a searched number or keyword and if found, then add to the List<string>listSearchResults
        this.Words = this.SearchCriteria.Split().ToList();
        // NetworkDrives.MapDrive("iboxx");                                                                                                                                     //uncomment 3/23
        List<string> allDirectories = Directory.GetDirectories(DestinationCurrentFolder, "*.*", SearchOption.AllDirectories).ToList();
        //NetworkDrives.DisconnectDrive("iboxx");                                                                                                                               //uncomment3/23
        this.Results = this.Words.SelectMany(x => allDirectories.FindAll(y => y.ToUpper().Contains(x.ToUpper())).ToArray()).ToList();

        // If Search Results contain atleast one folder
        if (this.Results.Count > 0) {
            contineSearch();
        }
        // If no results are found for the search, display message on the label, focus on the text box and highlight the contents
        else {
            this.Response.Write("<script>alert('No files or folders found within current folder using the given search criteria');</script>");
        }
    }

    private void contineSearch() {
        // Clear myGridview.FSItems and add ".." Parent Folder LinkButton option
        // Select all distinct Folders found by the search criteria
        // Alphabetize the results
        // Add found results to myGridview.FSItems
        this.MyGridView.updateMyGridviewDataSource();
        List<string> lstDistinctDirectories = Results.Select(x => Path.GetDirectoryName(x)).Where(x => Path.GetDirectoryName(Path.GetDirectoryName(x)).Equals(DestinationHomeFolder)).Distinct().ToList();
        lstDistinctDirectories.Sort();
        this.MyGridView.FSItems = lstDistinctDirectories.Select(x => new MyFile(new DirectoryInfo(x))).ToList();

        // Set searchMode true for display puposes
        // Set the DataSource of the Gridview to myGridview.FSItems and update the webpage display of the Gridview
        // Update the label to state "Viewing the search 90823 90825 64-Bit in the current folder: \\boxx\Engineering\Drivers\Laptop Parts"
        this.SearchMode = true;
        CurrentPathLblText = "Viewing the search <b>" + this.SearchCriteria + "</b> " + "in the current folder: " + DestinationCurrentFolder;
    }
}