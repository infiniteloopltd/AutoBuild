using Octokit; // Install-Package Octokit 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

public partial class github : System.Web.UI.Page
{
    protected GitHubClient client = new GitHubClient(new ProductHeaderValue("Autobuild"));
    protected void Page_Load(object sender, EventArgs e)
    {
        var username = ConfigurationManager.AppSettings["GITHUB_USERNAME"];
        var password = ConfigurationManager.AppSettings["GITHUB_PASSWORD"];        
        client.Credentials = new Credentials(username, password); 
        CloneRepo();
    }

    private void CloneRepo(RepositoryContent repo = null)
    {
        var output = ConfigurationManager.AppSettings["DEPLOY_PATH"];
        var username = ConfigurationManager.AppSettings["GITHUB_USERNAME"];
        var repository = ConfigurationManager.AppSettings["GITHUB_REPO"];
        Task<IReadOnlyList<RepositoryContent>> task;
        if (repo == null)
        {
            task = client.Repository.Content.GetAllContents(username, repository);
        }
        else
        {
            task = client.Repository.Content.GetAllContents(username, repository, repo.Path);
            Thread.Sleep(TimeSpan.FromSeconds(5)); // Rate limiting.
        }        
        var repoContent = task.Result;
        var wc = new WebClient();
        foreach (var file in repoContent)
        {
            if (file.Type == "Dir")
            {
                Response.Write("Creating: " + output + file.Path + "<br>");
                Response.Flush();  // Dubug
                Directory.CreateDirectory(output + file.Path);
                CloneRepo(file);
            }
            if (file.DownloadUrl == null) continue;
            var bDownload = wc.DownloadData(file.DownloadUrl);
            File.WriteAllBytes(output + file.Path, bDownload);
            Response.Write("Writing: " + output + file.Path + "<br>");
            Response.Flush(); // Dubug
        }
    }
}