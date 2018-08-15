using Octokit; // Install-Package Octokit 
using System;
using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.GZip; // Install-Package SharpZipLib
using ICSharpCode.SharpZipLib.Tar; // Install-Package SharpZipLib

public partial class github : System.Web.UI.Page
{
    protected GitHubClient client = new GitHubClient(new ProductHeaderValue("Autobuild"));
    protected void Page_Load(object sender, EventArgs e)
    {
        var username = ConfigurationManager.AppSettings["GITHUB_USERNAME"];
        var password = ConfigurationManager.AppSettings["GITHUB_PASSWORD"];
        var repository = ConfigurationManager.AppSettings["GITHUB_REPO"];
        var output = ConfigurationManager.AppSettings["DEPLOY_PATH"];
        client.Credentials = new Credentials(username, password);
        var tDownload = client.Repository.Content.GetArchive(username, repository);
        var bArchive = tDownload.Result;
        var stream = new MemoryStream(bArchive);      
        Stream gzipStream = new GZipInputStream(stream);
        Stream gzipStream2 = new GZipInputStream(stream);
        TarInputStream tarIn = new TarInputStream(gzipStream);
        TarEntry tarEntry;
        var strRoot = "";
        while ((tarEntry = tarIn.GetNextEntry()) != null)
        {
            string name = tarEntry.Name.Replace('/', Path.DirectorySeparatorChar);
            if (strRoot == "") strRoot = name;
            if (tarEntry.IsDirectory)
                continue;                                
            if (Path.IsPathRooted(name))
                name = name.Substring(Path.GetPathRoot(name).Length);
            name = name.Replace(strRoot, "");
            string outName = Path.Combine(output, name);
            string directoryName = Path.GetDirectoryName(outName);
            Directory.CreateDirectory(directoryName);
            FileStream outStr = new FileStream(outName, System.IO.FileMode.Create);
            tarIn.CopyEntryContents(outStr);
            outStr.Close();
        }
        tarIn.Close();
        gzipStream.Close();
        stream.Close();
      
    }


}