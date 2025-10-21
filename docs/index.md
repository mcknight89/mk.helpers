# mk.helpers

A comprehensive collection of utility and extension methods designed to enhance your .NET development experience. With a wide array of helper methods, this library aims to simplify various tasks and processes, making your development journey a breeze.

## Features

- **Extension Methods**: Powerful extensions for common .NET types (DateTime, List, Object, String, etc.)
- **Threading Utilities**: Advanced thread management with `ThreadLord` for efficient task processing
- **Performance Tools**: `PerfTimer` for accurate performance measurement
- **Helper Classes**: 
  - `EnumHelper` - Enum utilities
  - `IpHelper` - IP address operations
  - `RandomHelper` - Enhanced random generation
  - `ReflectionHelper` - Reflection utilities
  - `StaticData` - Common static data access

## Getting Started

Install the NuGet package:

```bash
dotnet add package mk.helpers
```

## API Documentation

Browse the complete [API Reference](api/index.md) to explore all available helpers and extensions.

## Examples

### ThreadLord - Efficient Task Processing

```csharp
var threadLord = new ThreadLord(maxConcurrency: 4, maxQueueSize: 100);
threadLord.OnError = ex => Console.WriteLine($"Error: {ex.Message}");

threadLord.Enqueue(() => {
    // Your work here
});

// Check statistics
Console.WriteLine($"Completed: {threadLord.Completed}, Failed: {threadLord.Failed}");
```

### PerfTimer - Performance Measurement

```csharp
using (var timer = new PerfTimer())
{
    // Code to measure
    DoSomething();
    
    Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds}ms");
}
```

## Source Code

GitHub: [https://github.com/mcknight89/mk.helpers](https://github.com/mcknight89/mk.helpers)

## License

MIT License - see the [repository](https://github.com/mcknight89/mk.helpers) for details.
