using System;
using LibGit2Sharp;

// Install-Package LibGit2Sharp -Version 0.24

public partial class github : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Repository.Clone("https://github.com/infiniteloopltd/AutoBuild.git", @"E:\wwwroot\autobuild.webtropy.com\", null);  
    }
}