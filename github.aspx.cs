using System;
using GitSharp;
// Install-Package GitSharp

public partial class github : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Git.Clone("https://github.com/infiniteloopltd/AutoBuild.git", @"E:\wwwroot\autobuild.webtropy.com\");  
    }
}