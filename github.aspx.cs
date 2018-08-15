using System;
using System.IO;
using GitSharp;
using GitSharp.Commands;
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

        CopyFolder(new DirectoryInfo(tempFolder), @"e:\wwwroot\autobuild.webtropy.com\");
        Response.Write("Copied");

        // To Do, delete.
    }

    public bool CopyFolder( DirectoryInfo source, string destination)
    {
        try
        {
            foreach (string dirPath in Directory.GetDirectories(source.FullName))
            {
                var newDirPath = dirPath.Replace(source.FullName, destination);
                Directory.CreateDirectory(newDirPath);
                CopyFolder(new DirectoryInfo(dirPath),newDirPath);
            }
            //Copy all the files & Replaces any files with the same name
            foreach (string filePath in Directory.GetFiles(source.FullName))
            {
                File.Copy(filePath, filePath.Replace(source.FullName, destination), true);
            }
            return true;
        }
        catch (IOException exp)
        {
            return false;
        }
    }

    public string GetTemporaryDirectory()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }
}