using System;
using System.IO;
using Gtk;
using ATL;
using ATL.AudioData;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnQuitActionActivated(object sender, EventArgs e)
    {
        OnDeleteEvent(sender, (DeleteEventArgs)e);
    }

    int MsgBox(string text, string title, MessageType msgtype, ButtonsType buttontype)
    {
        var dialog = new MessageDialog(this, DialogFlags.Modal, msgtype, buttontype, text);
        dialog.Title = title;
        dialog.Show();
        int result = dialog.Run();
        dialog.Destroy();
        return result;
    }

    protected void OnButtonViewActivated(object sender, EventArgs e)
    {
        var filepath = entry_file_path.Text;
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(filepath);
        try
        {
            stringBuilder.Replace("~", Environment.GetEnvironmentVariable("HOME"), 0, 1);
        }
        catch (Exception ex)
        {

        }
        filepath = stringBuilder.ToString();

        if (Directory.Exists(filepath) == true)
        {
            MsgBox("\"" + filepath + "\" is directory", "Error", MessageType.Error, ButtonsType.Ok);
            return;
        }
        try
        {
            if (File.Exists(filepath) == false)
            {
                MsgBox("File not found or can't read: \"" + filepath + "\"\nCheck file path and permissions.", "Error", MessageType.Error, ButtonsType.Ok);
                return;
            }
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                if (!fs.CanRead)
                {
                    try
                    {
                        if (File.Exists(filepath) == false)
                        {
                            MsgBox("File not found: \"" + filepath + "\"", "Error", MessageType.Error, ButtonsType.Ok);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MsgBox("CaAn't read: \"" + filepath + "\"\nCheck file permissions.", "Error", MessageType.Error, ButtonsType.Ok);
                    }
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            MsgBox("Can't read: \"" + filepath + "\"\nCheck file permissions.", "Error", MessageType.Error, ButtonsType.Ok);
            return;
        }
        entry_loaded_file_path.Text = filepath;
        Track theTrack = new Track(filepath);

        MsgBox("Title: \"" + theTrack.Title + "\"\nDuration (ms): \"" + theTrack.DurationMs + "\"\nDuration: \"" + theTrack.Duration + "\"", "Info", MessageType.Info, ButtonsType.Ok);

        System.Text.StringBuilder filter = new System.Text.StringBuilder("");

        //ATL.AudioData.AudioDataManager audioDataManager = new AudioDataManager(audioDataIO);
        ATL.Playlist.PlaylistIOFactory.GetInstance();
        foreach (Format f in ATL.Playlist.PlaylistIOFactory.GetInstance().getFormats())
        {
            if (f.Readable)
            {
                foreach (string extension in f)
                {
                    filter.Append(extension).Append(";");
                }
            }
        }
        // Removes the last separator
        filter.Remove(filter.Length - 1, 1);
        MsgBox("Filter(s): \"" + filter.ToString() + "\"", "Info", MessageType.Info, ButtonsType.Ok);

    }
}
