using System;

//https://stackoverflow.com/questions/1435753/asp-net-access-to-network-share/1435789#1435789

internal static class NetworkDrives {
    internal static bool MapDrive(string server) {
        bool ReturnValue = false;
        string Path = "";
        string Username = "";
        string Password = "";
        if (server == "iboxx") {
            Path = System.Configuration.ConfigurationManager.AppSettings["IBoxxServerPath"];
            Username = System.Configuration.ConfigurationManager.AppSettings["IBoxxUserName"];
            Password = System.Configuration.ConfigurationManager.AppSettings["IBoxxPassword"];
        } else if (server == "web") {
            Path = System.Configuration.ConfigurationManager.AppSettings["WebServerPath"];
            Username = System.Configuration.ConfigurationManager.AppSettings["WebUserName"];
            Password = System.Configuration.ConfigurationManager.AppSettings["WebPassword"];
        }

        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardOutput = true;

        p.StartInfo.FileName = "net.exe";
        p.StartInfo.Arguments = " use " + Path + " " + Password + " /user:" + Username;
        p.Start();
        p.WaitForExit();

        string ErrorMessage = p.StandardError.ReadToEnd();
        string OuputMessage = p.StandardOutput.ReadToEnd();
        if (ErrorMessage.Length > 0) {
            throw new Exception("Error:" + ErrorMessage);
        } else {
            ReturnValue = true;
        }
        return ReturnValue;
    }

    internal static bool DisconnectDrive(string server) {
        string Path = "";
        if (server == "iboxx") {
            Path = System.Configuration.ConfigurationManager.AppSettings["IBoxxServerPath"];
        } else if (server == "web") {
            Path = System.Configuration.ConfigurationManager.AppSettings["WebServerPath"];
        } else if (server == "eee") {
            Path = System.Configuration.ConfigurationManager.AppSettings["eee1"];
        }
        bool ReturnValue = false;
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardOutput = true;

        p.StartInfo.FileName = "net.exe";
        p.StartInfo.Arguments = " use " + Path + " /DELETE";
        p.Start();
        p.WaitForExit();

        string ErrorMessage = p.StandardError.ReadToEnd();
        string OuputMessage = p.StandardOutput.ReadToEnd();
        if (ErrorMessage.Length > 0) {
            throw new Exception("Error:" + ErrorMessage);
        } else {
            ReturnValue = true;
        }
        return ReturnValue;
    }
}