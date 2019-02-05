<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" MaintainScrollPositionOnPostback="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Sort Driver</title>
  <link rel="stylesheet" runat="server" media="screen" href="~/Css/default.css" />
  <style>
    /* Center the loader */
    #loader {
      position: absolute;
      left: 50%;
      top: 50%;
      z-index: 1;
      width: 150px;
      height: 150px;
      margin: -75px 0 0 -75px;
      border: 16px solid #450084;
      border-radius: 50%;
      border-top: 16px solid #AD9C65;
      width: 120px;
      height: 120px;
      -webkit-animation: spin 2s linear infinite;
      animation: spin 2s linear infinite;
      /*display:none;*/
      visibility: hidden;
    }

    @-webkit-keyframes spin {
      0% {
        -webkit-transform: rotate(0deg);
      }

      100% {
        -webkit-transform: rotate(360deg);
      }
    }

    @keyframes spin {
      0% {
        transform: rotate(0deg);
      }

      100% {
        transform: rotate(360deg);
      }
    }

    /* Add animation to "page content" */
    .animate-bottom {
      position: relative;
      -webkit-animation-name: animatebottom;
      -webkit-animation-duration: 1s;
      animation-name: animatebottom;
      animation-duration: 1s
    }

    @-webkit-keyframes animatebottom {
      from {
        bottom: -100px;
        opacity: 0
      }

      to {
        bottom: 0px;
        opacity: 1
      }
    }

    @keyframes animatebottom {
      from {
        bottom: -100px;
        opacity: 0
      }

      to {
        bottom: 0;
        opacity: 1
      }
    }

    #myDiv {
      display: none;
      text-align: center;
    }
  </style>
  <script>
    function sure() {
      var comfirm = confirm('Please confirm to copy file(s) \n\nPlease Wait for confirmation while files are copied');
      if (comfirm) {
        document.getElementById("loader").style.visibility = "visible";
      }
      return comfirm;
    }

    function loading() {
      document.getElementById("loader").style.visibility = "visible";
    }

    function submit() {
      var a = document.activeElement;
      //if element is nothing, submit copy button
      //if element is a radio button, submit copy button
      //if element is a checkbox, submit copy button

      //if element is a textbox, submit done/rename button
    }

    function EnterKeyFilter() {
      // If key pressed was Enter key
      if (window.event.keyCode == 13) {
        // HTML id contains txtFileItem and if the textbox is visible
        if (<%=FocusedID.Contains("txtFileItem").ToString().ToLower() %> && document.getElementById('<%= FocusedID%>') !== null) {
          var txtFileItem = document.getElementById('<%= FocusedID%>');
          // Get btnDone on the same GridViewRow
          var btnDone = document.getElementById(txtFileItem.parentNode.parentNode.children[3].firstElementChild.id)
          btnDone.click();
        }

        // Blocks any unwanted action taken by pressing Enter
        event.returnValue = false;
        event.cancel = true;
      }
    }
  </script>
</head>
<body onkeydown="javascript:EnterKeyFilter();">
  <form id="form1" runat="server">
    <div>
      <div id="loader" runat="server"></div>
    </div>
    <div>
      <br />
      <%--<asp:Button ID="btnCopy" runat="server" Text="Copy Files" OnClick="btnCopy_Click" OnClientClick="return sure();" />--%>
      <asp:Button ID="btnCopy" runat="server" Text="Copy Files" OnClick="btnCopy_Click" UseSubmitBehavior="false" OnClientClick="loading();" />
      <%--<asp:Button ID="btnDelete" runat="server" Text="Delete" class="floatRight"/>--%>
    </div>
    <div class="split floatLeft">
      <div>
        <br />
      </div>
      <asp:Label ID="LocationTitle" runat="server" Text="File Location" CssClass="locationHeader" />
      <br />
      <asp:Label ID="lblSourceCurrentPath" runat="server" Text="lblCurrentPath">
      </asp:Label>
      <asp:GridView
        ID="gvSource" runat="server" AutoGenerateColumns="false" EmptyDataText="No files to show" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowCommand="gvFiles_RowCommand" OnRowDataBound="gvFiles_RowDataBound">
        <Columns>
          <asp:TemplateField HeaderText="Select" HeaderStyle-HorizontalAlign="Left">
            <ItemTemplate>
              <%--<asp:RadioButton ID="rbSelect" runat="server" Visible="false" OnCheckedChanged="RadioButton_CheckedChanged" AutoPostBack="true" />--%>
              <asp:CheckBox ID="chkSelect" runat="server" Visible="false" AutoPostBack="true" />
            </ItemTemplate>
          </asp:TemplateField>
          <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-HorizontalAlign="Left">
            <ItemTemplate>
              <asp:LinkButton runat="server" ID="lbFolderItem" CommandName="OpenSourceFolder" CommandArgument='<%# Eval("Name") %>' CssClass="linkColor" Font-Underline="false" OnClientClick="loading();" />
              <asp:Literal runat="server" ID="ltlFileItem" />
            </ItemTemplate>
          </asp:TemplateField>
          <asp:BoundField DataField="FileSystemType" HeaderText="Type" SortExpression="FileSystemType" HeaderStyle-HorizontalAlign="Left" />
          <asp:BoundField DataField="LastWriteTime" HeaderText="Date Modified" SortExpression="LastWriteTime" HeaderStyle-HorizontalAlign="Left" />
          <asp:TemplateField HeaderText="Size" SortExpression="Size" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Left">
            <ItemTemplate>
              <%# displayFileSize((long?) Eval("Size")) %>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
          </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="headerStyle" />
        <RowStyle CssClass="rowColor" />
        <AlternatingRowStyle CssClass="altRowColor" />
      </asp:GridView>
      <br />
      <br />
      <br />
      <asp:Button ID="selectAll" runat="server" Text="Select All CheckBoxes" OnClick="SelectAll_Click" Visible="false" />
      <div>
        <asp:Label ID="lblError" runat="server"></asp:Label>
      </div>
    </div>

    <%--<asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />--%>
    <div>
      <div class="split">
        <div>
          <%--<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" style="height: 26px" Text="Button" />--%>
          <br />
          <asp:Label ID="lblDestSearch" runat="server" Text="search for:" />
          <asp:TextBox ID="txtDestSearch" runat="server" onFocus="this.select()" />
          <asp:Button ID="btnDestSearch" runat="server" Text="Search" OnClick="btnDestSearch_Click" />
        </div>
        <div>
          <asp:Label ID="lblDestinationTitle" runat="server" Text="Destination Folder" CssClass="locationHeader" />
          <br />
          <asp:Label ID="lblDestinationCurrentPath" runat="server" Text="lblCurrentPath" />
          <asp:GridView ID="gvDestination" runat="server" AutoGenerateColumns="false" EmptyDataText="No files to show" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowCommand="gvFiles_RowCommand" OnRowDataBound="gvFiles_RowDataBound">
            <Columns>
              <asp:TemplateField>
                <ItemTemplate>
                  <asp:ImageButton ID="expand" runat="server" ImageUrl="./images/closed.gif" OnClick="expandCollapseFolders" Visible="false" OnClientClick="loading();" />
                </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                  <asp:RadioButton ID="rbSelect" runat="server" Visible="false" OnCheckedChanged="RadioButton_CheckedChanged" AutoPostBack="true" />
                  <%--<asp:CheckBox ID="chkSelect" runat="server" Visible="false" />--%>
                </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                  <asp:LinkButton runat="server" ID="lbFolderItem" CommandName="OpenDestFolder" CommandArgument='<%# Eval("Name") %>' CssClass="linkColor" Font-Underline="false" OnClientClick="loading();" />
                  <asp:Literal runat="server" ID="ltlFileItem" />
                </ItemTemplate>
              </asp:TemplateField>
              <asp:BoundField DataField="FileSystemType" HeaderText="Type" SortExpression="FileSystemType" HeaderStyle-HorizontalAlign="Left" />
              <asp:BoundField DataField="LastWriteTime" HeaderText="Date Modified" SortExpression="LastWriteTime" HeaderStyle-HorizontalAlign="Left" />

              <asp:TemplateField>
                <ItemStyle HorizontalAlign="Right" />
                <ItemTemplate>
                  <%# createNewRowSubGridView(Eval("Name")) %>
                  <asp:GridView ID="gvInsideFolder" runat="server" Width="100%" GridLines="None" AutoGenerateColumns="false" HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0" Visible="false" OnRowCommand="InnerRowCommand">
                    <RowStyle CssClass="rowColor2" />
                    <AlternatingRowStyle CssClass="altRowColor2" />
                    <Columns>
                      <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                          <asp:RadioButton ID="rbSelect" runat="server" Visible="false" OnCheckedChanged="RadioButton_CheckedChanged" AutoPostBack="true" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <%--<asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" />--%>
                      <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                          <asp:Literal runat="server" ID="ltlFileItem" />
                          <asp:TextBox ID="txtFileItem" runat="server" Visible="false" OnClientClick="submit();" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:BoundField DataField="FileSystemType" HeaderText="Type" SortExpression="FileSystemType" HeaderStyle-HorizontalAlign="Left" />
                      <asp:BoundField DataField="LastWriteTime" HeaderText="Date Modified" SortExpression="LastWriteTime" HeaderStyle-HorizontalAlign="Left" />
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="Up" runat="server" CommandName="MoveUp" Text="^" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="loading();" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="Down" runat="server" CommandName="MoveDown" Text="v" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="loading();" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="Rename" runat="server" CommandName="Rename" Text="Rename" CommandArgument="<%# Container.DataItemIndex %>" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="Done" runat="server" CommandName="Rename" Text="Done" Visible="false" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="loading();" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="Archive" runat="server" CommandName="Archive" Text="Archive" CommandArgument="<%# Container.DataItemIndex %>" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="ArchiveComfirm" runat="server" CommandName="ArchiveComfirm" Text="Comfirm Archive" Visible="false" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="loading();" />
                        </ItemTemplate>
                      </asp:TemplateField>
                      <asp:TemplateField Visible="false">
                        <ItemTemplate>
                          <asp:Button ID="ArchiveCancel" runat="server" CommandName="ArchiveCancel" Text="Cancel" Visible="false" CommandArgument="<%# Container.DataItemIndex %>" OnClientClick="loading();" />
                        </ItemTemplate>
                      </asp:TemplateField>
                    </Columns>

                  </asp:GridView>
                </ItemTemplate>
              </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="headerStyle" />
            <RowStyle CssClass="rowColor" />
            <AlternatingRowStyle CssClass="altRowColor" />
          </asp:GridView>
        </div>
      </div>
    </div>

  </form>
</body>
</html>
