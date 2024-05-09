using System.Net.Http.Json;

namespace AzureIpRangeExplorer.Pages;

public partial class RangeExplorer
{
    string? ipRangesResource = $"azure-ranges/latest.json";
    private AzureIpRanges? ranges;

    private IEnumerable<string> Regions => 
        ranges?.Values.Select(range => range.Properties.Region)
            .Distinct() 
            .Where(region => !string.IsNullOrWhiteSpace(region)) ?? [];

    private string? SelectedRegion { get; set; }

    private IEnumerable<string> Services => 
        ranges?.Values.Select(range => range.Properties.SystemService)
            .Distinct()
            .Where(service => !string.IsNullOrWhiteSpace(service)) ?? [];

    private string? SelectedService { get; set; }
    
    private bool DisableRegionFilter => string.IsNullOrWhiteSpace(SelectedService);
    
    private AzureServiceRange? SelectedRange => 
        ranges?.Values.FirstOrDefault(range => 
            range.Properties.Region == (SelectedRegion ?? string.Empty)  && 
            range.Properties.SystemService == SelectedService);

    protected override async Task OnInitializedAsync()
    {
        ranges = await Http.GetFromJsonAsync<AzureIpRanges>(this.ipRangesResource);
    }

    public record AzureIpRanges(int ChangeNumber, string Cloud, AzureServiceRange[] Values);

    public record AzureServiceRange(string Name, AzureServiceRangeProperties Properties);

    public record AzureServiceRangeProperties(string Region, string[] AddressPrefixes, string SystemService);
}