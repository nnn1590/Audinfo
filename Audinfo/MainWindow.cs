using System;
using System.IO;
using Gtk;
//using ATL;
//using ATL.AudioData;
//using NAudio.Wave;
//using NAudio.Wave.SampleProviders;
using CitraFileLoader;
using IsabelleLib;

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
    int MsgBox(string text)
    {
        var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Other, ButtonsType.Ok, text);
        dialog.Show();
        int result = dialog.Run();
        dialog.Destroy();
        return result;
    }

    public FISP file;
    public void ConvertFile(string filename)
    {
        switch (filename.Substring(filename.Length - 4))
        {
            case ".wav":
                entry_loaded_file_format.Text = "WAV";
                RiffWave w = new RiffWave();
                w.Load(File.ReadAllBytes(filename));
                file = new FISP(w);
                string outputfilepath = entry_output_file_path.Text;
                if (checkbutton_output_copy_input_name.Active)
                {
                    int lastDotLocate = entry_input_file_path.Text.LastIndexOf('.');
                    if (lastDotLocate > 0)
                    {
                        outputfilepath = entry_input_file_path.Text.Substring(0, lastDotLocate) + ".bwav";
                    }
                    else
                    {
                        outputfilepath = entry_input_file_path.Text + ".bwav";
                    }
                }
                File.WriteAllBytes(outputfilepath, BinaryWave.FromFISP(file).ToBytes());
                break;
            case "bwav":
                entry_loaded_file_format.Text = "BWAV";
                MsgBox("Unsupported type - Sorry, BWAV input support is too bad. Try using VGMStream.\nNOTE: But if you want to try it with this tool, change the source code ...", "Warning", MessageType.Warning, ButtonsType.Ok);
                return;
                BinaryWave r = new BinaryWave();
                r.Load(File.ReadAllBytes(filename));
                file = new FISP(r);
                break;
            default:
                entry_loaded_file_format.Text = "Unknown";
                MsgBox("Uknown type - Only WAV and BWAV are supported.\nIf file isn't WAV or BWAV but like Ogg, convert before like: `ffmpeg -i your.ogg your.wav`\nThis check depends on the file name (mainly the extension), not the actual file contents.\nIf it is WAV or BWAV, please change the file name.\nBut BWAV input support is too bad. In that case, try using VGMStream.", "Error",MessageType.Error,ButtonsType.Ok);
                return;
        }
        //MsgBox(file.stream.isLoop.ToString() ?? "NULL!!!");
    }

    protected void OnButtonViewActivated(object sender, EventArgs e)
    {
        var filepath = entry_input_file_path.Text;
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
                        MsgBox("Can't read: \"" + filepath + "\"\nCheck file permissions.", "Error", MessageType.Error, ButtonsType.Ok);
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
        //Track theTrack = new Track(filepath);

        //MsgBox("Title: \"" + theTrack.Title + "\"\nDuration (ms): \"" + theTrack.DurationMs + "\"\nDuration: \"" + theTrack.Duration + "\"", "Info", MessageType.Info, ButtonsType.Ok);

        //System.Text.StringBuilder filter = new System.Text.StringBuilder("");

        //ATL.AudioData.AudioDataManager audioDataManager = new AudioDataManager(audioDataIO);
        /*ATL.Playlist.PlaylistIOFactory.GetInstance();
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
        filter.Remove(filter.Length - 1, 1);*/
        //MsgBox("Filter(s): \"" + filter.ToString() + "\"", "Info", MessageType.Info, ButtonsType.Ok);

        ConvertFile(filepath);
    }

    protected void OnMapEvent(object o, MapEventArgs args)
    {
        int x, y;
        this.GetSize(out x, out y);
        this.WidthRequest = x; 
        this.HeightRequest = y;
        this.Resizable = true;

        // Because the "View" button is actually "Convert to BWAV"...
        button_view.Label = button_convert_to_bwav.Label;
        button_view.Image = button_convert_to_bwav.Image;
    }

    protected void OnEntryInputFilePathChanged(object sender, EventArgs e)
    {
        if (checkbutton_output_copy_input_name.Active)
        {
            int lastDotLocate = entry_input_file_path.Text.LastIndexOf('.');
            if (lastDotLocate > 0) {
                // Bad result if path is like "/i.have.dot/i_have_no_dot" or "/home/owner/My.Dump/music_001" ...
                entry_output_file_path.Text = entry_input_file_path.Text.Substring(0,lastDotLocate) + ".{,b}wav";
            }
            else
            {
                entry_output_file_path.Text = entry_input_file_path.Text + ".{,b}wav";
            }
        }
    }

    protected void OnCheckbuttonOutputCopyInputNameToggled(object sender, EventArgs e)
    {
        OnEntryInputFilePathChanged(null, null);
    }

    protected void OnEntryOutputFilePathChanged(object sender, EventArgs e)
    {
        if (entry_output_file_path.HasFocus) checkbutton_output_copy_input_name.Active = false;
    }

    protected void OnButtonConvertToBwavClicked(object sender, EventArgs e)
    {
    }
}
