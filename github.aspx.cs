using Octokit; // Install-Package Octokit 
using System;
using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.GZip; // Install-Package SharpZipLib
using ICSharpCode.SharpZipLib.Tar; // Install-Package SharpZipLib
using System.Threading;
using Newtonsoft.Json.Linq;

public partial class github : System.Web.UI.Page
{
    protected GitHubClient client = new GitHubClient(new ProductHeaderValue("Autobuild"));
    string username = ConfigurationManager.AppSettings["GITHUB_USERNAME"];
    string password = ConfigurationManager.AppSettings["GITHUB_PASSWORD"];
    string repository = ConfigurationManager.AppSettings["GITHUB_REPO"];
    string output = ConfigurationManager.AppSettings["DEPLOY_PATH"];
    protected void Page_Load(object sender, EventArgs e)
    {
        var strLog = "Started at " + DateTime.Now;
        client.Credentials = new Credentials(username, password);
        var strJsonPayload = Request.Form["Payload"];
        string strExpectedCommit = "";
        if (!string.IsNullOrEmpty(strJsonPayload))
        {
            strLog += "\nGitHub payload " + strJsonPayload; 
            var jPayload = JObject.Parse(strJsonPayload);
            strExpectedCommit = jPayload["after"].ToString().Substring(0, 7);
            strLog += "\nExpected Commit " + strExpectedCommit;
        }
        for(int i=0;i<10;i++)
        {
            var blnOK = DownloadArchive(strExpectedCommit);
            if (blnOK) break;
            Thread.Sleep(TimeSpan.FromSeconds(5));
            strLog += "\nRetry " + i;
        }
        File.WriteAllText(output + "buildlog.txt", strLog);
    }

    private bool DownloadArchive(string expectedCommit)
    {
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
            if (strRoot == "")
            {
                strRoot = name;
                if (!strRoot.Contains(expectedCommit)) return false;
            }
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
        return true;
    }


}