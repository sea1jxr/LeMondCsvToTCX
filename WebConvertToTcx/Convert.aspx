<%@ Page Title="Convert To TCX application" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Convert.aspx.cs" Inherits="WebConvertToTcx.Convert" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent" >
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<LayoutTemplate>
    <span class="failureNotification">
        <asp:Literal ID="FailureText" runat="server"></asp:Literal>
    </span>
    <div class="accountInfo">
        <fieldset class="login">
            <div class="upload">Lap 1: <asp:FileUpload ID="Lap1File" runat="server" class="textEntry"/></div>
            <div class="upload">Lap 2: <asp:FileUpload ID="Lap2File" runat="server" class="textEntry"/></div>
            <div class="upload">Lap 3: <asp:FileUpload ID="Lap3File" runat="server" class="textEntry"/></div>
            <div class="submitButton">
                <asp:Button ID="Button2" runat="server" Text="Convert" OnClick="Convert_Click" class="flatButton"/>
            </div>
        </fieldset>
    </div>
</LayoutTemplate>
</asp:Content>

