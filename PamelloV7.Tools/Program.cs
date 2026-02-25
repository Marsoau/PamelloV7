using System.Collections;
using System.Reflection;
using System.Text;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Tools.Generators;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Commands;

namespace PamelloV7.Tools;

class Program
{
    public static void Main(string[] args) {
        GenerateEventsDto();
    }
    
    public static void GenerateEventsDto() {
        var directory = Directory.CreateDirectory("../../../../PamelloV7.Wrapper/Events/Dto");
        
        EventsDtoGenerator.Generate(directory);
    }

    public static void GenerateCommands() {
        var directory = Directory.CreateDirectory("../../../../PamelloV7.Wrapper/Extensions");
        
        CommandExtensionsGenerator.Generate(directory);
    }
}
