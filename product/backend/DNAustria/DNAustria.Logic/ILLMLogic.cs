using System.Threading.Tasks;

namespace DNAustria.Logic;

public interface ILLMLogic
{
    Task<string?> GetChatCompletionAsync(string prompt);
}
