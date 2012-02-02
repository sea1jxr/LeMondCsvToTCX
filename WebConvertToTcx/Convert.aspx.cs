using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using ConvertToTcx;
using System.Collections.Specialized;
using System.Drawing;
using System.Diagnostics;

namespace WebConvertToTcx
{
    public partial class Convert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private List<HttpPostedFile> GatherFiles()
        {
            var files = new List<HttpPostedFile>();
            if (!Lap1File.HasFile)
            {
                throw new Exception("A file was not specified for Lap1.");
            }

            files.Add(Lap1File.PostedFile);

            if (Lap2File.HasFile)
            {
                files.Add(Lap2File.PostedFile);
            }

            if (Lap3File.HasFile)
            {
                if (!Lap2File.HasFile)
                {
                    throw new Exception("A Lap3 file was given, but no Lap2 file was specified.");
                }

                files.Add(Lap3File.PostedFile);
            }

            return files;
        }

        private void WriteError(string message)
        {
            FailureText.Text = message;
        }

        protected void Convert_Click(object sender, EventArgs e)
        {
            try
            {
                var files = GatherFiles();
                Debug.Assert(files.Count > 0, "No files were recieved and no error was thrown.");

                var stream = new MemoryStream();
                using (TextWriter textWriter = new StreamWriter(stream))
                {
                    var sourceStreams = files.Select(f => new SourcedStream() { Stream = f.InputStream, Source = f.FileName });
                    new Converter().WriteTcxFile(sourceStreams, textWriter);
                    textWriter.Flush();
                }
                // don't mess up the response until we get through the conversion error free
                Response.Buffer = true;
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=" + files.First().FileName + ".convertedto.tcx");
                // got this content type from garmin's connect.garming.com export to tcx feature
                Response.ContentType = "application/vnd.garmin.tcx+xml;charset=utf-8";
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }
        }
    }
}