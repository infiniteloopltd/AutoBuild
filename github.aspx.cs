using System;

public partial class github : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string headers = String.Empty;
        foreach (string key in Request.Headers.AllKeys)
            headers += key + "=" + Request.Headers[key] + Environment.NewLine;
        headers += Environment.NewLine + Environment.NewLine + Request.Url.Query;

        foreach (string key in Request.Form.AllKeys)
            headers += key + "=" + Request.Form[key] + Environment.NewLine;

        Email.Send("fiach.reid@gmail.com", "GitHub webhook", headers);
    }
}