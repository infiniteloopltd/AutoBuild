using System;
using System.IO;
using GitSharp;
using GitSharp.Commands;
using System.IO.Compression;
// Install-Package GitSharp

public partial class github : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var tempFolder = GetTemporaryDirectory();
        Response.Write("Cloning into :" + tempFolder + "<br>");
        Git.Clone(new CloneCommand {
            Source = "https://github.com/infiniteloopltd/AutoBuild.git",
            GitDirectory = tempFolder,
        });
        Response.Write("Cloned<br>");        
        var zipFile = tempFolder + ".zip";

        Response.Write("Zipping / unzipping <br>");
        ZipFile.CreateFromDirectory(tempFolder, zipFile);
        ZipFile.ExtractToDirectory(zipFile, @"E:\wwwroot\autobuild.webtropy.com\");
        File.Delete(zipFile);
    }

    public string GetTemporaryDirectory()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }
}