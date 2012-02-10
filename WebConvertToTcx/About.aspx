<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="WebConvertToTcx.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        About
    </h2>
    <p>
        This application ConvertToTcx will convert CompuTrainer .3DP files, and Lemond GForce and Revolution .CSV files to .TCX for upload to sites like <a href="http://www.strava.com">Strava</a> and <a href="http://connect.garmin.com">Garmin Connect</a>.
        <br />
        <br />
        The app was authored by <a href="mailto:converttotcx@reedjrs.com">Jeff Reed</a> and is available as open source on Github <a href="https://github.com/sea1jxr/LeMondCsvToTCX">here</a>.  It started out as a LeMond only converter and has morphed into more so please excuse the name.  Please contact me if you have any questions, comments and\or suggestions.
    </p>
</asp:Content>
