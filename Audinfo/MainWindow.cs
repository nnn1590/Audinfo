﻿using System;
using System.IO;
using Gtk;
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
        Application.Quit();
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

    public void ConvertFile(string filename, string OutputFormat)
    {
        FISP file;
        string outputfilepath;

        switch (filename.Substring(filename.Length - 4))
        {
            case ".wav":
                RiffWave w = new RiffWave();
                w.Load(File.ReadAllBytes(filename));
                file = new FISP(w);
                outputfilepath = entry_output_file_path.Text;
                if (checkbutton_output_copy_input_name.Active)
                {
                    int lastDotLocate = entry_input_file_path.Text.LastIndexOf('.');
                    if (lastDotLocate > 0)
                    {
                        if (OutputFormat == "BWAV") outputfilepath = entry_input_file_path.Text.Substring(0, lastDotLocate) + ".bwav";
                        if (OutputFormat == "WAV") outputfilepath = entry_input_file_path.Text.Substring(0, lastDotLocate) + ".wav";
                    }
                    else
                    {
                        if (OutputFormat == "BWAV") outputfilepath = entry_input_file_path.Text + ".bwav";
                        if (OutputFormat == "WAV") outputfilepath = entry_input_file_path.Text + ".wav";
                    }
                }
                //MsgBox("in?:" + filename +"\nout:"+outputfilepath);
                file.stream.isLoop = checkbutton_looping.Active;
                file.stream.loopStart = (uint)spinbutton_loop_start.Value;
                file.stream.loopEnd = (uint)spinbutton_loop_end.Value;
                //File.WriteAllBytes(outputfilepath, BinaryWave.FromRiff(w).ToBytes());
                if (OutputFormat == "BWAV") File.WriteAllBytes(outputfilepath, BinaryWave.FromFISP(file).ToBytes());
                if (OutputFormat == "WAV") File.WriteAllBytes(outputfilepath, RiffWaveFactory.CreateRiffWave(file).ToBytes());
                break;
            case "bwav":
                BinaryWave r = new BinaryWave();
                r.Load(File.ReadAllBytes(filename));
                file = new FISP(r);
                outputfilepath = entry_output_file_path.Text;
                if (checkbutton_output_copy_input_name.Active)
                {
                    int lastDotLocate = entry_input_file_path.Text.LastIndexOf('.');
                    if (lastDotLocate > 0)
                    {
                        if (OutputFormat == "BWAV") outputfilepath = entry_input_file_path.Text.Substring(0, lastDotLocate) + ".bwav";
                        if (OutputFormat == "WAV") outputfilepath = entry_input_file_path.Text.Substring(0, lastDotLocate) + ".wav";
                    }
                    else
                    {
                        if (OutputFormat == "BWAV") outputfilepath = entry_input_file_path.Text + ".bwav";
                        if (OutputFormat == "WAV") outputfilepath = entry_input_file_path.Text + ".wav";
                    }
                }
                //riffWave
                file.stream.encoding = (byte)1;
                if (OutputFormat == "BWAV") File.WriteAllBytes(outputfilepath, file.ToBytes());
                if (OutputFormat == "WAV") File.WriteAllBytes(outputfilepath, RiffWaveFactory.CreateRiffWave(file).ToBytes());
                break;
            default:
                MsgBox("Unknown type - Only WAV and BWAV are supported.\nIf file isn't WAV or BWAV but like Ogg, convert before. (Oddly, some WAVs crash. Exporting with Audacity may work.)\nThis check depends on the file name (mainly the extension), not the actual file contents.\nIf it is WAV or BWAV, please change the file name.\nBut BWAV input support is too bad. In that case, try using VGMStream.", "Error", MessageType.Error, ButtonsType.Ok);
                return;
        }
        //file.stream
        /*MsgBox("file.stream.isLoop: \"" + (file.stream.isLoop.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.loopStart: \"" + (file.stream.loopStart.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.loopEnd: \"" + (file.stream.loopEnd.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.originalLoopStart: \"" + (file.stream.originalLoopStart.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.originalLoopEnd: \"" + (file.stream.originalLoopEnd.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.encoding: \"" + (file.stream.encoding.ToString() ?? "NULL!!!") + "\"\n"
        //+ "file.stream.magic: \"" + (file.stream.magic.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.sampleRate: \"" + (file.stream.sampleRate.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.secretInfo: \"" + (file.stream.secretInfo.ToString() ?? "NULL!!!") + "\"\n");*/
    }

    bool IsReadable(string filepath, bool showDialog)
    {
        if (Directory.Exists(filepath) == true)
        {
            if (showDialog) MsgBox("\"" + filepath + "\" is directory", "Error", MessageType.Error, ButtonsType.Ok);
            return false;
        }
        try
        {
            if (File.Exists(filepath) == false)
            {
                if (showDialog) MsgBox("File not found or can't read: \"" + filepath + "\"\nCheck file path and permissions.", "Error", MessageType.Error, ButtonsType.Ok);
                return false;
            }
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                if (!fs.CanRead)
                {
                    try
                    {
                        if (File.Exists(filepath) == false)
                        {
                            if (showDialog) MsgBox("File not found: \"" + filepath + "\"", "Error", MessageType.Error, ButtonsType.Ok);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (showDialog) MsgBox("Can't read: \"" + filepath + "\"\nCheck file permissions.", "Error", MessageType.Error, ButtonsType.Ok);
                    }
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            if (showDialog) MsgBox("Can't read: \"" + filepath + "\"\nCheck file permissions.", "Error", MessageType.Error, ButtonsType.Ok);
            return false;
        }
        return true;
    }

    protected void OnButtonLoadClicked(object sender, EventArgs e)
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
        if (!IsReadable(filepath,true)) return;
        entry_loaded_file_path.Text = filepath;
        FISP file;

        switch (filepath.Substring(filepath.Length - 4))
        {
            case ".wav":
                entry_loaded_file_format.Text = "WAV";
                RiffWave w = new RiffWave();
                w.Load(File.ReadAllBytes(filepath));
                file = new FISP(w);
                break;
            case "bwav":
                entry_loaded_file_format.Text = "BWAV";
                BinaryWave r = new BinaryWave();
                if (r.Codic == 0) entry_loaded_file_format.Text = "BWAV(PCM16)";
                if (r.Codic == 1) entry_loaded_file_format.Text = "BWAV(DSPADPCM)";
                //RiffWave riffWave = new RiffWave();
                r.Load(File.ReadAllBytes(filepath));
                file = new FISP(r);
                break;
            default:
                entry_loaded_file_format.Text = "Unknown";
                MsgBox("Unknown type - Only WAV and BWAV are supported.\nIf file isn't WAV or BWAV but like Ogg, convert before. (Oddly, some WAVs crash. Exporting with Audacity may work.)\nThis check depends on the file name (mainly the extension), not the actual file contents.\nIf it is WAV or BWAV, please change the file name.\nBut BWAV input support is too bad. In that case, try using VGMStream.", "Error", MessageType.Error, ButtonsType.Ok);
                return;
        }

        checkbutton_looping.Sensitive = spinbutton_loop_start.Sensitive = spinbutton_loop_end.Sensitive = hbox_convert_buttons.Sensitive = true;
        checkbutton_looping.Inconsistent = false;
        spinbutton_loop_start.Adjustment.Upper = spinbutton_loop_end.Adjustment.Upper = double.MaxValue;
        checkbutton_looping.Active = file.stream.isLoop;
        spinbutton_loop_start.Value = file.stream.loopStart;
        spinbutton_loop_end.Value = file.stream.loopEnd;
        //MsgBox(spinbutton_loop_start.Adjustment.Upper.ToString()+"\n" +spinbutton_loop_end.Adjustment.Upper.ToString());
        MsgBox("file.stream.isLoop: \"" + (file.stream.isLoop.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.loopStart: \"" + (file.stream.loopStart.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.loopEnd: \"" + (file.stream.loopEnd.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.originalLoopStart: \"" + (file.stream.originalLoopStart.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.originalLoopEnd: \"" + (file.stream.originalLoopEnd.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.encoding: \"" + (file.stream.encoding.ToString() ?? "NULL!!!") + "\"\n"
        //+ "file.stream.magic: \"" + (file.stream.magic.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.sampleRate: \"" + (file.stream.sampleRate.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.secretInfo: \"" + (file.stream.secretInfo.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.vMajor: \"" + (file.stream.vMajor.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.vMinor: \"" + (file.stream.vMinor.ToString() ?? "NULL!!!") + "\"\n"
        + "file.stream.vRevision: \"" + (file.stream.vRevision.ToString() ?? "NULL!!!") + "\"");
    }

    protected void OnMapEvent(object o, MapEventArgs args)
    {
        int x, y;
        GetSize(out x, out y);
        WidthRequest = x;
        HeightRequest = y;
        Resizable = true;
        Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " (Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + ")";
        label_smile.Visible = true;
    }

    protected void OnEntryInputFilePathChanged(object sender, EventArgs e)
    {
        if (checkbutton_output_copy_input_name.Active)
        {
            int lastDotLocate = entry_input_file_path.Text.LastIndexOf('.');
            if (lastDotLocate > 0)
            {
                // Bad result if path is like "/i.have.dot/i_have_no_dot" or "/home/owner/My.Dump/music_001" ...
                entry_output_file_path.Text = entry_input_file_path.Text.Substring(0, lastDotLocate) + ".{,b}wav";
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
        var filepath = entry_loaded_file_path.Text;
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(filepath);
        try
        {
            stringBuilder.Replace("~", Environment.GetEnvironmentVariable("HOME"), 0, 1);
        }
        catch (Exception ex)
        {

        }
        filepath = stringBuilder.ToString();
        if (!IsReadable(filepath, true)) return;

        ConvertFile(filepath, "BWAV");
    }

    protected void OnButtonConvertToWavClicked(object sender, EventArgs e)
    {
        var filepath = entry_loaded_file_path.Text;
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(filepath);
        try
        {
            stringBuilder.Replace("~", Environment.GetEnvironmentVariable("HOME"), 0, 1);
        }
        catch (Exception ex)
        {

        }
        filepath = stringBuilder.ToString();
        if (!IsReadable(filepath, true)) return;

        ConvertFile(filepath, "WAV");
    }

    protected void OnButtonInputChooseClicked(object sender, EventArgs e)
    {
        FileChooserDialog fileChooserDialog;
        fileChooserDialog = new FileChooserDialog("Choose input file", this, FileChooserAction.Open,("Cancel"),ResponseType.Cancel,("Open"),ResponseType.Accept,null);
        if (File.Exists(entry_input_file_path.Text) || Directory.Exists(entry_input_file_path.Text)) fileChooserDialog.SetFilename(entry_input_file_path.Text);
        fileChooserDialog.Show();
        if (fileChooserDialog.Run() == -3) entry_input_file_path.Text = fileChooserDialog.Filename;
        fileChooserDialog.Destroy();
    }

    protected void OnButtonOutputChooseClicked(object sender, EventArgs e)
    {
        FileChooserDialog fileChooserDialog;
        fileChooserDialog = new FileChooserDialog("Choose output file", this, FileChooserAction.Save, ("Cancel"), ResponseType.Cancel, ("Save"), ResponseType.Accept,null);
        try
        {
            if (File.Exists(entry_output_file_path.Text) || Directory.Exists(entry_output_file_path.Text)) fileChooserDialog.SetFilename(entry_output_file_path.Text); else if (Directory.Exists(System.IO.Path.GetDirectoryName(entry_output_file_path.Text))) fileChooserDialog.SetFilename(System.IO.Path.GetDirectoryName(entry_output_file_path.Text));
        }
        catch(Exception ex)
        { 
        }
        fileChooserDialog.Show();
        if (fileChooserDialog.Run() == -3)
        {
            entry_output_file_path.Text = fileChooserDialog.Filename;
            checkbutton_output_copy_input_name.Active = false;
        }
        fileChooserDialog.Destroy();
    }
}
