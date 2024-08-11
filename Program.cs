using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using Serilog;

namespace aws_dyndns;

class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", "Log.txt"),
                rollingInterval: RollingInterval.Month,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();
        
        // Replace with your AWS credentials
        TextReader textReader = new StreamReader("config.json");
        Config? config =  JsonConvert.DeserializeObject<Config>(textReader.ReadToEnd());
        AWSCredentials credentials = new BasicAWSCredentials(config.AccessKey,config.SecretKey);
        AmazonRoute53Client route53Client = new AmazonRoute53Client(credentials, RegionEndpoint.EUCentral1);
        ChangeBatch changeBatch = new ChangeBatch();
        var we = new HttpClient().GetAsync("https://api.ipify.org").Result.Content.ReadAsStream();
        TextReader te = new StreamReader(we);
        var res = new ResourceRecordSet
        {
            Name = config.Domain, // Your domain name
            Type = RRType.A,
            TTL = 60, // Time to live in seconds
            ResourceRecords = 
            {
                new ResourceRecord
                {
                    Value =te.ReadToEnd()
                }
                
            }
        };
        changeBatch.Changes.Add(new Change(ChangeAction.UPSERT,res));
        ChangeResourceRecordSetsRequest changeResourceRecordSetsRequest = new ChangeResourceRecordSetsRequest( config.HostedZoneId , changeBatch);
        var r= route53Client.ChangeResourceRecordSetsAsync(changeResourceRecordSetsRequest);
        Console.WriteLine(r.Result.HttpStatusCode);
        Log.Information(r.Result.HttpStatusCode.ToString());
    }
}

class Config
{
    public Config()
    {
    }

    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Domain { get; set; }
    public string HostedZoneId { get; set; }
}