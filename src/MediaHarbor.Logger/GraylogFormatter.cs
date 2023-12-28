using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core;

namespace MediaHarbor.Logger;

public class GraylogFormatter : ITextFormatter
{
    private readonly Lazy<IGelfConverter> _converter;
        
    public GraylogFormatter() 
    {
        ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(new GraylogSinkOptions());
        _converter = new Lazy<IGelfConverter>(() => sinkComponentsBuilder.MakeGelfConverter());
    }
        
    public void Format(LogEvent logEvent, TextWriter output) 
    {
        output.Write(_converter.Value.GetGelfJson(logEvent).ToString());
    }
}