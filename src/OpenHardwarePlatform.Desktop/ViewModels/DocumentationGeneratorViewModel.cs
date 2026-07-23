using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Text.Json;

namespace OpenHardwarePlatform.Desktop.ViewModels;

public partial class DocumentationGeneratorViewModel : ObservableObject
{
    [ObservableProperty]
    private string _deviceName = string.Empty;

    [ObservableProperty]
    private string _vendorInfo = string.Empty;

    [ObservableProperty]
    private string _descriptorInfo = string.Empty;

    [ObservableProperty]
    private string _outputPath = string.Empty;

    [ObservableProperty]
    private string _statusText = "Ready";

    public ObservableCollection<DocSection> Sections { get; } = new();

    [RelayCommand]
    public async Task GenerateMarkdown()
    {
        var path = string.IsNullOrWhiteSpace(OutputPath)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "openhid_doc.md")
            : OutputPath;

        var md = $"# HID Device Documentation\n\n";
        md += $"## Device Information\n";
        md += $"- **Name:** {DeviceName}\n";
        md += $"- **Vendor:** {VendorInfo}\n\n";
        md += $"## Report Descriptor\n```\n{DescriptorInfo}\n```\n\n";
        
        foreach (var section in Sections)
        {
            md += $"## {section.Title}\n{section.Content}\n\n";
        }

        await File.WriteAllTextAsync(path, md);
        StatusText = $"Generated: {path}";
    }

    [RelayCommand]
    public async Task GenerateHtml()
    {
        var path = string.IsNullOrWhiteSpace(OutputPath)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "openhid_doc.html")
            : OutputPath;

        var html = $"<!DOCTYPE html><html><head><title>HID Device Documentation</title>";
        html += "<style>body{font-family:Arial;max-width:800px;margin:auto;padding:20px}";
        html += "h1{color:#6750A4}pre{background:#f5f5f5;padding:10px;border-radius:4px}</style></head><body>";
        html += $"<h1>HID Device Documentation</h1>";
        html += $"<h2>Device Information</h2><ul>";
        html += $"<li><strong>Name:</strong> {DeviceName}</li>";
        html += $"<li><strong>Vendor:</strong> {VendorInfo}</li></ul>";
        html += $"<h2>Report Descriptor</h2><pre>{DescriptorInfo}</pre>";
        html += "</body></html>";

        await File.WriteAllTextAsync(path, html);
        StatusText = $"Generated: {path}";
    }

    [RelayCommand]
    public async Task GenerateJson()
    {
        var path = string.IsNullOrWhiteSpace(OutputPath)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "openhid_doc.json")
            : OutputPath;

        var doc = new
        {
            DeviceName,
            VendorInfo,
            DescriptorInfo,
            Sections = Sections.Select(s => new { s.Title, s.Content }).ToList()
        };

        var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
        StatusText = $"Generated: {path}";
    }

    [RelayCommand]
    public void AddSection()
    {
        Sections.Add(new DocSection { Title = "New Section", Content = "Content here..." });
    }

    [RelayCommand]
    public void Clear()
    {
        DeviceName = string.Empty;
        VendorInfo = string.Empty;
        DescriptorInfo = string.Empty;
        Sections.Clear();
        StatusText = "Cleared";
    }
}

public class DocSection
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}