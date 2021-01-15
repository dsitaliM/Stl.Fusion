using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Stl.CommandR;
using Stl.CommandR.Commands;
using Stl.CommandR.Configuration;

namespace Stl.Fusion.Operations.Internal
{
    public class LocalCompletionCommandProducer : ICommandHandler<ICommand>
    {
        public class Options
        {
            public LogLevel LogLevel { get; set; } = LogLevel.None;
        }

        protected LogLevel LogLevel { get; }
        protected ILogger Log { get; }

        public LocalCompletionCommandProducer(Options? options,
            IInvalidationInfoProvider invalidationInfoProvider,
            ILogger<InvalidateCompletedCommandHandler>? log = null)
        {
            options ??= new();
            Log = log ?? NullLogger<InvalidateCompletedCommandHandler>.Instance;
            LogLevel = options.LogLevel;
        }

        [CommandHandler(Order = -10_000, IsFilter = true)]
        public async Task OnCommandAsync(ICommand command, CommandContext context, CancellationToken cancellationToken)
        {
            await context.InvokeRemainingHandlersAsync(cancellationToken).ConfigureAwait(false);

            var requiresCompletion =
                context.OuterContext == null // Should be a top-level command
                && !(command is IMetaCommand) // Shouldn't be a "second-order" command
                && !Computed.IsInvalidating(); // Invalidating = definitely not a real operation
            if (!requiresCompletion)
                return;

            var completionCommand = context.Items.TryGet<ICompletionCommand>() ?? CompletionCommand.New(command);
            await context.Commander.RunAsync(completionCommand, true, default).ConfigureAwait(false);
        }
    }
}
